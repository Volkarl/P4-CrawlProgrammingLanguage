namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// Represents any expression that has 2 and only 2 parts. 
    /// This can be comparisons and subfield access
    /// NB: Subfield should really have its own node with a string as 
    /// RightHandSide. Otherwise it is possible to represent ilegal such as
    /// foo.(4 + 2)
    /// </summary>
    public class BinaryNode : ExpressionNode
    {
        private ExpressionNode _lhs;
        private ExpressionNode _rhs;

        public ExpressionNode LeftHandSide => GetRed(ref _lhs, 0);
        public ExpressionNode RightHandSide => GetRed(ref _rhs, 1);

        public BinaryNode(CrawlSyntaxNode parent, Internal.ExpressionNode self, int indexInParent)
            : base(parent, self, indexInParent)
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
                case 0: return LeftHandSide;
                case 1: return RightHandSide;
                default: return default(CrawlSyntaxNode);
            }
        }
    }
}