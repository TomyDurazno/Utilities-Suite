using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Utility.Core.Tokens
{
    /*
        Token configs reads the string representation of particular tokens/reserved words
        from 'TokenConfigs.xml'
        
        Its implemented this way so anyone using this program can change or redefine their own reserved words

    */

    public static class TokenConfigs
    {
        #region Private Properties

        private static XElement Configurations
        {
            get
            {
                return XDocument.Parse(Dynamic_Invoker.Properties.Resources.TokenConfigs).Element("configuration");
            }
        }

        private static string GetElement(string elementName)
        {
            return Regex.Replace(Configurations.Element(elementName).Value.ToString().Trim(), "^\"|\"$", "");
        }

        #endregion

        #region Public Properties

        public static char SeparatorChar { get { return Convert.ToChar(GetElement("Separator")); } }

        public static string SeparatorString { get { return GetElement("Separator"); } }

        public static string Pipe { get { return GetElement("Pipe"); ; } }

        public static string Seq { get { return GetElement("Seq"); } }

        public static string TextStart { get { return GetElement("TextStart"); } }

        public static string TextEnd { get { return GetElement("TextEnd"); } }

        public static string VarNameStart { get { return GetElement("VarNameStart"); } }

        #endregion
    }
}
