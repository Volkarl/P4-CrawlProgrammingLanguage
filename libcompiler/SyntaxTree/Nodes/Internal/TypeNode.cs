using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class TypeNode : GreenNode
    {
        public CrawlType ExportedType { get; }

        public TypeNode(Interval interval, CrawlType expotedType) : base(NodeType.Type, interval)
        {
            ExportedType = expotedType;
            ChildCount = 0;
        }

        public override GreenNode GetChildAt(int slot)
        {
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.TypeNode(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
