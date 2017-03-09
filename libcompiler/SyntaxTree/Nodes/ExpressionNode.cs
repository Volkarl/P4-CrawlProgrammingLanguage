using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class ExpressionNode : CrawlSyntaxNode
    {
        protected ExpressionNode(CrawlSyntaxTree owningTree, Interval interval, NodeType type)
            : base(owningTree, type, interval)
        {
        }
    }
}