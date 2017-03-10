namespace libcompiler.SyntaxTree.Nodes
{
    public class LiteralNode : ExpressionNode
    {
        public string Value { get; }
        public LiteralType LiteralType { get; }

        public LiteralNode(CrawlSyntaxNode parrent, Internal.LiteralNode self, int slot) : base(parrent, self, slot)
        
        {
            Value = self.Value;
            LiteralType = self.LiteralType;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}