using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class BinaryNode : ExpressionNode
    {
        public GreenNode LeftHandSide { get; }
        public GreenNode RightHandSide { get; }

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

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return LeftHandSide;
                case 1: return RightHandSide;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.BinaryNode(parrent, this, slot);
        }
    }
}