using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class CrawlSyntaxNode
    {
        public CrawlSyntaxTree OwningTree { get; }
        public NodeType Type { get; }
        public Interval CodeInterval { get; }

        protected CrawlSyntaxNode(CrawlSyntaxTree owningTree, NodeType type, Interval codeInterval)
        {
            OwningTree = owningTree;
            Type = type;
            CodeInterval = codeInterval;
        }
    }
}