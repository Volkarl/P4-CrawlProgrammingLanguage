using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenSelectiveFlowNode : GreenFlowNode
    {
        public GreenExpressionNode Check { get; }
        public GreenBlockNode Primary { get; }
        public GreenBlockNode Alternative { get; }

        

        public GreenSelectiveFlowNode(Interval interval, NodeType type, GreenExpressionNode check, GreenBlockNode primary, GreenBlockNode alternative) : base(type, interval)
        {
            Check = check;
            Primary = primary;
            Alternative = alternative;

            ChildCount = alternative == null ? 2 : 3;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return Check;
                case 1: return Primary;
                case 2: return Alternative;

                default:
                    return default(GreenCrawlSyntaxNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.SelectiveFlowNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new GreenSelectiveFlowNode(Interval, Type, (GreenExpressionNode) newChild, Primary, Alternative);
                case 1: return new GreenSelectiveFlowNode(Interval, Type, Check, (GreenBlockNode) newChild, Alternative);
                case 2: return new GreenSelectiveFlowNode(Interval, Type, Check, Primary, (GreenBlockNode) newChild);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}