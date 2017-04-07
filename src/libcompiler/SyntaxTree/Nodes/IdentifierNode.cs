using System;

namespace libcompiler.SyntaxTree.Nodes
{
    public class IdentifierNode : CrawlSyntaxNode
    {
        public string Value { get; }
        public IdentifierNode(CrawlSyntaxNode parent, Internal.GreenIdentifierNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            Value = self.Value;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }
    }
}
