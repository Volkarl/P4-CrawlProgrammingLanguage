using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ImportNode : CrawlSyntaxNode
    {
        public string Module { get; }

        public ImportNode(CrawlSyntaxTree owningTree, Interval interval, string module)
            : base(owningTree, NodeType.Import, interval)
        {
            Module = module;
        }
    }
}