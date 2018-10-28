using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Core.Attributes
{
    /*
        This attributes are the ones used to recognize Invocables to be run
    */

    public class InvokerAttributes
    {
        public class Invoker : Attribute
        {
            public string Name { get; set; }

            public Invoker(string name)
            {
                Name = name;
            }
        }

        public class InvokerCaller : Attribute
        {
            public string Description { get; set; }

            public InvokerCaller()
            {

            }
            
            public InvokerCaller(string description)
            {
                Description = description;
            }
        }
    }
}
