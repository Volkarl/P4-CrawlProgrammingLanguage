using System;

namespace libcompiler.SyntaxTree.Nodes
{
    public class TokenNode : CrawlSyntaxNode
    {
        public string Value { get; }
        public TokenNode(CrawlSyntaxNode parent, Internal.TokenNode self, int slot) : base(parent, self, slot)
        {
            Value = self.Value;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}
