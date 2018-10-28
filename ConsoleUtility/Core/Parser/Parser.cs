using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /*

        Parser is used to transform a string input into chuncks of strings (as an array)
        to later be converted in a token stream by the ExpressionTokenizer

    */

    public static class Parser
    {
        public static string[] ByDelimiters(string[] args, string startsWith, string endsWith, string joinsWith)
        {
            bool inAcum = false;

            var acum = new List<string>();

            var list = new List<string>();

            foreach (var word in args)
            {
                if (!inAcum && word.StartsWith(startsWith)) // && word.Contains(joinsWith))//Start
                {
                    inAcum = true;
                    acum.Add(word.Substring(1));
                }
                else if (inAcum && word.EndsWith(endsWith)) // && word.Contains(joinsWith))//End
                {
                    inAcum = false;
                    acum.Add(word.Remove(word.Length - 1));

                    list.Add(string.Join(joinsWith, acum));
                    acum.Clear();
                }
                else if (inAcum)
                {
                    acum.Add(word);
                }
                else
                {
                    list.Add(word);
                }
            }

            return list.ToArray();
        }
    }
}
