using Utility.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Utility.Core.Attributes.InvokerAttributes;
using Utility.Tools;

namespace Utility.Invocables
{
    [Invoker("readtext", "Read a text file from desktop")]
    public class ReadTxt
    {
        private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        //Must implement method that returns void and recive string[]
        //or implement method that returns Task and receive string[]
        //with dedicated attributes so it can be called in runtime 
        [InvokerCaller()]
        public void Read(string[] arguments)
        {
            var filename = string.Join(" ", arguments);

            if (filename == null)
            {
                Console.WriteLine("Missing file name");
                return;
            }

            var dir = Path.Combine(desktopPath, filename);
            if (!File.Exists(dir))
            {
                Console.WriteLine("File doesn't exists: {0}", dir);
                return;
            }

            var lines = MyUtils.ReadTxtFromDesktop(filename);

            foreach (var item in lines)
            {
                Console.WriteLine(item);
            }
        }
    }

    [Invoker("open", "Opens a file from desktop")]
    public class Opener
    {
        private static string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        [InvokerCaller()]
        public void Open(string[] arguments)
        {
            var fileName = arguments.FirstOrDefault();

            if (fileName == null)
            {
                Console.WriteLine("Missing file name");
                return;
            }

            var dir = Path.Combine(desktopPath, fileName);
            if (!File.Exists(dir))
            {
                Console.WriteLine("File doesn't exists: {0}", dir);
                return;
            }

            MyUtils.OpenFileFromDesktop(fileName);
        }
    }
}
