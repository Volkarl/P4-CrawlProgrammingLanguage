using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class SelectiveFlowNode : FlowNode
    {
        public ExpressionNode Check { get; }
        public BlockNode Primary { get; }
        public BlockNode Alternative { get; }

        

        public SelectiveFlowNode(Interval interval, FlowType type, ExpressionNode check, BlockNode primary, BlockNode alternative) : base(MakeNodeType(type), interval)
        {
            Check = check;
            Primary = primary;
            Alternative = alternative;
        }

        private static NodeType MakeNodeType(FlowType type)
        {
            switch (type)
            {
                case FlowType.If:
                    return NodeType.If;
                case FlowType.IfElse:
                    return NodeType.IfElse;
                case FlowType.While:
                    return NodeType.While;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public enum FlowType
        {
            If,
            IfElse,
            While,
        }

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return Check;
                case 1: return Primary;
                case 2: return Alternative;

                default:
                    return default(GreenNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.SelectiveFlowNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new NotImplementedException();
        }
    }
}