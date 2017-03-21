namespace libcompiler.SyntaxTree.Nodes
{
    public class UnaryNode : ExpressionNode
    {
        public ExpressionNode Target => GetRed(ref _target, 0);

        private ExpressionNode _target;

        public UnaryNode(CrawlSyntaxNode parent, Internal.ExpressionNode self, int slot) : base(parent, self, slot)
        {

        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            if(index == 0)
                return Target;

            return default(CrawlSyntaxNode);
        }
    }

}