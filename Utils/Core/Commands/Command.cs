using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Core
{
    /*
        Commands that Dynamic Invoker recognizes
    */

    public enum Command
    {
        NotDefined = 0,
        Call,
        Seq,
        Pipe,        
        Types,
        Var,
        Heap,
        Clear,
        Exit,
        Commands,
        Invocables
    }
}
