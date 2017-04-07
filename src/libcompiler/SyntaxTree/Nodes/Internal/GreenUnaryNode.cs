using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    class GreenUnaryNode : GreenExpressionNode
    {
        public GreenExpressionNode Target { get; }

        public GreenUnaryNode(Interval interval, ExpressionType expressionType, GreenExpressionNode target) : base(interval, NodeType.UnaryExpression, expressionType)
        {
            Target = target;
            ChildCount = 1;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            if (slot == 0)
                return Target;

            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.UnaryNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            if(index == 0) return new GreenUnaryNode(Interval, ExpressionType, (GreenExpressionNode)newChild);

            throw new ArgumentOutOfRangeException();
        }
    }
}
