using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class SelectiveFlowNode : FlowNode
    {
        public ExpressionNode Check { get; }
        public BlockNode Primary { get; }
        public BlockNode Alternative { get; }

        public SelectiveFlowNode(FlowType type, ExpressionNode check, BlockNode primary, BlockNode alternative,
            Interval interval, CrawlSyntaxTree owningTree) : base(owningTree, MakeNodeType(type), interval)
        {
            Check = check;
            Primary = primary;
            Alternative = alternative;
        }

        private static NodeType MakeNodeType(FlowType type)
        {
            switch (type)
            {
                case FlowType.If:
                    return NodeType.If;
                case FlowType.IfElse:
                    return NodeType.IfElse;
                case FlowType.While:
                    return NodeType.While;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public enum FlowType
        {
            If,
            IfElse,
            While,
        }
    }
}