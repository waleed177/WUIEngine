using System;
using System.Collections.Generic;
using System.Text;

namespace WUIShared {
    public static class StringUtils {

        public static int CountBegin(this string str, string begin) {
            int res = 0;
            if (begin.Length == 0)
                return 0;
            while (str.StartsWith(begin)) {
                res++;
                str = str.Remove(0, begin.Length);
            }
            return res;
        }

        public static int FindFirstNonAlphanumeric(this string str, int startId = 0, int endId = -1) {
            int end = endId >= 0 ? endId : str.Length;
            for (int i = startId; i < end; i++)
                if (!Char.IsLetterOrDigit(str[i]))
                    return i;
            return -1;
        }

        public static bool CharsSatisfy(this string str, Func<char, bool> f) {
            for (int i = 0; i < str.Length; i++)
                if (!f(str[i]))
                    return false;
            return true;
        }

        public static List<string> SplitUsingFunction(this string str, Func<char, bool> splitter, Func<char, bool> lonelys) {
            List<string> res = new List<string>();
            string token = "";
            bool splitterNow = splitter(str[0]);

            //waw+wow
            for (int i = 0; i < str.Length; i++) {
                char c = str[i];
                if (lonelys(c)) {
                    if (token != "") {
                        res.Add(token);
                        token = "";
                    }
                    res.Add(c.ToString());
                    continue;
                }
                if (splitterNow != splitter(c)) {
                    res.Add(token);
                    token = c.ToString();
                    splitterNow = splitter(c);
                } else {
                    token += c;
                }
            }
            if (token != "") {
                res.Add(token);
            }
            return res;
        }

        public static string Repeat(this string str, int num) {
            string res = "";
            for (int i = 0; i < num; i++)
                res += str;
            return res;
        }
    }
}
