using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    class UnaryNode : ExpressionNode
    {
        public ExpressionNode Target { get; }

        public UnaryNode(Interval interval, ExpressionType expressionType, ExpressionNode target) : base(interval, NodeType.UnaryExpression, expressionType)
        {
            Target = target;
            ChildCount = 1;
        }

        public override GreenNode GetChildAt(int slot)
        {
            if (slot == 0)
                return Target;

            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.UnaryNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if(index == 0) return new UnaryNode(Interval, ExpressionType, (ExpressionNode)newChild);

            throw new ArgumentOutOfRangeException();
        }
    }
}
