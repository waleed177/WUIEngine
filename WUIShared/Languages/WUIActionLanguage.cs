
using System;
using System.Collections.Generic;
using static WUIShared.Languages.WUIActionParser;
using static WUIShared.Languages.WUIActionTokenizer;

namespace WUIShared.Languages {
    public class WUIActionLanguage {
        private WUIActionParser parser;
        public delegate object FunctionDelegate(object[] args);
        public delegate void UserDefinedFunctionCall(string functionName, object[] args);
        public delegate void VariableSetDelegate(string[] path, object oldValue, object newValue);
        public delegate void ArraySetDelegate(string[] path, int index, object oldValue, object newValue);
        public delegate void PositionChangedDelegate(int row, int column);
        private Dictionary<string, FunctionDelegate> bindingDictionary;
        private Dictionary<string, object> variables;
        private Stack<Dictionary<string, object>> localVariables;

        Program parsedProgram;

        private object returnValueOfFunction;

        //readonly's
        readonly string[] argsPath = new string[] { "args" };
        readonly string[] thisPath = new string[] { "this" };
        readonly string[] funcPath = new string[] { "f" };

        public event VariableSetDelegate OnVariableSet;
        public event UserDefinedFunctionCall OnUserDefinedFunctionCall;
        public event UserDefinedFunctionCall OnUserDefinedFunctionCallEnd;
        public event ArraySetDelegate OnArraySet;
        public event PositionChangedDelegate OnPositionChanged;

        private Dictionary<object, string[]> variableReferenceToPath;

        public WUIActionLanguage() {
            parser = new WUIActionParser();
            bindingDictionary = new Dictionary<string, FunctionDelegate>();
            variables = new Dictionary<string, object>();
            localVariables = new Stack<Dictionary<string, object>>();
            variableReferenceToPath = new Dictionary<object, string[]>();

            SetVariable(argsPath, null);
            SetVariable(thisPath, null);
            SetVariable(funcPath, null);

            Bind("Return", (args) => {
                returnValueOfFunction = args[0];
                return null;
            });
        }

        public virtual void LoadCode(string code) {
            parser.LoadCode(";" + code); //TODO: FIGURE OUT WHY I NEED THE SEMICOLON.
            parsedProgram = parser.ParseCode();

        }

        public Action Compile() {
            return Compile(parsedProgram);
        }

        private Action Compile(Program prog) {
            Action res = () => { };

            foreach (var item in prog.body) {
                res += () => OnPositionChanged?.Invoke(item.row, item.column);
                if (item is FunctionCall functionCall) {
                    //TODO: Dont allow functions to not be explicitly declared before assigning?
                    if (bindingDictionary.ContainsKey(functionCall.functionName)) {
                        FunctionDelegate func = bindingDictionary[functionCall.functionName];
                        res += () => {
                            func(ComputeArguments(functionCall.arguments));
                        };
                    } else {
                        res += () => {
                            bindingDictionary[functionCall.functionName](ComputeArguments(functionCall.arguments));
                        };
                    }
                } else if (item is BinaryOperator binaryOperator) {
                    if (binaryOperator.left is Variable leftVariable) {
                        switch (binaryOperator.operatorName) {
                            case TokenTypes.Operator_Assign:
                                res += () => SetVariable(leftVariable.path, ComputeValue(binaryOperator.right));
                                break;
                            case TokenTypes.Operator_AddEqual:
                                res += () => SetVariable(leftVariable.path, (int)GetVariable(leftVariable.path) + (int)ComputeValue(binaryOperator.right));
                                break;
                            case TokenTypes.Operator_SubtractEqual:
                                res += () => SetVariable(leftVariable.path, (int)GetVariable(leftVariable.path) - (int)ComputeValue(binaryOperator.right));
                                break;
                            default:
                                break;
                        }
                    } else if (binaryOperator.left is ArrayIndexedObject arrayIndexedObject) {
                        switch (binaryOperator.operatorName) {
                            case TokenTypes.Operator_Assign:
                                res += () => {
                                    object[] array = (object[])GetVariable(arrayIndexedObject.arrayName.path);
                                    int index = (int)ComputeValue(arrayIndexedObject.arrayIndice);
                                    object oldVal = array[index];
                                    array[index] = ComputeValue(binaryOperator.right);
                                    OnArraySet?.Invoke(arrayIndexedObject.arrayName.path, index, oldVal, array[index]);
                                };
                                break;
                            case TokenTypes.Operator_AddEqual:
                                res += () => {
                                    object[] array = (object[])GetVariable(arrayIndexedObject.arrayName.path);
                                    int index = (int)ComputeValue(arrayIndexedObject.arrayIndice);

                                    object oldVal = array[index];
                                    array[index] = (int)array[index] + (int)ComputeValue(binaryOperator.right);
                                    OnArraySet?.Invoke(arrayIndexedObject.arrayName.path, index, oldVal, array[index]);
                                };
                                break;
                            case TokenTypes.Operator_SubtractEqual:
                                res += () => {
                                    object[] array = (object[])GetVariable(arrayIndexedObject.arrayName.path);
                                    int index = (int)ComputeValue(arrayIndexedObject.arrayIndice);
                                    object oldVal = array[index];
                                    array[index] = (int)array[index] - (int)ComputeValue(binaryOperator.right);
                                    OnArraySet?.Invoke(arrayIndexedObject.arrayName.path, index, oldVal, array[index]);
                                };
                                break;
                            default:
                                break;
                        }
                    } else throw new NotSupportedException("Non variables are not supported for operators currently");
                } else if (item is RightUnaryOperator rightUnaryOperator) {
                    if (rightUnaryOperator.left is Variable leftVariable)
                        switch (rightUnaryOperator.operatorName) {
                            case TokenTypes.Operator_Increment:
                                res += () => SetVariable(leftVariable.path, (int)GetVariable(leftVariable.path) + 1);
                                break;
                            case TokenTypes.Operator_Decrement:
                                res += () => SetVariable(leftVariable.path, (int)GetVariable(leftVariable.path) - 1);
                                break;
                            default:
                                break;
                        } else throw new NotImplementedException("Non variables are not implemented for operators currently");
                } else if (item is IfStatement ifStatement) {
                    Action trueBody = Compile(ifStatement.TrueBody);
                    if (ifStatement.FalseBody == null) {
                        res += () => {
                            if ((int)ComputeValue(ifStatement.condition) != 0)
                                trueBody();
                        };
                    } else {
                        Action falseBody = Compile(ifStatement.FalseBody);
                        res += () => {
                            if ((int)ComputeValue(ifStatement.condition) != 0)
                                trueBody();
                            else
                                falseBody();
                        };
                    }
                } else if (item is ForLoop forLoop) {
                    Action initialize = Compile(forLoop.initialization);
                    Action incrementation = Compile(forLoop.incrementation);
                    Action body = Compile(forLoop.body);
                    res += () => {
                        for (initialize(); (int)ComputeValue(forLoop.condition) != 0; incrementation())
                            body();
                    };
                }
            }

            return res;
        }

        private object[] ComputeArguments(List<ParseObject> arguments) {
            List<object> res = new List<object>();
            foreach (var item in arguments)
                res.Add(ComputeValue(item));
            return res.ToArray();
        }

        private object ComputeValue(ParseObject item) {
            object res;
            if (item is Integer integer)
                res = integer.value;
            else if (item is Variable variable)
                res = GetVariable(variable.path);
            else if (item is ArrayIndexedObject arrayIndexedObject)
                res = ((object[])GetVariable(arrayIndexedObject.arrayName.path))[(int)ComputeValue(arrayIndexedObject.arrayIndice)];
            else if (item is FunctionCall functionCall) {
                res = bindingDictionary[functionCall.functionName](ComputeArguments(functionCall.arguments));
            } else if (item is BinaryOperator binaryOperator) {
                switch (binaryOperator.operatorName) {
                    case TokenTypes.Operator_Add:
                        res = (int)ComputeValue(binaryOperator.left) + (int)ComputeValue(binaryOperator.right);
                        break;
                    case TokenTypes.Operator_Subtract:
                        res = (int)ComputeValue(binaryOperator.left) - (int)ComputeValue(binaryOperator.right);
                        break;
                    case TokenTypes.Operator_Multiply:
                        res = (int)ComputeValue(binaryOperator.left) * (int)ComputeValue(binaryOperator.right);
                        break;
                    case TokenTypes.Operator_Divide:
                        res = (int)ComputeValue(binaryOperator.left) / (int)ComputeValue(binaryOperator.right);
                        break;
                    case TokenTypes.Operator_Modulus:
                        res = (int)ComputeValue(binaryOperator.left) % (int)ComputeValue(binaryOperator.right);
                        break;
                    case TokenTypes.Operator_Equal:
                        res = ComputeValue(binaryOperator.left).Equals(ComputeValue(binaryOperator.right)) ? 1 : 0;
                        break;
                    case TokenTypes.Operator_NotEqual:
                        res = ComputeValue(binaryOperator.left).Equals(ComputeValue(binaryOperator.right)) ? 0 : 1;
                        break;
                    case TokenTypes.Operator_GreaterThan:
                        res = ((int)ComputeValue(binaryOperator.left) > (int)ComputeValue(binaryOperator.right)) ? 1 : 0;
                        break;
                    case TokenTypes.Operator_LessThan:
                        res = ((int)ComputeValue(binaryOperator.left) < (int)ComputeValue(binaryOperator.right)) ? 1 : 0;
                        break;
                    case TokenTypes.Operator_GreaterThanOrEqual:
                        res = ((int)ComputeValue(binaryOperator.left) >= (int)ComputeValue(binaryOperator.right)) ? 1 : 0;
                        break;
                    case TokenTypes.Operator_LessThanOrEqual:
                        res = ((int)ComputeValue(binaryOperator.left) <= (int)ComputeValue(binaryOperator.right)) ? 1 : 0;
                        break;
                    case TokenTypes.Operator_And:
                        res = (((int)ComputeValue(binaryOperator.left)) == 1 && ((int)ComputeValue(binaryOperator.right)) == 1) ? 1 : 0;
                        break;
                    case TokenTypes.Operator_Or:
                        res = (((int)ComputeValue(binaryOperator.left)) == 1 || ((int)ComputeValue(binaryOperator.right)) == 1) ? 1 : 0;
                        break;
                    default: throw new NotImplementedException("Not implemented " + binaryOperator.operatorName);
                }
            } else if (item is WUIActionParser.String str)
                res = str.value;
            else if (item is Program program) {
                res = Compile(program);
            } else if (item is ArrayConstructor arrayConstructor) {
                return new object[arrayConstructor.arrayLength];
            } else
                throw new NotImplementedException("This item is not implemented!");
            return res;
        }

        public void Bind(string functionName, FunctionDelegate function) {
            bindingDictionary[functionName] = function;
        }

        public void SetVariable(string[] path, object value) {
            //TODO: Better way to implement user defined functions.
            if (path.Length == 1 && value is Action action) {

                Bind(path[0], (args) => {
                    object _returnValueOfFunction = returnValueOfFunction;
                    object _args = GetVariable(argsPath);
                    object _this = GetVariable(thisPath);
                    object _func = GetVariable(funcPath);

                    OnUserDefinedFunctionCall?.Invoke(path[0], args);

                    SetVariable(argsPath, new Dictionary<string, object>() { { "array", args.Clone() } });
                    SetVariable(thisPath, null);
                    SetVariable(funcPath, new Dictionary<string, object>());
                    action();

                    OnUserDefinedFunctionCallEnd?.Invoke(path[0], args);

                    object res = returnValueOfFunction;
                    returnValueOfFunction = _returnValueOfFunction;
                    SetVariable(argsPath, _args);
                    SetVariable(thisPath, _this);
                    SetVariable(funcPath, _func);
                    return res;
                });
            }

            if (value != null && !variableReferenceToPath.ContainsKey(value))
                variableReferenceToPath[value] = path;
            OnVariableSet?.Invoke(path, VariableExists(path) ? GetVariable(path) : null, value);
            switch (path.Length) {
                case 1:
                    variables[path[0]] = value;
                    break;
                case 2:
                    ((Dictionary<string, object>)variables[path[0]])[path[1]] = value;
                    break;
                default:
                    Dictionary<string, object> cur = variables;
                    for (int i = 0; i < path.Length - 1; i++) {
                        cur = (Dictionary<string, object>)cur[path[i]];
                    }
                    cur[path[path.Length - 1]] = value;
                    break;
            }
        }

        public void RemoveVariable(string[] path) {
            switch (path.Length) {
                case 1:
                    variables.Remove(path[0]);
                    break;
                case 2:
                    ((Dictionary<string, object>)variables[path[0]]).Remove(path[1]);
                    break;
                default: throw new NotSupportedException("Nesting of variables is not supported for more than 2.");
            }
        }

        //TODO: Investigate random errors here.
        public object GetVariable(string[] path) {
            switch (path.Length) {
                case 1: return variables[path[0]];
                case 2: return ((Dictionary<string, object>)variables[path[0]])[path[1]];
                default:
                    Dictionary<string, object> cur = variables;
                    for (int i = 0; i < path.Length - 1; i++) {
                        cur = (Dictionary<string, object>)cur[path[i]];
                    }
                    return cur[path[path.Length - 1]];
            }
        }

        public string[] GetPathFromReference(object reference) {
            return variableReferenceToPath[reference];
        }

        public bool VariableExists(string[] path) {
            try {
                GetVariable(path);
            } catch {
                return false;
            }
            return true;
        }
    }
}