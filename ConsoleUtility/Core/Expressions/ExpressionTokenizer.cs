using Utility.Core.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Tools;

namespace Utility.Core.Expressions
{
    public class TokenInputStream
    {
        public List<Token> HighLevelTokens { get; set; }

        public TokenInputStream()
        {
            HighLevelTokens = new List<Token>();
        }

        public TokenPlainStream GetStream()
        {
            return new TokenPlainStream(HighLevelTokens.SelectMany(h => h.GetGraph()).Where(t => !t.IsParent));
        }

        public void AddToken(Token token)
        {
            HighLevelTokens.Add(token);
        }

        public void AddTokens(IEnumerable<Token> tokens)
        {
            HighLevelTokens.AddRange(tokens);
        }

        public Command? Command
        {
            get
            {
                var token = HighLevelTokens.First();
                return MyUtils.GetEnums<Command>()
                              .Where(e => e.ToString().ToLower() == token.Value.ToLower())
                              .FirstOrDefault();
            }
        }

        public bool IsCommandInput => Command.HasValue;
    }

    public class TokenPlainStream
    {
        public IEnumerable<Token> PlainTokens
        {
            get; set;
        }

        public TokenPlainStream(IEnumerable<Token> tokens)
        {
            PlainTokens = tokens;
        }
    }

    public static class TokenInputStreamExtensions
    {
        public static string GetStringsRepresentation(this TokenPlainStream stream)
        {
            var values = stream.PlainTokens.Select(t => t.StringRepresentation);
            
            return values.Project(s => string.Join(TokenConfigs.Joiner, s));
        }            
    }

    public class TokenStreamer
    {
        public static TokenInputStream MakeTokenInput(string sInput)
        {
            var stream = new TokenInputStream();

            var splited = sInput.Split(TokenConfigs.Separator);

            var first = new FirstToken() { Value = splited.FirstOrDefault(), TokenType = TokenType.Init_Line };

            stream.AddToken(first);

            var execPlan = splited.Skip(1).Project(ss => string.Join(TokenConfigs.Joiner, ss));

            var sSeqs = execPlan.Split(new string[] { TokenConfigs.Seq }, StringSplitOptions.None);

            var seqSymbols = sSeqs.Length.Project(l => l - 1).ToEnumerable(() => new SeqSymbolToken() { Value = TokenConfigs.Seq,  });

            int cont = 1;

            if (seqSymbols.Any())
            {
                foreach (var seq in sSeqs)
                {
                    var seqs = MakePipeExpressions(seq);
                    stream.AddTokens(seqs);

                    if (cont < sSeqs.Length)
                    {
                        stream.AddToken(seqSymbols.First());
                    }

                    cont++;
                }
            }
            else
            {
                //no seq symbol
                stream.AddTokens(MakePipeExpressions(execPlan));
            }
          
            return stream;
        }

        public static IEnumerable<Token> MakePipeExpressions(string execPlan)
        {
            var spipes = execPlan.Split(new string[] { TokenConfigs.Pipe }, StringSplitOptions.None);

            //Pipe expressions
            var expressions = spipes.Select(sp => new PipeExpressionToken() { Value = sp.Trim(), TokenType = TokenType.Expression });

            //Pipe delimitor
            var pipeSymbols = spipes.Length.Project(l => l - 1).ToEnumerable(() => new PipeSymbolToken() { Value = TokenConfigs.Pipe });

            if (pipeSymbols.Any())
            {
                return expressions.Enlaze<Token>(pipeSymbols);
            }
            else
            {
                return (expressions as IEnumerable<Token>);
            }
        }

        public static List<ArgToken> ParseByDelimiters(string[] sArgs)
        {
            var parsed = Parser.ByDelimiters(sArgs, TokenConfigs.TextStart, TokenConfigs.TextEnd, TokenConfigs.Joiner)
                               .Where(v => !string.IsNullOrEmpty(v));

            return parsed.Select(p => p.Contains(TokenConfigs.Separator) ? new TextArgToken(p, TokenType.Arg) : new ArgToken(p, TokenType.Arg))                        
                         .ToList();
        }
    }
}
