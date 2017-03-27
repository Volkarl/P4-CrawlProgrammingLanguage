namespace libcompiler.SyntaxTree.Nodes
{
    public class MultiChildExpressionNode : ExpressionNode
    {
        private ListNode<ExpressionNode> _args;
        public ListNode<ExpressionNode> Arguments => GetRed(ref _args, 0);

        public MultiChildExpressionNode(CrawlSyntaxNode parent, Internal.GreenMultiChildExpressionNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return Arguments;
                default: return default(CrawlSyntaxNode);
            }
        }
    }
}