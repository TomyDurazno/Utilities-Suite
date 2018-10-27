using Utility.Core;
using Utility.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Tools;

namespace Utility.Core.Tokens
{
    public enum TokenType
    {
        Init_Line,
        Init_Expression,
        Arg,
        Symbol,
        Expression
    }

    #region Token Types

    public class Token
    {
        public virtual List<Token> Tokens { get; set; }

        public List<Token> GetGraph()
        {
            var result = new List<Token>() { this };

            if (Tokens != null)
                result.AddRange(Tokens.SelectMany(t => t.GetGraph()));

            return result;
        }

        public TokenType TokenType { get; set; }

        public string Value { get; set; }

        public virtual bool IsParent => false;

        public virtual string StringRepresentation => Value.Trim();

        public virtual bool IsPipeSymbol => false;

        public virtual bool IsVarNameDefinition => false;

        public virtual bool IsSeqSymbol => false;
    }

    public class FirstToken : Token
    {

    }

    public class ArgToken : Token
    {
        public ArgToken(string value, TokenType ttype)
        {
            Value = value;
            TokenType = ttype;
        }

        public override string StringRepresentation => Value.CleanString(TokenConfigs.TextStart, TokenConfigs.TextEnd);

        public override bool IsVarNameDefinition
        {
            get
            {
                return Value.StartsWith(TokenConfigs.VarNameStart);
            }
        }
    }

    public class TextArgToken : ArgToken
    {
        public TextArgToken(string value, TokenType ttype) : base(value, ttype)
        {

        }

        public override string StringRepresentation => string.Concat(TokenConfigs.TextStart, Value.Trim(), TokenConfigs.TextEnd);
    }

    public class SymbolToken : Token
    {

    }

    public class PipeSymbolToken : Token
    {
        public override bool IsPipeSymbol => true;
    }

    public class SeqSymbolToken : Token
    {
        public override bool IsSeqSymbol => true;
    }

    public class ExpressionToken : Token
    {

    }

    public class PipeExpressionToken : ExpressionToken
    {
        public override List<Token> Tokens
        {
            get
            {
                var splited = Value.Split(TokenConfigs.Separator);

                var scommand = splited.FirstOrDefault();

                var items = new List<Token>() { new FirstToken() { Value = scommand, TokenType = TokenType.Init_Expression } };

                TokenStreamer.ParseByDelimiters(splited.Skip(1).ToArray()).Call(items.AddRange);

                return items;
            }
        }

        public override bool IsParent => true;
    }

    #endregion
}
