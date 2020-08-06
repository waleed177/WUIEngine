using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WUIShared.Languages {
    public class WUIActionTokenizer {
        public struct Token {
            public TokenTypes type;
            public object value;

            public int row, column;

            public string Position() {
                return "Row: " + row + ", " + column;
            }
        }

        public int TabIndex { get; private set; }
        public bool StartOfLine { get; private set; }

        private string code;
        private CharacterStream stream;
        private Queue<Token> peekedWords;
        private Dictionary<char, TokenTypes> punctuationDictionary;
        private Dictionary<string, TokenTypes> operatorDictionary;

        public enum TokenTypes {
            __PUNCTUATIONSTART,
            Punctuation_Comma,
            Punctuation_Semicolon,
            Punctuation_Colon,
            Punctuation_Open_Parenthesis,
            Punctuation_Close_Parenthesis,
            Punctuation_Open_Square,
            Punctuation_Close_Square,
            Punctuation_Open_Squiggly,
            Punctuation_Close_Squiggly,
            Punctuation_NewLine,
            __PUNCTUATIONEND,

            Number,
            String,
            Keyword,
            Identifier,

            __OPERATORSTART,
            Operator_Add,
            Operator_Subtract,
            Operator_Increment,
            Operator_Decrement,
            Operator_Divide,
            Operator_Multiply,
            Operator_AddEqual,
            Operator_SubtractEqual,
            Operator_DivideEqual,
            Operator_MultiplyEqual,
            Operator_GreaterThan,
            Operator_LessThan,
            Operator_GreaterThanOrEqual,
            Operator_LessThanOrEqual,
            Operator_Equal,
            Operator_Assign,
            Operator_And,
            Operator_Not,
            Operator_Or,
            Operator_NotEqual,
            Operator_Modulus,
            Operator_Access,
            __OPERATOREND,

            EOF,
        }

        public WUIActionTokenizer() {
            peekedWords = new Queue<Token>();
            punctuationDictionary = new Dictionary<char, TokenTypes>() {
                [','] = TokenTypes.Punctuation_Comma,
                [';'] = TokenTypes.Punctuation_Semicolon,
                ['('] = TokenTypes.Punctuation_Open_Parenthesis,
                [')'] = TokenTypes.Punctuation_Close_Parenthesis,
                ['{'] = TokenTypes.Punctuation_Open_Squiggly,
                ['}'] = TokenTypes.Punctuation_Close_Squiggly,
                ['['] = TokenTypes.Punctuation_Open_Square,
                [']'] = TokenTypes.Punctuation_Close_Square,
                [':'] = TokenTypes.Punctuation_Colon,
                ['\n'] = TokenTypes.Punctuation_NewLine
            };
            operatorDictionary = new Dictionary<string, TokenTypes>() {
                ["+"] = TokenTypes.Operator_Add,
                ["+="] = TokenTypes.Operator_AddEqual,
                ["++"] = TokenTypes.Operator_Increment,

                ["-"] = TokenTypes.Operator_Subtract,
                ["-="] = TokenTypes.Operator_SubtractEqual,
                ["--"] = TokenTypes.Operator_Decrement,

                ["/"] = TokenTypes.Operator_Divide,
                ["/="] = TokenTypes.Operator_DivideEqual,

                ["*"] = TokenTypes.Operator_Multiply,
                ["*="] = TokenTypes.Operator_MultiplyEqual,

                ["="] = TokenTypes.Operator_Assign,
                ["=="] = TokenTypes.Operator_Equal,
                
                ["<"] = TokenTypes.Operator_LessThan,
                ["<="] = TokenTypes.Operator_LessThanOrEqual,
                [">"] = TokenTypes.Operator_GreaterThan,
                [">="] = TokenTypes.Operator_GreaterThanOrEqual,

                ["&&"] = TokenTypes.Operator_And,
                ["||"] = TokenTypes.Operator_Or,

                ["!"] = TokenTypes.Operator_Not,
                ["!="] = TokenTypes.Operator_NotEqual,
                ["%"] = TokenTypes.Operator_Modulus,

                ["."] = TokenTypes.Operator_Access,
            };
        }

        public void LoadCode(string code) {
            this.code = code;
            stream = new CharacterStream(code);
            peekedWords.Clear();
        }

        public Token NextToken() {
            if (peekedWords.Count > 0)
                return peekedWords.Dequeue();

            Token res = new Token();

            char peekChar = stream.Peek();

            while (stream.PeekEquals("//")) {
                stream.DumpUntil('\n');
                stream.Next();
                peekChar = stream.Peek();
            }

            while (peekChar == ' ' || peekChar == '\n' || peekChar == '\r' || peekChar == '\t') {
                if (peekChar == '\n') {
                    TabIndex = 0;
                    StartOfLine = true;
                } else if (peekChar == '\t' && StartOfLine)
                    TabIndex++;
                stream.Next();
                peekChar = stream.Peek();
            }

            res.row = stream.Row;
            res.column = stream.Column;

            if (StartOfLine) {
                res.type = TokenTypes.Punctuation_NewLine;
                res.value = null;
                StartOfLine = false;
            } else if (peekChar == 0) {
                res.type = TokenTypes.EOF;
                res.value = null;
            } else if (IsPunctuation(peekChar)) {
                res.type = punctuationDictionary[stream.Next()];
                res.value = null;
            } else if (char.IsDigit(peekChar)) {
                res.type = TokenTypes.Number;
                res.value = ReadNumber();
            } else if (peekChar == '"') {
                res.type = TokenTypes.String;
                res.value = ReadString();
            } else if (IsOperatorStart(peekChar)) {
                res.type = operatorDictionary[ReadOperator()];
                res.value = null;
            } else if (IsKeyword(stream.PeekOnly(IsKeywordCharacters))) {
                res.type = TokenTypes.Keyword;
                res.value = stream.NextOnly(IsKeywordCharacters);
            } else if (IsIdentifierStart(peekChar)) {
                res.type = TokenTypes.Identifier;
                res.value = stream.NextOnly(IsIdentifier);
            } else {
                throw new Exception("Unknown thing..");
            }

            return res;
        }

        public Token PeekToken() {
            if (peekedWords.Count == 0)
                peekedWords.Enqueue(NextToken());
            return peekedWords.Peek();
        }

        private int ReadNumber() {
            string num = "";
            while (char.IsDigit(stream.Peek()))
                num += stream.Next();
            return int.Parse(num);
        }

        private string ReadString() {
            string res = "";
            stream.Next(); //Ignore first ".
            while (stream.Peek() != '"')
                res += stream.Next();
            stream.Next();
            return res;
        }

        private string ReadOperator() {
            string op = stream.Peek(2);
            if (IsOperatorStart(op[1])) {
                stream.Dump(2);
                return op;
            } else {
                stream.Dump(1);
                return op[0].ToString();
            }
        }

        private bool IsPunctuation(char c) => ",;(){}[]:\n".IndexOf(c) >= 0;
        private bool IsOperatorStart(char c) => "+-/*=.<>&!".IndexOf(c) >= 0;
        private bool IsIdentifierStart(char c) => char.IsLetter(c) || c == '_';
        private bool IsIdentifier(char c) => char.IsLetterOrDigit(c) || c == '_';
        private bool IsKeyword(string c) => c == "if" || c == "else" || c == "for" || c == "while" || c == "break" || c == "end" || c == "func" || c == "function" || c == "def" || c == "define" || c == "include" || c == "struct" || c == "class" || c == "netstruct" || c == "netclass";
        private bool IsKeywordCharacters(char c) => char.IsLetter(c);

        public bool IsOperator(Token token) => TokenTypes.__OPERATORSTART < token.type && token.type < TokenTypes.__OPERATOREND;
        public bool IsPunctuation(Token token) => TokenTypes.__PUNCTUATIONSTART < token.type && token.type < TokenTypes.__PUNCTUATIONEND;

    }
}
