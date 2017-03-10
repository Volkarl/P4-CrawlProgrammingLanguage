using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return Target;
                case 1: return Value;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.AssignmentNode(parrent, this, slot);
        }
    }
}
