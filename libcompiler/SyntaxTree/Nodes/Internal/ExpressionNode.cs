using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class ExpressionNode : GreenNode
    {
        public ExpressionType ExpressionType { get; }

        protected ExpressionNode(Interval interval, NodeType type, ExpressionType expressionType)
            : base(type, interval)
        {
            ExpressionType = expressionType;
        }
    }
}