using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class MultiChildExpressionNode : ExpressionNode
    {
        public ListNode<Nodes.ExpressionNode> Arguments { get; }

        public MultiChildExpressionNode(Interval interval, ExpressionType expressionType, ListNode<Nodes.ExpressionNode> arguments) : base(interval, NodeType.MultiExpression, expressionType)
        {
            Arguments = arguments;
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }

        public override GreenNode GetSlot(int slot)
        {
            if (slot == 0) return Arguments;

            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.MultiChildExpressionNode(parrent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if(index == 0)
                return new MultiChildExpressionNode(this.Interval, ExpressionType, (ListNode<Nodes.ExpressionNode>) newChild);

            throw new ArgumentOutOfRangeException();
        }
    }
}