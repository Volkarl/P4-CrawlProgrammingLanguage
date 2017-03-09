using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class BlockNode : CrawlSyntaxNode
    {
        //TODO: Probably some kind of (generated) Scope information here
        public IReadOnlyCollection<CrawlSyntaxNode> Children { get; }

        public BlockNode(CrawlSyntaxTree owningTree, Interval interval, IEnumerable<CrawlSyntaxNode> children)
            : base(owningTree, NodeType.Block, interval)
        {
            Children = children.ToList().AsReadOnly();
        }
    }
}