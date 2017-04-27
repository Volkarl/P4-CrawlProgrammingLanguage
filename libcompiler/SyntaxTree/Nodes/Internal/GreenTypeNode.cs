using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenTypeNode : GreenCrawlSyntaxNode
    {
        public String TypeTextDefinition { get; }
        public CrawlType ExportedType { get; }
        public bool IsReference { get; set; }

        public GreenTypeNode(Interval interval, string typeTextDefinition, CrawlType expotedType, bool isReference)
            : base(NodeType.Type, interval)
        {
            TypeTextDefinition = typeTextDefinition;
            ExportedType = expotedType;
            IsReference = isReference;
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
