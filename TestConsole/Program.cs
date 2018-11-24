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
            Console.SetWindowSize(100, 10);
            var provider = new IOProvider();
            await new InvokerService("Main", new StreamProvider(provider.Reader, provider.Writer), false, provider.PostRun).Run();
        }

        public class IOProvider
        {        
            public string Reader()
            {
                return Console.ReadLine();
            }

            public string Writer(string input)
            {
                Console.WriteLine();
                Console.WriteLine(input);
                return string.Empty;
            }

            public void PostRun(string s)
            {
                Console.WriteLine(s);
            }
        }
    }
}
