using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class FlowNode : CrawlSyntaxNode
    {
        protected FlowNode(CrawlSyntaxTree owningTree, NodeType type, Interval interval)
            : base(owningTree, type, interval)
        {
        }
    }
}