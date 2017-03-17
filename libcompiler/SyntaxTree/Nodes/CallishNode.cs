using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class CallishNode : ExpressionNode
    {
        private ExpressionNode _target;
        private ListNode<ExpressionNode> _arguments;


        public ExpressionNode Target => GetRed(ref _target, 0);
        public ListNode<ExpressionNode> Arguments => GetRed(ref _arguments, 1);


        public CallishNode(CrawlSyntaxNode parrent, Internal.ExpressionNode self, int slot) : base(parrent, self, slot)
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new NotImplementedException();
        }
    }
}