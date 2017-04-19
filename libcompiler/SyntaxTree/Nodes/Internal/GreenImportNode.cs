using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenImportNode : GreenCrawlSyntaxNode
    {
        public string Package { get; }

        /// <summary> You see, GreenNameSpaceNode is just an abstraction of this node. It's quite identical except for it's advertised type, which is why we need to be able to set it outside the ctor.</summary>
        private const NodeType THE_TYPE_OF_THIS_NODE = NodeType.Import;

        public GreenImportNode(Interval interval, string package)
            : base(THE_TYPE_OF_THIS_NODE, interval)
        {
            Package = package;
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