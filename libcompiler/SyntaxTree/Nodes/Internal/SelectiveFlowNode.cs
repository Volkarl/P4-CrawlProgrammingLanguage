using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class SelectiveFlowNode : FlowNode
    {
        public ExpressionNode Check { get; }
        public BlockNode Primary { get; }
        public BlockNode Alternative { get; }

        

        public SelectiveFlowNode(Interval interval, NodeType type, ExpressionNode check, BlockNode primary, BlockNode alternative) : base(type, interval)
        {
            Check = check;
            Primary = primary;
            Alternative = alternative;

            ChildCount = alternative == null ? 2 : 3;
        }

        public override GreenNode GetChildAt(int slot)
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

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.SelectiveFlowNode(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new SelectiveFlowNode(Interval, Type, (ExpressionNode) newChild, Primary, Alternative);
                case 1: return new SelectiveFlowNode(Interval, Type, Check, (BlockNode) newChild, Alternative);
                case 2: return new SelectiveFlowNode(Interval, Type, Check, Primary, (BlockNode) newChild);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}