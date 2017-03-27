using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    class GreenCallishNode : GreenExpressionNode
    {
        public GreenExpressionNode Target { get; }
        public GreenListNode<Nodes.ExpressionNode> Arguments { get; }


        public GreenCallishNode(Interval interval, GreenExpressionNode target, GreenListNode<Nodes.ExpressionNode> arguments,
            ExpressionType expressionType) : base(interval, MakeNodeType(expressionType), expressionType)
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

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return Target;
                case 1: return Arguments;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.CallishNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            if(index == 0)
                return new GreenCallishNode(this.Interval, (GreenExpressionNode)newChild, Arguments, ExpressionType);
            else if(index == 1)
                return new GreenCallishNode(this.Interval, Target, (GreenListNode<Nodes.ExpressionNode>)newChild, ExpressionType);

            throw new ArgumentOutOfRangeException();
        }
    }
}
