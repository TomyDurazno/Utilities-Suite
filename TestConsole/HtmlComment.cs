using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Tools;
using static Utility.Core.Attributes.InvokerAttributes;

namespace ConsoleTest
{
    [Invoker("hcomment")]
    public class HtmlComment
    {
        [InvokerCaller]
        public string Dum(string[] arguments)
        {
            return arguments.Aggregate(new StringBuilder(), (builder, s) => builder.Append(string.Format("<!--{0}--> ", s))).ToString();
        }
    }

    [Invoker("hsum")]
    public class Type
    {
        //public class Internal
        //{
        //    public Lazy<List<string>> Added { get { return new Lazy<List<string>>(); }}
        //}

        //public Lazy<Internal> State { get; set; }

        public Lazy<StringBuilder> SBuilder { get { return new Lazy<StringBuilder>(); }}

        [InvokerCaller]
        public string Dum(string[] arguments)
        {
            return arguments.Aggregate(SBuilder.Value, (builder, s) => builder.Append(string.Format("<!--{0}--> ", s))).ToString();
        }
    }
}
