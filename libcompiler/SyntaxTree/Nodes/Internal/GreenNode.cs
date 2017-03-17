using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class GreenNode
    {
        public NodeType Type { get; }
        public Interval Interval { get; }

        public int ChildCount { get; protected set; }

        public abstract GreenNode GetChildAt(int slot);
        public abstract CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot);

        protected GreenNode(NodeType type, Interval interval)
        {
            Type = type;
            Interval = interval;
        }

        internal abstract GreenNode WithReplacedChild(GreenNode newChild, int index);
    }
}
