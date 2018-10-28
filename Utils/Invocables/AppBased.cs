using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static Utility.Core.Attributes.InvokerAttributes;

namespace Utility.Invocables
{
    [Invoker("ThrowEx", "Throws and exception when called")]
    public class ThrowEx
    {
        [InvokerCaller]
        public void Throw()
        {
            throw new Exception("BOOOM");
        }
    }

    [Invoker("Closer", "Close the application after an interval (in seconds)")]
    public class Closer
    {
        //Must implement method that returns void and recive string[]
        //or implement method that returns Task and receive string[]
        //with dedicated attributes so it can be called in runtime 
        [InvokerCaller]
        public async Task Close(string[] args)
        {
            await Task.Delay(Convert.ToInt32(args.First()) * 1000)
                      .ContinueWith(t => Environment.Exit(1));
        }
    }

    [Invoker("PlaySound")]
    public class PlaySound
    {
        [InvokerCaller]
        public async Task Play(string[] arguments)
        {
            var sounds = new Dictionary<string, SystemSound>();
            var props = typeof(SystemSounds).GetProperties();
            var notFound = new List<string>();

            if (!arguments.Any())
            {
                Console.WriteLine("sounds arguments missing");
            }
            else
            {
                foreach (var arg in arguments)
                {
                    var prop = props.Where(p => p.Name.ToLower() == arg.ToLower()).FirstOrDefault();

                    if (prop != null)
                    {
                        sounds.Add(arg, (SystemSound)prop.GetValue(null));
                    }
                    else
                    {
                        notFound.Add(arg);
                    }
                }

                foreach (var sound in sounds)
                {
                    sound.Value.Play();
                    Console.WriteLine("Played sound: {0}", sound.Key);
                    await Task.Delay(1000);
                }

                if (notFound.Any())
                    Console.WriteLine("Undefined sounds: {0}", string.Join(", ", notFound));
            }
        }
    }

    [Invoker("IP")]
    public class Ipper
    {
        [InvokerCaller]
        public void GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            if (ip != null)
                Console.WriteLine(ip.ToString());
            else
                Console.WriteLine("No network adapters with an IPv4 address in the system!");
        }
    }

    [Invoker("connected")]
    public class Connected
    {
        [InvokerCaller]
        public void Get()
        {
            Console.WriteLine(System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable());
        }
    }


    [Invoker("dump")]
    public class Dumper
    {
        [InvokerCaller]
        public string[] Dump(string[] args)
        {
            foreach (var item in args)
            {
                Console.WriteLine(item);
            }

            return args;
        }
    }
}
