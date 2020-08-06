using System;
using System.Collections.Generic;
using System.Text;
using static WUIShared.Languages.WUIActionTokenizer;

namespace WUIShared.Languages {
    public class WUIActionParser {
        private readonly string[] argsArrayPath = new string[] { "args", "array" };
        WUIActionTokenizer tokenizer;

        public WUIActionParser() {
            tokenizer = new WUIActionTokenizer();
        }


        public void LoadCode(string code) {
            tokenizer.LoadCode(code);
        }

        public Program ParseCode() {
            Program res = new Program();

            Token token;
            while ((token = tokenizer.PeekToken()).type != TokenTypes.EOF) {
                ParseStatement(res);
            }

            return res;
        }

        private void ParseStatement(Program res) {
            ParseObject parseObject = ParseStatement();
            if (parseObject != null)
                res.body.Add(parseObject);
        }


        private ParseObject ParseStatement() {
            //Console.WriteLine(token.type + " : " + token.value + " : " + tokenizer.TabIndex);
            Token token = tokenizer.NextToken();

            switch (token.type) {
                case TokenTypes.Number:
                    break;
                case TokenTypes.String:
                    break;
                case TokenTypes.Keyword:
                    if ((string)token.value == "if") {
                        IfStatement ifStatement = new IfStatement();
                        ifStatement.column = token.column;
                        ifStatement.row = token.row;
                        ifStatement.condition = ReadValue(tokenizer.NextToken());
                        ParseStatementBody(ifStatement.TrueBody = new Program());
                        Token possibleElseToken = tokenizer.PeekToken();
                        if (possibleElseToken.type == TokenTypes.Keyword && (string)possibleElseToken.value == "else") {
                            tokenizer.NextToken(); //dump the else token.
                            ParseStatementBody(ifStatement.FalseBody = new Program());
                        }

                        return ifStatement;
                    } else if ((string)token.value == "for") {
                        ForLoop forLoop = new ForLoop();
                        forLoop.column = token.column;
                        forLoop.row = token.row;
                        ParseStatement(forLoop.initialization = new Program());
                        forLoop.condition = ReadValue(tokenizer.NextToken());
                        ParseStatement(forLoop.incrementation = new Program());
                        ParseStatementBody(forLoop.body = new Program());
                        return forLoop;
                    }
                    break;
                case TokenTypes.Identifier: {
                        Token peek = tokenizer.PeekToken();
                        if (peek.type == TokenTypes.Punctuation_Open_Parenthesis) {
                            return ReadFunction(token);
                        } else {
                            return ReadValue(token);
                        }
                    }
                case TokenTypes.EOF:
                    break;
                default:
                    break;
            }
            return null;
        }
        /*
         * This is for statements that need single or multistatement bodies or even multiline bodies.
         * works like this:
         * If there is a colon then its either multiline or multistatement.
         * If there is no colon, then its single statement.
         * The multistatement one ends at the end of the line.
         * The multiline ends when the tab level decreases by atleast one from the tab level of the colon.
         */
        private void ParseStatementBody(Program body) {
            Token token;
            Token possiblyColon = tokenizer.PeekToken();
            if (possiblyColon.type == TokenTypes.Punctuation_Colon) {
                int refTabIndex = tokenizer.TabIndex;
                tokenizer.NextToken(); //Dump the colon.
                Token multilineCheckToken = tokenizer.PeekToken();
                if (multilineCheckToken.type == TokenTypes.Punctuation_NewLine) {
                    //Its a multiline if.
                    tokenizer.NextToken(); //dump the new line.
                    while (tokenizer.TabIndex > refTabIndex && (token = tokenizer.PeekToken()).type != TokenTypes.EOF)
                        ParseStatement(body);
                } else while (!((token = tokenizer.PeekToken()).type == TokenTypes.Punctuation_NewLine) && token.type != TokenTypes.EOF)
                        ParseStatement(body);
            } else {
                ParseStatement(body);
            }
        }

        private FunctionCall ReadFunction(Token token) {
            FunctionCall functionCall = new FunctionCall() {
                functionName = (string)token.value,
                column = token.column,
                row = token.row
            };

            tokenizer.NextToken(); //Dump the (;

            while ((token = tokenizer.NextToken()).type != TokenTypes.Punctuation_Close_Parenthesis && token.type != TokenTypes.EOF) {
                functionCall.arguments.Add(ReadValue(token));
                token = tokenizer.PeekToken();
                if (token.type == TokenTypes.Punctuation_Comma) {
                    tokenizer.NextToken();
                    continue; //TODO: Possibly make commas non-optional
                }
            }
            return functionCall;
        }

        private ParseObject ReadValue(Token token) {
            ParseObject res = null;
            switch (token.type) {
                case TokenTypes.Number:
                    res = new Integer() { value = (int)token.value };
                    break;
                case TokenTypes.String:
                    res = new String() { value = (string)token.value };
                    break;
                case TokenTypes.Identifier:

                    Token peek = tokenizer.PeekToken();
                    if (peek.type == TokenTypes.Operator_Access) {
                        List<string> path = new List<string>();
                        path.Add(token.value.ToString());
                        do {
                            tokenizer.NextToken(); //Dump the .
                            string varName = (string)tokenizer.NextToken().value;
                            path.Add(varName);
                            peek = tokenizer.PeekToken();
                        } while (peek.type == TokenTypes.Operator_Access);


                        if (peek.type == TokenTypes.Punctuation_Open_Square) {
                            tokenizer.NextToken(); //dump the [

                            res = new ArrayIndexedObject() {
                                arrayName = new Variable() { path = path.ToArray(), row = token.row, column = token.column },
                                arrayIndice = ReadValue(tokenizer.NextToken())
                            };

                            Token potentialClosingBracket = tokenizer.NextToken(); //]
                            if (potentialClosingBracket.type != TokenTypes.Punctuation_Close_Square)
                                throw new Exception("Expecting ] at " + token.Position() + " instead got " + token.type);
                        } else {
                            res = new Variable() { path = path.ToArray() };
                        }

                    } else if (peek.type == TokenTypes.Punctuation_Open_Parenthesis) {
                        res = ReadFunction(token); //token is the function name
                    } else
                        res = new Variable() { path = new string[] { token.value.ToString() } };
                    break;
                case TokenTypes.Punctuation_Open_Squiggly: {
                        Program func = new Program();

                        Token funcToken;
                        while ((funcToken = tokenizer.PeekToken()).type != TokenTypes.EOF && funcToken.type != TokenTypes.Punctuation_Close_Squiggly)
                            ParseStatement(func);
                        res = func;
                    }
                    break;
                case TokenTypes.Punctuation_Open_Parenthesis: {
                        //Named parameters function.
                        Program func = new Program();

                        //TODO: Check if the other method is more optimum (making a new parse object for named parameters and doing the rest of the work in wuiactionlanguage).
                        int argNum = 0;
                        Token funcToken;
                        while ((funcToken = tokenizer.PeekToken()).type != TokenTypes.EOF && funcToken.type != TokenTypes.Punctuation_Close_Parenthesis) {
                            tokenizer.NextToken();
                            if (funcToken.type == TokenTypes.Identifier) {
                                //Generate code for the parameter list.
                                //f.[ParamName] = args.array[argNum];
                                func.body.Add(new BinaryOperator() {
                                    left = new Variable() {
                                        path = new string[] { "f", (string)funcToken.value }
                                    },
                                    operatorName = TokenTypes.Operator_Assign,
                                    right = new ArrayIndexedObject() {
                                        arrayName = new Variable() {
                                            path = argsArrayPath
                                        },
                                        arrayIndice = new Integer() {
                                            value = argNum++
                                        }
                                    }
                                });
                            } else {
                                throw new Exception("Expected identifier at " + funcToken.Position() + " instead got " + funcToken.type);
                            }
                        }

                        tokenizer.NextToken(); //Dump the )

                        funcToken = tokenizer.NextToken();

                        if (funcToken.type != TokenTypes.Punctuation_Open_Squiggly) {
                            throw new Exception("Expected { at " + funcToken.Position() + " got " + funcToken.type);
                        }
                        tokenizer.NextToken(); //dump the {.

                        //Read the function.
                        while ((funcToken = tokenizer.PeekToken()).type != TokenTypes.EOF && !(funcToken.type == TokenTypes.Punctuation_Close_Squiggly))
                            ParseStatement(func);
                        res = func;
                    }
                    break;
                case TokenTypes.Punctuation_Open_Square: {
                        Token length = tokenizer.NextToken();
                        if (length.type != TokenTypes.Number)
                            throw new Exception("Expected a number got " + length.type);
                        Token closingBrackets = tokenizer.NextToken();
                        if (closingBrackets.type == TokenTypes.Punctuation_Close_Square)
                            res = new ArrayConstructor { arrayLength = (int)length.value };
                        else
                            throw new Exception("Expecting ] at " + closingBrackets.Position() + " instead got " + closingBrackets.type);
                    }
                    break;
                default:
                    break;
            }

            if (tokenizer.IsOperator(tokenizer.PeekToken())) {
                Token op = tokenizer.NextToken();
                if (op.type == TokenTypes.Operator_Increment || op.type == TokenTypes.Operator_Decrement) {
                    res = new RightUnaryOperator() { operatorName = op.type, left = res };
                } else {
                    res = new BinaryOperator() { operatorName = op.type, left = res, right = ReadValue(tokenizer.NextToken()) };
                }
            }

            res.column = token.column;
            res.row = token.row;

            return res;
        }

        ////////////////////////////////Classes////////////////////////
        public class ParseObject {
            public int row, column;
        }

        public class Program : ParseObject {
            public Program() {
                body = new List<ParseObject>();
            }
            public List<ParseObject> body;
        }

        public class Variable : ParseObject {
            public string[] path;
        }

        public class Integer : ParseObject {
            public int value;
        }

        public class String : ParseObject {
            public string value;
        }

        public class BinaryOperator : ParseObject {
            public TokenTypes operatorName;
            public ParseObject left, right;
        }

        public class RightUnaryOperator : ParseObject {
            public TokenTypes operatorName;
            public ParseObject left;
        }

        public class LeftUnaryOperator : ParseObject {
            public TokenTypes operatorName;
            public ParseObject right;
        }

        public class FunctionCall : ParseObject {
            public FunctionCall() {
                arguments = new List<ParseObject>();
            }
            public string functionName;
            public List<ParseObject> arguments;
        }

        public class IfStatement : ParseObject {
            public ParseObject condition;
            public Program TrueBody, FalseBody;
        }

        public class ForLoop : ParseObject {
            public Program initialization;
            public ParseObject condition;
            public Program incrementation;
            public Program body;
        }

        public class ArrayConstructor : ParseObject {
            public int arrayLength;
        }

        public class ArrayIndexedObject : ParseObject {
            public Variable arrayName;
            public ParseObject arrayIndice;
        }
    }
}

//for v.i = 0; v.i < 10; v.i++: