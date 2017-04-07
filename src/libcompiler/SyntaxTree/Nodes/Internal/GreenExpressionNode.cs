using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class GreenExpressionNode : GreenCrawlSyntaxNode
    {
        public ExpressionType ExpressionType { get; }

        protected GreenExpressionNode(Interval interval, NodeType type, ExpressionType expressionType)
            : base(type, interval)
        {
            ExpressionType = expressionType;
        }
    }
}