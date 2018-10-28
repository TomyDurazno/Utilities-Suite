using System;
using System.Threading.Tasks;
using Utility.Core;
using Utility.Core.Streams;

namespace ConsoleTest
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var invoker = new InvokerService("Main", new StreamProvider(Console.ReadLine, input =>
            {
                Console.WriteLine();
                Console.WriteLine(input);
                return string.Empty;
            }));

            invoker.PostRun = Console.WriteLine;

            await invoker.Run();
        }
    }
}
