using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class DeclerationNode : CrawlSyntaxNode
    {
        public ProtectionLevel ProtectionLevel { get; }

        protected DeclerationNode(CrawlSyntaxTree owningTree, Interval interval, NodeType type,
            ProtectionLevel protectionLevel) : base(owningTree, type, interval)
        {
            ProtectionLevel = protectionLevel;
        }

    }
}