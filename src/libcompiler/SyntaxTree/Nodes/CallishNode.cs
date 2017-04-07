using System;

namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// Represents a node looking like a call. This is actuall calls and index access.
    /// </summary>
    public class CallishNode : ExpressionNode
    {
        private ExpressionNode _target;
        private ListNode<ExpressionNode> _arguments;

        /// <summary>
        /// The expression to call.
        /// </summary>
        public ExpressionNode Target => GetRed(ref _target, 0);

        /// <summary>
        /// The arguments of the call. If no arguments it means an empty call __();
        /// </summary>
        public ListNode<ExpressionNode> Arguments => GetRed(ref _arguments, 1);


        public CallishNode(CrawlSyntaxNode parent, Internal.GreenExpressionNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return Target;
                case 1: return Arguments;
                default: return default(CrawlSyntaxNode);
            }
        }
    }
}