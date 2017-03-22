using System;

namespace libcompiler.SyntaxTree.Nodes
{
    public class TokenNode : CrawlSyntaxNode
    {
        public string Value { get; }
        public TokenNode(CrawlSyntaxNode parent, Internal.TokenNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            Value = self.Value;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }
    }
}
