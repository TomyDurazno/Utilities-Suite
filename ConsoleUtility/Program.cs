using Utility.Core.Streams;
using System;
using System.Threading.Tasks;
using Utility.Core;

namespace Utility
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var stream = new StreamProvider(Console.ReadLine, input =>
            {
                Console.WriteLine();
                Console.WriteLine(input);
                return string.Empty;
            });

            var invoker = new InvokerService("Main", stream);

            invoker.PostRun = Console.WriteLine;

            await invoker.Run();
        }
    }
}
