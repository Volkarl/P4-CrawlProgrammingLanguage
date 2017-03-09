using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class AssignmentNode : CrawlSyntaxNode
    {
        /// <summary>
        /// The target of the assignment
        /// </summary>
        public ExpressionNode Target { get; }

        /// <summary>
        /// The value
        /// </summary>
        public ExpressionNode Value { get; }

        public AssignmentNode(CrawlSyntaxTree owningTree, Interval interval, ExpressionNode target,
            ExpressionNode value) : base(owningTree, NodeType.Assignment, interval)
        {
            Target = target;
            Value = value;
        }
    }
}