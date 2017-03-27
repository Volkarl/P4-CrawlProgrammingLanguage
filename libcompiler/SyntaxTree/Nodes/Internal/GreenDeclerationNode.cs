using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class GreenDeclerationNode : GreenCrawlSyntaxNode
    {
        public ProtectionLevel ProtectionLevel { get; }

        protected GreenDeclerationNode(Interval interval, NodeType type, ProtectionLevel protectionLevel) : base(type, interval)
        {
            ProtectionLevel = protectionLevel;
        }

    }
}