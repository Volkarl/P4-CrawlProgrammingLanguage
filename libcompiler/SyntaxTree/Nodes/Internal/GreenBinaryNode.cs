using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenBinaryNode : GreenExpressionNode
    {
        public GreenExpressionNode LeftHandSide { get; }
        public GreenExpressionNode RightHandSide { get; }

        public GreenBinaryNode(Interval interval, GreenExpressionNode lhs,
            GreenExpressionNode rhs, ExpressionType type) : base(interval, NodeType.BinaryExpression, type)
        {
            LeftHandSide = lhs;
            RightHandSide = rhs;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return LeftHandSide;
                case 1: return RightHandSide;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.BinaryNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            if(index == 0)
                return new GreenBinaryNode(this.Interval, (GreenExpressionNode) newChild, RightHandSide, ExpressionType);
            else if(index == 1)
                return new GreenBinaryNode(this.Interval, LeftHandSide, (GreenExpressionNode)newChild, ExpressionType);

            throw new ArgumentOutOfRangeException();
        }
    }
}