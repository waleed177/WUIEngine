
using System;
using static WUIShared.Languages.WUIActionParser;

namespace WUIShared.Languages
{
    public class WUIActionLanguage
    {
        private WUIActionParser parser;

        public WUIActionLanguage() {
            parser = new WUIActionParser();
        }

        public void Evaluate(string code) {
            parser.LoadCode(code);
            Program program = parser.ParseCode();


        }
    }
}
