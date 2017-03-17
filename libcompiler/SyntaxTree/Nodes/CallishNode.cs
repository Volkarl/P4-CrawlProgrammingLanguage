using System;

namespace libcompiler.SyntaxTree.Nodes
{
    public class CallishNode : ExpressionNode
    {
        private ExpressionNode _target;
        private ListNode<ExpressionNode> _arguments;


        public ExpressionNode Target => GetRed(ref _target, 0);
        public ListNode<ExpressionNode> Arguments => GetRed(ref _arguments, 1);


        public CallishNode(CrawlSyntaxNode parent, Internal.ExpressionNode self, int slot) : base(parent, self, slot)
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}