using System.Linq;
using System.Text;
using static Utility.Core.Attributes.InvokerAttributes;

namespace ConsoleTest
{
    [Invoker("hnested")]
    public class HtmlNested
    {
        [InvokerCaller]
        public string Dum(string[] arguments)
        {
            return arguments.Reverse()
                            .Aggregate(string.Empty, (acum, s) => string.Format(string.IsNullOrEmpty(acum) ? "<{1}></{1}>" : "<{1}>{0}</{1}>", acum, s));            
        }
    }
}
