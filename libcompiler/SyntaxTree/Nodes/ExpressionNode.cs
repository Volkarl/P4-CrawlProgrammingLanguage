namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class ExpressionNode : CrawlSyntaxNode
    {
        public ExpressionType ExpressionType { get; }

        protected ExpressionNode(CrawlSyntaxNode parent, Internal.ExpressionNode self, int slot) : base(parent, self, slot)
        {
            ExpressionType = self.ExpressionType;
        }
    }
}