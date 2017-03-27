using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    class GreenAssignmentNode : GreenCrawlSyntaxNode
    {
        public GreenExpressionNode Target { get; }
        public GreenExpressionNode Value { get; }

        public GreenAssignmentNode(Interval interval, GreenExpressionNode target,
            GreenExpressionNode block) : base(NodeType.Assignment, interval)
        {
            Target = target;
            Value = block;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return Target;
                case 1: return Value;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.AssignmentNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            if(index == 0)
                return new GreenAssignmentNode(this.Interval, (GreenExpressionNode) newChild, Value);
            else if(index == 1)
                return new GreenAssignmentNode(this.Interval, Target, (GreenExpressionNode) newChild);

            throw new IndexOutOfRangeException();
        }
    }
}
