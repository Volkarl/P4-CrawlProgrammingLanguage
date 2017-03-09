using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class AssignmentNode : CrawlSyntaxNode
    {
        public ExpressionNode LeftHandSide { get; }
        public ExpressionNode RightHandSide { get; }

        public AssignmentNode(CrawlSyntaxTree owningTree, Interval interval, ExpressionNode leftHandSide,
            ExpressionNode rightHandSide) : base(owningTree, NodeType.Assignment, interval)
        {
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide;
        }
    }
}