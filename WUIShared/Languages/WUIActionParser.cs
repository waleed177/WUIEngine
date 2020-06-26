using System;
using System.Collections.Generic;
using System.Text;
using static WUIShared.Languages.WUIActionTokenizer;

namespace WUIShared.Languages
{
    public class WUIActionParser
    {
        WUIActionTokenizer tokenizer;
        Token token = new Token();

        public WUIActionParser() {
            tokenizer = new WUIActionTokenizer();
        }

        public void LoadCode(string code) {
            tokenizer.LoadCode(code);
        }

        public Program ParseCode() {
            Program res = new Program();

            while ((token = tokenizer.NextToken()).type != TokenTypes.EOF) {
                ParseStatement(res);
            }

            return res;
        }

        private void ParseStatement(Program res) {
            //Console.WriteLine(token.type + " : " + token.value + " : " + tokenizer.TabIndex);

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
                        ifStatement.TrueBody = new Program();
                        Token possiblyColon = tokenizer.PeekToken();
                        if(possiblyColon.type == TokenTypes.Punctuation && (char) possiblyColon.value == ':') {
                            tokenizer.NextToken(); //Dump the colon.

                            while (!((token = tokenizer.NextToken()).type == TokenTypes.Punctuation && (char) token.value == '\n') && token.type != TokenTypes.EOF)
                                ParseStatement(ifStatement.TrueBody);
                        } else {
                            token = tokenizer.NextToken();
                            ParseStatement(ifStatement.TrueBody);
                        }

                        res.body.Add(ifStatement);
                    }
                    break;
                case TokenTypes.Identifier: {
                        Token peek = tokenizer.PeekToken();
                        if(peek.type == TokenTypes.Operator) {
                            res.body.Add(ReadValue(token));
                        }else {
                            //Function
                            FunctionCall functionCall = new FunctionCall() {
                                functionName = (string)token.value
                            };

                            while ((token = tokenizer.NextToken()).type != TokenTypes.Punctuation && token.type != TokenTypes.EOF)
                                functionCall.arguments.Add(ReadValue(token));
                            res.body.Add(functionCall);
                        }
                    }
                    break;
                case TokenTypes.Operator:
                    break;
                case TokenTypes.EOF:
                    break;
                default:
                    break;
            }
        }

        private ParseObject ReadValue(Token token) {
            ParseObject res = null;
            switch (token.type) {
                case TokenTypes.Number:
                    res = new Integer() { value = (int) token.value };
                    break;
                case TokenTypes.String:
                    res = new String() { value = (string) token.value };
                    break;
                case TokenTypes.Identifier:
                    Token peek = tokenizer.PeekToken();
                    if(peek.type == TokenTypes.Operator && (string) peek.value == "@") {
                        tokenizer.NextToken();
                        string varName = (string) tokenizer.NextToken().value;
                        res = new Variable() { path = new string[] { (string) token.value, varName } };
                    } else throw new Exception("Identifier doesnt make sense");
                    break;
                default:
                    break;
            }

            if(tokenizer.PeekToken().type == TokenTypes.Operator) {
                Token op = tokenizer.NextToken();
                string oper = (string)op.value;
                if(oper == "++" || oper == "--") {
                    res = new RightUnaryOperator() { operatorName = oper, left = res };
                } else {
                    res = new BinaryOperator() { operatorName = (string) op.value, left = res, right = ReadValue(tokenizer.NextToken()) };
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
    }
}
