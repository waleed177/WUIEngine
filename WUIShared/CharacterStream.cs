using System;
using System.Collections.Generic;
using System.Text;

namespace WUIShared {
    public class CharacterStream {
        private string str;
        private int position;

        public CharacterStream(string str) {
            this.str = str;
            position = 0;
        }

        public char Peek() {
            if (position < str.Length)
                return str[position];
            else
                return (char)0;
        }

        public bool PeekEquals(string str) {
            for (int i = 0; i < str.Length; i++)
                if (position + i >= this.str.Length || str[i] != this.str[position + i])
                    return false;
            return true;
        }

        public string PeekWord(char chr = ' ') {
            int pos = position;
            while (pos < str.Length && str[pos] != chr) pos++;
            return str.Substring(position, pos - position);
        }

        public string NextWord(char chr = ' ') {
            int start = position;
            while (position < str.Length && str[position] != chr) position++;
            return str.Substring(start, position - start);
        }

        public void DumpUntil(char chr) {
            int start = position;
            while (position < str.Length && str[position] != chr) position++;
        }

        public string PeekOnly(Func<char, bool> filter) {
            int pos = position;
            while (pos < str.Length && filter(str[pos])) pos++;
            return str.Substring(position, pos - position);
        }

        public void Previous(int amount = 1) {
            position -= amount;
        }

        public string NextOnly(Func<char, bool> filter) {
            int start = position;
            while (position < str.Length && filter(str[position])) position++;
            return str.Substring(start, position - start);
        }

        public char Next() {
            if (position < str.Length)
                return str[position++];
            else
                return (char)0;
        }

        public bool EOF() {
            return Peek() == 0;
        }

        public string Peek(int amount) {
            if (position + amount < str.Length)
                return str.Substring(position, amount);
            else if (position + 1 < str.Length)
                return str.Substring(position, str.Length - position);
            else return "";
        }

        public string Next(int amount) {
            string res = Peek(amount);
            position += res.Length;
            return res;
        }

        public void Dump(int amt) {
            position += amt;
        }
    }
}
