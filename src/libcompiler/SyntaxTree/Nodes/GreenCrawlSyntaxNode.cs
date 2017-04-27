using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree
{
    public abstract partial class CrawlSyntaxNode
    {
        public abstract class GreenCrawlSyntaxNode
        {
            public NodeType Type { get; }
            public Interval Interval { get; }

            public int ChildCount { get; protected set; } = 2;

            internal abstract GreenCrawlSyntaxNode GetChildAt(int slot);

            /// <summary>
            /// Create new red representation of node with specified parent.
            /// </summary>
            internal abstract CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent);

            protected GreenCrawlSyntaxNode(NodeType type, Interval interval)
            {
                Type = type;
                Interval = interval;
            }

            internal abstract GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index);
        }
    }
}
