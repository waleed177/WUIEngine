using System;
using System.Collections.Generic;
using System.Text;
using static WUIShared.Languages.WUIActionTokenizer;

namespace WUIShared.Languages {
    public class WUIActionParser {
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
                case TokenTypes.Punctuation:
                    break;
                case TokenTypes.Number:
                    break;
                case TokenTypes.String:
                    break;
                case TokenTypes.Keyword:
                    if ((string)token.value == "if") {
                        IfStatement ifStatement = new IfStatement();
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
                        ParseStatement(forLoop.initialization = new Program());
                        forLoop.condition = ReadValue(tokenizer.NextToken());
                        ParseStatement(forLoop.incrementation = new Program());
                        ParseStatementBody(forLoop.body = new Program());
                        return forLoop;
                    }
                    break;
                case TokenTypes.Identifier: {
                        Token peek = tokenizer.PeekToken();
                        if (peek.type == TokenTypes.Operator) {
                            return ReadValue(token);
                        } else {
                            //Function
                            FunctionCall functionCall = ReadFunction(token);
                            return functionCall;
                        }
                    }
                case TokenTypes.Operator:
                    break;
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
            if (possiblyColon.type == TokenTypes.Punctuation && (char)possiblyColon.value == ':') {
                int refTabIndex = tokenizer.TabIndex;
                tokenizer.NextToken(); //Dump the colon.
                Token multilineCheckToken = tokenizer.PeekToken();
                if (multilineCheckToken.type == TokenTypes.Punctuation && (char)multilineCheckToken.value == '\n') {
                    //Its a multiline if.
                    tokenizer.NextToken(); //dump the new line.
                    while (tokenizer.TabIndex > refTabIndex && (token = tokenizer.PeekToken()).type != TokenTypes.EOF)
                        ParseStatement(body);
                } else while (!((token = tokenizer.PeekToken()).type == TokenTypes.Punctuation && (char)token.value == '\n') && token.type != TokenTypes.EOF)
                        ParseStatement(body);
            } else {
                ParseStatement(body);
            }
        }

        private FunctionCall ReadFunction(Token token) {
            FunctionCall functionCall = new FunctionCall() {
                functionName = (string)token.value
            };

            while ((token = tokenizer.NextToken()).type != TokenTypes.Punctuation && token.type != TokenTypes.EOF)
                functionCall.arguments.Add(ReadValue(token));
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
                    if (peek.type == TokenTypes.Operator && (string)peek.value == ".") {
                        List<string> path = new List<string>();
                        path.Add(token.value.ToString());
                        do {
                            tokenizer.NextToken(); //Dump the .
                            string varName = (string)tokenizer.NextToken().value;
                            path.Add(varName);
                            peek = tokenizer.PeekToken();
                        } while (peek.type == TokenTypes.Operator && (string)peek.value == ".");

                        res = new Variable() { path = path.ToArray() };

                    } else if (peek.type == TokenTypes.Operator) {
                        res = new Variable() { path = new string[] { token.value.ToString() } };
                    } else
                        res = ReadFunction(token); //token is the function name
                    break;
                case TokenTypes.Punctuation:
                    if ((char)token.value == '{') {
                        Program func = new Program();

                        Token funcToken;
                        while ((funcToken = tokenizer.PeekToken()).type != TokenTypes.EOF && !(funcToken.type == TokenTypes.Punctuation && (char)funcToken.value == '}'))
                            ParseStatement(func);
                        return func;
                    }
                    break;
                default:
                    break;
            }

            if (tokenizer.PeekToken().type == TokenTypes.Operator) {
                Token op = tokenizer.NextToken();
                string oper = (string)op.value;
                if (oper == "++" || oper == "--") {
                    res = new RightUnaryOperator() { operatorName = oper, left = res };
                } else {
                    res = new BinaryOperator() { operatorName = (string)op.value, left = res, right = ReadValue(tokenizer.NextToken()) };
                }
            }
            return res;
        }

        ////////////////////////////////Classes////////////////////////
        public class ParseObject {

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
            public string operatorName;
            public ParseObject left, right;
        }

        public class RightUnaryOperator : ParseObject {
            public string operatorName;
            public ParseObject left;
        }

        public class LeftUnaryOperator : ParseObject {
            public string operatorName;
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
    }
}

//for v.i = 0; v.i < 10; v.i++: