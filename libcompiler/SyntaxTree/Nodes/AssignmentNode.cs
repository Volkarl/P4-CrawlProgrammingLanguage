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

        public AssignmentNode(CrawlSyntaxNode parent, GreenNode self, int slot) : base(parent, self, slot)
        {

        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}