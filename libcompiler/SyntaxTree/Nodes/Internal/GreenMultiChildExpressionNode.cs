using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class MultiChildExpressionNode : ExpressionNode
    {
        public ListNode<Nodes.ExpressionNode> Arguments { get; }

        public MultiChildExpressionNode(Interval interval, ExpressionType expressionType, ListNode<Nodes.ExpressionNode> arguments) : base(interval, NodeType.MultiExpression, expressionType)
        {
            Arguments = arguments;
            ChildCount = 1;
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }

        public override GreenNode GetChildAt(int slot)
        {
            if (slot == 0) return Arguments;

            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.MultiChildExpressionNode(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if(index == 0)
                return new MultiChildExpressionNode(this.Interval, ExpressionType, (ListNode<Nodes.ExpressionNode>) newChild);

            throw new ArgumentOutOfRangeException();
        }
    }
}