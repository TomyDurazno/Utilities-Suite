using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Utility.Core.Tokens
{
    //Dynamic Token Configuration
    //Potentially can be read from a config file
    public static class TokenConfigs
    {
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

        public static char Separator { get { return Convert.ToChar(GetElement("Separator")); } }

        public static string Joiner { get { return GetElement("Joiner"); } }

        public static string Pipe { get { return GetElement("Pipe"); ; } }

        public static string Seq { get { return GetElement("Seq"); } }

        public static string TextStart { get { return GetElement("TextStart"); } }

        public static string TextEnd { get { return GetElement("TextEnd"); } }

        public static string VarNameStart { get { return GetElement("VarNameStart"); } }
    }
}
