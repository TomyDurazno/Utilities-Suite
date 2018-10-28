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
            await new InvokerService("Main", new StreamProvider(Reader, Writer), false, PostRun).Run();
        }

        public static string Reader()
        {
            return Console.ReadLine();
        }

        static string Writer(string input)
        {
            Console.WriteLine();
            Console.WriteLine(input);
            return string.Empty;
        }

        static void PostRun(string s)
        {
            Console.WriteLine(s);
        }
    }
}
