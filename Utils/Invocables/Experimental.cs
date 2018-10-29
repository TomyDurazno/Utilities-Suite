using Utility.Core;
using Utility.Core.Expressions;
using Utility.Core.Streams;
using Utility.Core.Tokens;
using static Utility.Core.Attributes.InvokerAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Tools;

namespace Utility.Invokers
{

    [Invoker("inside")]
    public class Insider
    {
        [InvokerCaller]
        public async Task Inside(string[] args)
        {
            var stream = new StreamProvider(() => string.Join(TokenConfigs.SeparatorString, args).CleanString(TokenConfigs.TextStart, TokenConfigs.TextEnd), input => { Console.WriteLine(input); return string.Empty; });

            var invoker = new InvokerService("Inside", stream, true, Console.WriteLine);

            await invoker.Run();

            { }
        }
    }

    [Invoker("dynamic")]
    public class DynamicCall
    {
        [InvokerCaller]
        public async Task Call(string[] args)
        {
            var name = args.First();
            var skipargs = args.Skip(1).ToArray();
            var binderService = new BinderService();

            await binderService.DynamicGenInvoke(name, skipargs);
        }
    }

    [Invoker("Date")]
    public class Date
    {
        [InvokerCaller]
        public async Task<DateTime> Dater(string[] args)
        {
            //first arg milliseconds of interval
            var milliseconds = args.First().Project(Convert.ToInt32);
            var seconds = args.Skip(1).First().Project(Convert.ToInt32);

            var stop = DateTime.Now.Add(new TimeSpan(0, 0, seconds)).TimeOfDay;
            //second arg time stop -> amount of minuts
            //action
            return await MyUtils.SetTimeOutAsync(milliseconds, stop, () => DateTime.Now);
        }
    }

    [Invoker("hour")]
    public class Add1Hour
    {
        [InvokerCaller]
        public async Task<DateTime> Add(DateTime arg)
        {
            return await Task.FromResult(arg.AddHours(1));
        }
    }

    [Invoker("todate")]
    public class ToDate
    {
        [InvokerCaller]
        public async Task<DateTime> Add(string arg)
        {
            return await Task.FromResult(Convert.ToDateTime(arg));
        }
    }

    [Invoker("dump")]
    public class Dumper
    {
        [InvokerCaller]
        public async Task<dynamic> Dump(dynamic arg)
        {
            Console.WriteLine(arg); //Mutation, now I understand !!
            return await Task.FromResult(arg);
        }
    }

    [Invoker("log")]
    public class Log
    {
        [InvokerCaller]
        public void Logs(object log)
        {
            Console.WriteLine(log);
        }
    }


    [Invoker("Counter")]
    public class Counter
    {
        [InvokerCaller]
        public async Task Count(string[] arguments)
        {
            var times = Convert.ToInt32(arguments.First());

            var interval = Convert.ToInt32(arguments.Skip(1).First());

            var counter = 1;

            while (times >= counter)
            {
                Console.WriteLine(counter);
                await Task.Delay(new TimeSpan(0, 0, interval));
                counter++;
            }
        }
    }
}
