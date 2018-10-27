using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Core.Tokens
{
    //Dynamic Token Configuration
    //Potentially can be read from a config file
    public static class TokenConfigs
    {
        public static char Separator { get { return Joiner.ToCharArray().First(); }}

        public static string Joiner { get { return " "; }}

        public static string Pipe { get { return " > "; }}

        public static string Seq { get { return " | "; }}

        public static string TextStart { get { return "\""; }}

        public static string TextEnd { get { return "\""; }}

        public static string VarNameStart { get { return "*"; } }
    }
}
