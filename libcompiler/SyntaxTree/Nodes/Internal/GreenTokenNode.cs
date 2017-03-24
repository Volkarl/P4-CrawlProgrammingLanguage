using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class TokenNode : GreenNode
    {
        public string Value { get; }

        public TokenNode(Interval interval, string value) : base(NodeType.Token, interval)
        {
            Value = value;
            ChildCount = 0;
        }

        public override GreenNode GetChildAt(int slot)
        {
            return null;
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.TokenNode(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
