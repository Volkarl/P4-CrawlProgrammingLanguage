using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    class AssignmentNode : GreenNode
    {
        public ExpressionNode Target { get; }
        public ExpressionNode Value { get; }

        public AssignmentNode(Interval interval, ExpressionNode target,
            ExpressionNode block) : base(NodeType.Assignment, interval)
        {
            Target = target;
            Value = block;
        }

        public override GreenNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return Target;
                case 1: return Value;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.AssignmentNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if(index == 0)
                return new AssignmentNode(this.Interval, (ExpressionNode) newChild, Value);
            else if(index == 1)
                return new AssignmentNode(this.Interval, Target, (ExpressionNode) newChild);

            throw new IndexOutOfRangeException();
        }
    }
}
