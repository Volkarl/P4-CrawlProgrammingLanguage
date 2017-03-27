using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// Represents an assignment (a = b;)
    /// </summary>
    public class AssignmentNode : CrawlSyntaxNode
    {
        private ExpressionNode _target;
        private ExpressionNode _value;

        /// <summary>
        /// The target of the assignment
        /// </summary>
        public ExpressionNode Target => GetRed(ref _target, 0);

        /// <summary>
        /// The value
        /// </summary>
        public ExpressionNode Value => GetRed(ref _value, 1);

        public AssignmentNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {

        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return Target;
                case 1: return Value;
                default: return default(CrawlSyntaxNode);
            }
        }
    }
}