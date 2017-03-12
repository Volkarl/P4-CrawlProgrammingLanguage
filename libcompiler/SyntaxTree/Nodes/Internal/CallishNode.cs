using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    class CallishNode : ExpressionNode
    {
        public ExpressionNode Target { get; }
        public ListNode<Nodes.ExpressionNode> Arguments { get; }


        public CallishNode(Interval interval, ExpressionNode target, ListNode<Nodes.ExpressionNode> arguments, ExpressionType expressionType) : base(interval, MakeNodeType(expressionType), expressionType)

        {
                Target = target;
                Arguments = arguments;
            }

        private static NodeType MakeNodeType(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Index:
                    return NodeType.Index;
                case ExpressionType.Invocation:
                    return NodeType.Call;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return Target;
                case 1: return Arguments;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.CallishNode(parrent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if(index == 0)
                return new CallishNode(this.Interval, (ExpressionNode)newChild, Arguments, ExpressionType);
            else if(index == 1)
                return new CallishNode(this.Interval, Target, (ListNode<Nodes.ExpressionNode>)newChild, ExpressionType);

            throw new ArgumentOutOfRangeException();
        }
    }
}
