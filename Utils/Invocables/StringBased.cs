using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utility.Core.Attributes.InvokerAttributes;

namespace Utility.Invocables
{
    [Invoker("Join")]
    public class Joiner
    {
        //Must implement method that returns void and recive string[]
        //or implement method that returns Task and receive string[]
        //with dedicated attributes so it can be called in runtime 
        [InvokerCaller]
        public void Join(string[] arguments)
        {
            var separator = arguments.First();

            var args = arguments.Skip(1);

            Console.WriteLine(string.Join(separator, args));
        }
    }

    [Invoker("Concat")]
    public class Concat
    {
        [InvokerCaller]
        public void Concater(string[] args)
        {
            Console.WriteLine(string.Concat(args));
        }
    }

    [Invoker("tostring")]
    public class ToString
    {
        [InvokerCaller]
        public string Stringer(object obj)
        {
            return obj.ToString();
        }
    }

    [Invoker("Upper")]
    public class Upper
    {
        [InvokerCaller]
        public string Up(string arg)
        {
            return arg.ToUpper();
        }
    }

    [Invoker("Guid", "returns an instance of System.Guid")]
    public class Guider
    {
        [InvokerCaller()]
        public string GiveGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }

    [Invoker("format")]
    public class Formatter
    {
        [InvokerCaller]
        public void Form(string[] arguments)
        {
            var format = arguments.FirstOrDefault();

            var args = arguments.Skip(1);

            var argsObj = args.Select(a => (object)a).ToArray();

            Console.WriteLine(string.Format(format, argsObj));
        }
    }

    [Invoker("repeat")]
    public class Repeater
    {
        [InvokerCaller]
        public string[] Repeat(string[] arguments)
        {
            var times = Convert.ToInt32(arguments.FirstOrDefault());

            var arg = arguments.Skip(1).FirstOrDefault();

            return Enumerable.Repeat(arg, times).ToArray();
        }
    }
}
