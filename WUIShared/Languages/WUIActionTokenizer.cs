using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WUIShared.Languages {
    public class WUIActionTokenizer {
        public struct Token {
            public TokenTypes type;
            public object value;
        }

        public int TabIndex { get; private set; }
        public bool StartOfLine { get; private set; }

        private string code;
        private CharacterStream stream;
        private Queue<Token> peekedWords;

        public enum TokenTypes {
            Punctuation,
            Number,     
            String,     
            Keyword,        
            Identifier,
            Operator, 
            EOF
        }

        public WUIActionTokenizer() {
            peekedWords = new Queue<Token>();
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
                }
                else if(peekChar == '\t' && StartOfLine)
                    TabIndex++;
                stream.Next();
                peekChar = stream.Peek();
            }

            if(StartOfLine) {
                res.type = TokenTypes.Punctuation;
                res.value = '\n';
                StartOfLine = false;
            } else if (peekChar == 0) {
                res.type = TokenTypes.EOF;
                res.value = null;
            } else if(IsPunctuation(peekChar)) {
                res.type = TokenTypes.Punctuation;
                res.value = stream.Next();
            } else if(char.IsDigit(peekChar)) {
                res.type = TokenTypes.Number;
                res.value = ReadNumber();
            } else if(peekChar == '"') {
                res.type = TokenTypes.String;
                res.value = ReadString();
            } else if(IsOperatorStart(peekChar)) {
                res.type = TokenTypes.Operator;
                res.value = ReadOperator();
            } else if(IsKeyword(stream.PeekOnly(IsKeywordCharacters))) {
                res.type = TokenTypes.Keyword;
                res.value = stream.NextOnly(IsKeywordCharacters);
            } else if(IsIdentifierStart(peekChar)) {
                res.type = TokenTypes.Identifier;
                res.value = stream.NextOnly(IsIdentifier);
            } else {
                throw new Exception("Unknown thing..");
            }

            return res;
        }

        public Token PeekToken() {
            if(peekedWords.Count == 0)
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
            }
            else {
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
    }
}
