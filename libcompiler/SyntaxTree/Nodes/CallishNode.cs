using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class CallishNode : ExpressionNode
    {
        public ExpressionNode Target { get; }
        public IReadOnlyCollection<ExpressionNode> Arguments { get; }

        public CallishNode(CrawlSyntaxTree owningTree, Interval interval, ExpressionNode target,
            IEnumerable<ExpressionNode> arguments, ExpressionType type)
            : base(owningTree, interval, MakeNodeType(type))
        {
            Target = target;
            Arguments = arguments.ToList().AsReadOnly();
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
    }
}