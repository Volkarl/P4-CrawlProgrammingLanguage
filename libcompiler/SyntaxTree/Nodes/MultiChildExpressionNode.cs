namespace libcompiler.SyntaxTree.Nodes
{
    public class MultiChildExpressionNode : ExpressionNode
    {
        private ListNode<ExpressionNode> _args;
        public ListNode<ExpressionNode> Arguments => GetRed(ref _args, 0);

        public MultiChildExpressionNode(CrawlSyntaxNode parent, Internal.MultiChildExpressionNode self, int slot) : base(parent, self, slot)
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