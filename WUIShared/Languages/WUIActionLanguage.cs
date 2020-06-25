
using System;
using System.Collections.Generic;
using static WUIShared.Languages.WUIActionParser;

namespace WUIShared.Languages {
    public class WUIActionLanguage {
        private WUIActionParser parser;
        public delegate object FunctionDelegate(object[] args);
        private Dictionary<string, FunctionDelegate> bindingDictionary;
        private Dictionary<string, object> variables;

        Program parsedProgram;

        public WUIActionLanguage() {
            parser = new WUIActionParser();
            bindingDictionary = new Dictionary<string, FunctionDelegate>();
            variables = new Dictionary<string, object>();
        }

        public void LoadCode(string code) {
            parser.LoadCode(code);
            parsedProgram = parser.ParseCode();

        }

        public Action Compile() {
            return Compile(parsedProgram);
        }

        private Action Compile(Program prog) {
            Action res = () => { };

            foreach (var item in prog.body) {
                if (item is FunctionCall functionCall) {
                    FunctionDelegate func = bindingDictionary[functionCall.functionName];
                    res += () => func(ComputeArguments(functionCall.arguments));
                } else if (item is BinaryOperator binaryOperator) {
                    if (binaryOperator.left is Variable leftVariable)
                        switch (binaryOperator.operatorName) {
                            case "=":
                                res += () => variables[leftVariable.name] = ComputeValue(binaryOperator.right);
                                break;
                            default:
                                break;
                        } else throw new NotSupportedException("Non variables are not supported for operators currently");
                } else if (item is RightUnaryOperator rightUnaryOperator) {
                    if (rightUnaryOperator.left is Variable leftVariable)
                        switch (rightUnaryOperator.operatorName) {
                            case "++":
                                res += () => variables[leftVariable.name] = (int)variables[leftVariable.name] + 1;
                                break;
                            case "--":
                                res += () => variables[leftVariable.name] = (int)variables[leftVariable.name] - 1;
                                break;
                            default:
                                break;
                        } else throw new NotImplementedException("Non variables are not implemented for operators currently");
                } else if (item is IfStatement ifStatement) {
                    Action trueBody = Compile(ifStatement.TrueBody);
                    res += () => {
                        if ((int)ComputeValue(ifStatement.condition) != 0)
                            trueBody();
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
                res = variables[variable.name];
            else if(item is BinaryOperator binaryOperator) {
                switch (binaryOperator.operatorName) {
                    case "+":
                        res = (int)ComputeValue(binaryOperator.left) + (int)ComputeValue(binaryOperator.right);
                        break;
                    case "-":
                        res = (int)ComputeValue(binaryOperator.left) - (int)ComputeValue(binaryOperator.right);
                        break;
                    case "*":
                        res = (int)ComputeValue(binaryOperator.left) * (int)ComputeValue(binaryOperator.right);
                        break;
                    case "/":
                        res = (int)ComputeValue(binaryOperator.left) / (int)ComputeValue(binaryOperator.right);
                        break;
                    case "==":
                        res = ((int)ComputeValue(binaryOperator.left) == (int)ComputeValue(binaryOperator.right)) ? 1 : 0;
                        break;
                    default: throw new NotImplementedException("Not implemented " + binaryOperator.operatorName);
                }
            }
            else if (item is WUIActionParser.String str)
                res = str.value;
            else
                throw new NotImplementedException("This item is not implemented!");
            return res;
        }

        public void Bind(string functionName, FunctionDelegate function) {
            bindingDictionary[functionName] = function;
        }
    }
}
