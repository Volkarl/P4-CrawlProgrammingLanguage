using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenMultiChildExpressionNode : GreenExpressionNode
    {
        public GreenListNode<Nodes.ExpressionNode> Arguments { get; }

        public GreenMultiChildExpressionNode(Interval interval, ExpressionType expressionType, GreenListNode<Nodes.ExpressionNode> arguments) : base(interval, NodeType.MultiExpression, expressionType)
        {
            Arguments = arguments;
            ChildCount = 1;
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            if (slot == 0) return Arguments;

            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.MultiChildExpressionNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            if(index == 0)
                return new GreenMultiChildExpressionNode(this.Interval, ExpressionType, (GreenListNode<Nodes.ExpressionNode>) newChild);

            throw new ArgumentOutOfRangeException();
        }
    }
}