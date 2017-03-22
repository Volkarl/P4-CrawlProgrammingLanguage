using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class GreenNode
    {
        public NodeType Type { get; }
        public Interval Interval { get; }

        public int ChildCount { get; protected set; } = 2;

        public abstract GreenNode GetChildAt(int slot);

        /// <summary>
        /// Create new red representation of node with specified parent.
        /// </summary>
        public abstract CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent);

        protected GreenNode(NodeType type, Interval interval)
        {
            Type = type;
            Interval = interval;
        }

        internal abstract GreenNode WithReplacedChild(GreenNode newChild, int index);
    }
}
