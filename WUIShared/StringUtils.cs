using System;
using System.Collections.Generic;
using System.Text;

namespace WUIShared
{
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

        public static int FindFirstNonAlphanumeric(this string str) {
            for (int i = 0; i < str.Length; i++)
                if (!Char.IsLetterOrDigit(str[i]))
                    return i;
            return -1;
        }

    }
}
