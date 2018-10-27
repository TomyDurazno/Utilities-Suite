using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Core.Streams
{
    public class StreamProvider
    {
        public Func<string> Reader { get; set; }
        public Func<string, string> Writer { get; set; }

        public StreamProvider(Func<string> reader, Func<string, string> writer)
        {
            Reader = reader;
            Writer = writer;
        }
    }
}
