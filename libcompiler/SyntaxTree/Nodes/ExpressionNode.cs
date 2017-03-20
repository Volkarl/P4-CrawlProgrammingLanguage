namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class ExpressionNode : CrawlSyntaxNode
    {
        /// <summary>
        /// The type of this expression. This could be add, call or multiply
        /// </summary>
        public ExpressionType ExpressionType { get; }

        protected ExpressionNode(CrawlSyntaxNode parent, Internal.ExpressionNode self, int slot) : base(parent, self, slot)
        {
            ExpressionType = self.ExpressionType;
        }
    }
}