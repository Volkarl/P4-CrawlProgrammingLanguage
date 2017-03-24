using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class LiteralNode : ExpressionNode
    {
        public string Value { get; }
        public LiteralType LiteralType { get; }

        public LiteralNode(Interval interval, string value, LiteralType literalType)
            : base(interval, NodeType.Literal, ExpressionType.Constant)
        {
            Value = value;
            LiteralType = literalType;
            ChildCount = 0;
        }

        

        public override string ToString()
        {
            return Value;
        }

        public override GreenNode GetChildAt(int slot)
        {
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.LiteralNode(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}