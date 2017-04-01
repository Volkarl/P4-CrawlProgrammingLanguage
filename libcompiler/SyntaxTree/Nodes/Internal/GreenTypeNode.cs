using System;
using Antlr4.Runtime.Misc;
using libcompiler.TypeSystem;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenTypeNode : GreenCrawlSyntaxNode
    {
        public string TypeName { get; }

        public GreenTypeNode(Interval interval, string typeName) : base(NodeType.Type, interval)
        {
            TypeName = typeName;
            ChildCount = 0;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.TypeNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
