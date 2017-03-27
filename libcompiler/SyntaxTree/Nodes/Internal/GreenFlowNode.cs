using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class GreenFlowNode : GreenCrawlSyntaxNode
    {
        protected GreenFlowNode(NodeType type, Interval interval)
            : base(type, interval)
        {
        }
    }
}