using System.Linq;
using System.Text;
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
}
