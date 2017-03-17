using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class BinaryNode : ExpressionNode
    {
        public ExpressionNode LeftHandSide { get; }
        public ExpressionNode RightHandSide { get; }

        public BinaryNode(Interval interval, ExpressionNode lhs,
            ExpressionNode rhs, ExpressionType type) : base(interval, NodeType.BinaryExpression, type)
        {
            LeftHandSide = lhs;
            RightHandSide = rhs;
        }

        private static NodeType HideExpressionType(ExpressionType type, NodeType nodeType)
        {
            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            return (NodeType)((int)type << 8) | nodeType;
        }

        public override GreenNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return LeftHandSide;
                case 1: return RightHandSide;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.BinaryNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if(index == 0)
                return new BinaryNode(this.Interval, (ExpressionNode) newChild, RightHandSide, ExpressionType);
            else if(index == 1)
                return new BinaryNode(this.Interval, LeftHandSide, (ExpressionNode)newChild, ExpressionType);

            throw new ArgumentOutOfRangeException();
        }
    }
}