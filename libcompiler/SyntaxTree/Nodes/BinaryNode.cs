using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class BinaryNode : ExpressionNode
    {
        public ExpressionNode LeftHandSide { get; }
        public ExpressionNode RightHandSide { get; }
        public ExpressionType ExpressionType { get; }

        public BinaryNode(CrawlSyntaxTree owningTree, Interval interval, ExpressionType type, ExpressionNode lhs,
            ExpressionNode rhs) : base(owningTree, interval, NodeType.BinaryExpression)
        {
            ExpressionType = type;
            LeftHandSide = lhs;
            RightHandSide = rhs;
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }
    }
}