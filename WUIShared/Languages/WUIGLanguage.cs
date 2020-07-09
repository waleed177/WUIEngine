using System;
using System.Collections.Generic;
using System.Text;
using static WUIShared.Languages.WUIGSyntaxParser;

namespace WUIShared.Languages
{
    public class WUIGLanguage
    {
        public delegate void PropertySetDelegate(string objectName, string propertyName, string propertyValue);
        public delegate void ComponentAddDelegate(string objectName, string componentName);
        public delegate void CreateObjectDelegate(string objectName);
        public delegate void EndOfObject(string objectName);

        private WUIGSyntaxParser parser;
        public event CreateObjectDelegate CreateObjectBinder;
        public event PropertySetDelegate SetPropertyBinder;
        public event ComponentAddDelegate ComponentAddBinder;
        public event EndOfObject EndOfObjectBinder;


        public WUIGLanguage() {
            parser = new WUIGSyntaxParser();
        }

        public void Evaluate(string code) {
            parser.LoadCode(code);

            string token = "";
            TokenTypes tokenType = TokenTypes.Unknown;
            string prevObjectName = null;
            while((token = parser.NextToken(out tokenType)) != null) {
                switch (tokenType) {
                    case TokenTypes.ObjectName:
                        if(prevObjectName != null)
                            EndOfObjectBinder(prevObjectName);
                        prevObjectName = token;
                        CreateObjectBinder(token);
                        break;
                    case TokenTypes.PropertyName:
                        SetPropertyBinder(parser.CurrentObjectName, token, parser.NextToken(out tokenType));
                        break;
                    case TokenTypes.InstantiateComponent:
                        ComponentAddBinder(parser.CurrentObjectName, token);
                        break;
                    case TokenTypes.Unknown:
                        
                        break;
                    default:
                        break;
                }
            }

            EndOfObjectBinder(parser.CurrentObjectName);

        }
    }
}
