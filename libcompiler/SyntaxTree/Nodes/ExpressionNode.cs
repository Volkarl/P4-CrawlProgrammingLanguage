namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class ExpressionNode : CrawlSyntaxNode
    {
        /// <summary>
        /// The type of this expression. This could be add, call or multiply
        /// </summary>
        public ExpressionType ExpressionType { get; }

        protected ExpressionNode(CrawlSyntaxNode parent, Internal.GreenExpressionNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            ExpressionType = self.ExpressionType;
        }
    }
}