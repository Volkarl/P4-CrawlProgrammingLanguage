using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class MultiChildExpressionNode : ExpressionNode
    {
        private ListNode<ExpressionNode> _args;
        public ListNode<ExpressionNode> Arguments => GetRed(ref _args, 0);

        public MultiChildExpressionNode(CrawlSyntaxNode parrent, Internal.MultiChildExpressionNode self, int slot) : base(parrent, self, slot)
        {
            
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}