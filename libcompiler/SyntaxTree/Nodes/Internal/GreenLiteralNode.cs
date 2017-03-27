using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenLiteralNode : GreenExpressionNode
    {
        public string Value { get; }
        public LiteralType LiteralType { get; }

        public GreenLiteralNode(Interval interval, string value, LiteralType literalType)
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

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.LiteralNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}