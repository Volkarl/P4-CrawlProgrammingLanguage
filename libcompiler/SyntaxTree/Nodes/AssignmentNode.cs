using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
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

        public AssignmentNode(CrawlSyntaxNode parrent, GreenNode self, int slot) : base(parrent, self, slot)
        {

        }
    }
}