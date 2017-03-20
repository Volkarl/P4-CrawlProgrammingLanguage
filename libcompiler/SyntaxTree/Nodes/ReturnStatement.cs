namespace libcompiler.SyntaxTree.Nodes
{
    public class ReturnStatement : CrawlSyntaxNode
    {
        private ExpressionNode _retval;
        public ExpressionNode ReturnValue => GetRed(ref _retval, 0);

        public ReturnStatement(CrawlSyntaxNode parent, Internal.ReturnStatement self, int slot) : base(parent, self, slot)
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return ReturnValue;
                default: return default(CrawlSyntaxNode);
            }
        }
    }
}