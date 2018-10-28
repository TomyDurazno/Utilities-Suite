using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Core.Streams
{
    /*
        The concept of StreamProvider is a way to give I/O to InvokerService. 
        Analyze if it make sense of it working with generic/dynamic types.
    */

    public class StreamProvider
    {
        public Func<string> Reader { get; }
        public Func<string, string> Writer { get; }

        public StreamProvider(Func<string> reader, Func<string, string> writer)
        {
            Reader = reader;
            Writer = writer;
        }
    }
}
