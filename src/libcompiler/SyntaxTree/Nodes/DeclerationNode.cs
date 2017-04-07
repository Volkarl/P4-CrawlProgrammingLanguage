using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class DeclerationNode : CrawlSyntaxNode
    {
        public ProtectionLevel ProtectionLevel { get; }

        protected DeclerationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            Internal.GreenDeclerationNode decl = ((Internal.GreenDeclerationNode) self);
            ProtectionLevel = decl.ProtectionLevel;
        }

    }
}