namespace libcompiler.SyntaxTree.Nodes
{
    public class LiteralNode : ExpressionNode
    {
        public string Value { get; }
        public LiteralType LiteralType { get; }

        public LiteralNode(CrawlSyntaxNode parent, Internal.LiteralNode self, int slot) : base(parent, self, slot)
        
        {
            Value = self.Value;
            LiteralType = self.LiteralType;
        }

        public override string ToString()
        {
            return Value;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}