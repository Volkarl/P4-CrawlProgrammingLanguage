using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class MultiChildExpressionNode : ExpressionNode
    {
        public ExpressionType ExpressionType { get; }
        public IReadOnlyCollection<ExpressionNode> Arguments { get; }

        public MultiChildExpressionNode(CrawlSyntaxTree owningTree, Interval interval, ExpressionType type,
            IEnumerable<ExpressionNode> children) : base(owningTree, interval, NodeType.MultiExpression)
        {
            ExpressionType = type;
            Arguments = children.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }
    }
}