using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenImportNode : GreenCrawlSyntaxNode
    {
        public string Module { get; }

        public GreenImportNode(Interval interval, string module)
            : base(NodeType.Import, interval)
        {
            Module = module;
            ChildCount = 0;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.ImportNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}