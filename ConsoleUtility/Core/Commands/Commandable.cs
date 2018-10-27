using Utility.Core.Expressions;
using Utility.Core.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Tools;

namespace Utility.Core.Commandables
{
    public class Commandable
    {

    }

    public class CallCommandable : Commandable
    {
        public string LookUpName { get; set; }

        public string[] Arguments { get; set; }

        public IEnumerable<Token> ArgsTokens { get; set; }

        public bool IsComodin()
        {
            return Arguments.FirstOrDefault()?.Project(a => a == "*") ?? false;
        }

        public bool StartsWithVarName
        {
            get
            {
                return ArgsTokens.FirstOrDefault()?.IsVarNameDefinition ?? false;
            }
        }

        public string VarName
        {
            get
            {
                return ArgsTokens.FirstOrDefault()?.Value;
            }
        }

        public CallCommandable(TokenPlainStream stream, int? skip = 1)
        {
            if (!skip.HasValue)
            {
                skip = 0;
            }

            LookUpName = stream.PlainTokens.Skip(skip.Value).First().Value;

            Arguments = stream.PlainTokens.Skip(skip.Value + 1).Select(t => t.Value).ToArray();

            ArgsTokens = stream.PlainTokens.Skip(skip.Value + 1);
        }

        public CallCommandable(IEnumerable<Token> PlainTokens)
        {
            //Already skiped command call
            LookUpName = PlainTokens.First().Value;

            Arguments = PlainTokens.Skip(1).Select(t => t.Value).ToArray();

            ArgsTokens = PlainTokens.Skip(1);
        } 
    }

    public class VarCommandable : Commandable
    {
        public string Name { get; set; }

        public string EqualsOperator { get; set; }

        public bool IsVarNameConvention
        {
            get
            {
                //should Token handle this ? dont know
                return Name.StartsWith(TokenConfigs.VarNameStart);
            }
        }


        public IEnumerable<Token> Statement { get; set; }

        public VarCommandable(IEnumerable<Token> PlainTokens)
        {
            //Already skiped command call
            Name = PlainTokens.Skip(1).First().Value;

            EqualsOperator = PlainTokens.Skip(2).First().Value;

            Statement = PlainTokens.Skip(3);
        }
    }

    public class HeapCommandable : Commandable
    {
        public string Operator { get; set; }

        public string[] Arguments { get; set; }

        public HeapCommandable(IEnumerable<Token> PlainTokens)
        {
            //Already skiped command call
            Operator = PlainTokens.Skip(1).First().Value;
            Arguments = PlainTokens.Skip(2).Where(t => t.IsVarNameDefinition).Select(t => t.Value).ToArray();
        }
    }

    public class SeqComandable : Commandable
    {
        public List<PipeCommandable> PipeCommandables { get; set; }

        public SeqComandable(TokenPlainStream stream)
        {
            PipeCommandables = new List<PipeCommandable>();

            var excs = stream.PlainTokens.Skip(1); // 'seq' call

            foreach (var items in excs.Chunk(t => t.IsSeqSymbol))
            {
                PipeCommandables.Add(new PipeCommandable(items));
            }
        }
    }

    public class PipeCommandable : Commandable
    {
        public List<CallCommandable> Commandables { get; set; }

        public bool IsDumper { get; set; }

        public PipeCommandable(TokenPlainStream stream)
        {
            Commandables = new List<CallCommandable>();

            var excs = stream.PlainTokens.Skip(1);

            foreach (var items in excs.Chunk(t => t.IsPipeSymbol))
            {
                Commandables.Add(new CallCommandable(items));
            }
        }

        public PipeCommandable(IEnumerable<Token> PlainTokens)
        {
            Commandables = new List<CallCommandable>();

            var excs = PlainTokens;//.Skip(1);

            foreach (var items in excs.Chunk(t => t.IsPipeSymbol))
            {
                Commandables.Add(new CallCommandable(items));
            }
        }
    }
}
