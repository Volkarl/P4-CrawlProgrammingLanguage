using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    /// <summary>
    /// Any any programmer-defined name that isn't a variable
    /// </summary>
    public class GreenIdentifierNode : GreenCrawlSyntaxNode
    {
        public string Value { get; }

        protected GreenIdentifierNode(Interval interval, string value, NodeType nodeType) : base(nodeType, interval)
        {
            Value = value;
            ChildCount = 0;
        }

        public GreenIdentifierNode(Interval interval, string value) : this(interval, value, NodeType.Identifier)
        {
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            return null;
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.IdentifierNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
