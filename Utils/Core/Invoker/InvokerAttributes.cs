using System;

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

            public string Description { get; set; }

            public Invoker(string name)
            {
                Name = name;
            }

            public Invoker(string name, string description) : this (name)
            {
                Description = description;
            }
        }

        public class InvokerCaller : Attribute
        {
            public InvokerCaller()
            {

            }
        }
    }
}
