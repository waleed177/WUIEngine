using System;
using System.Collections.Generic;
using System.Text;

namespace WUIShared.Languages {
    public class WUIGSyntaxParser {
        public enum TokenTypes {
            ObjectName,
            PropertyName,
            PropertyValue,
            InstantiateComponent,
            EOF,
            Unknown
        }
        private string code;
        private string[] lines;
        private string[] words;
        private int currentLine = 0;
        private int currentWord = 0;
        private int tabLevel = 0;
        internal string CurrentObjectName { get; private set; }

        public WUIGSyntaxParser() {

        }

        public WUIGSyntaxParser(string code) {
            LoadCode(code);
        }

        public void LoadCode(string code) {
            this.code = code;
            currentLine = 0;
            currentWord = 0;
            tabLevel = 0;
            lines = code.Split('\n');
            words = lines[0].Trim().Split(' ');
        }

        private void NextWord() {
            currentWord++;
            if (currentWord >= words.Length)
                NextLine();
        }

        private void NextLine() {
            ++currentLine;
            if (currentLine >= lines.Length)
                currentWord = -1;
            else {
                currentWord = 0;
                string line = lines[currentLine];
                words = line.Trim().Split(' ');
                tabLevel = line.CountBegin("\t");
            }
        }

        public string NextToken(out TokenTypes tokenType, bool peek = false) {
            tokenType = TokenTypes.Unknown;
            if (currentWord == -1) {
                tokenType = TokenTypes.EOF;
                return null;
            }

            string word = words[currentWord];
            while(word.Trim() == "") {
                NextWord();
                if(currentWord == -1) {
                    tokenType = TokenTypes.EOF;
                    return null;
                }
                word = words[currentWord];
            }

            if (tabLevel == 0 && word.EndsWith(":")) {
                tokenType = TokenTypes.ObjectName;
                word = word.Remove(word.Length - 1);
                CurrentObjectName = word;
            }else if(tabLevel == 0) {
                CurrentObjectName = null;
                if (currentWord == 0) {
                    tokenType = TokenTypes.PropertyName;
                } else if (currentWord == 1) {
                    tokenType = TokenTypes.PropertyValue;
                    return ReadString(1);
                }
            } else if (tabLevel == 1) {
                if (words.Length == 1)
                    tokenType = TokenTypes.InstantiateComponent;
                else if (currentWord == 0) {
                    tokenType = TokenTypes.PropertyName;
                } else if (currentWord == 1) {
                    tokenType = TokenTypes.PropertyValue;
                    return ReadString(1);
                }
            }

            if (!peek) NextWord();
            return word;
        }

        private string ReadString(int start) {
            string res = "";
            for (int i = start; i < words.Length; i++) res += (i == start ? "" : " ") + words[i];
            NextLine();
            return res;
        }
    }
}
