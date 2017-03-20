using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class DeclerationNode : CrawlSyntaxNode
    {
        public ProtectionLevel ProtectionLevel { get; }

        protected DeclerationNode(CrawlSyntaxNode parent, GreenNode self, int slot) : base(parent, self, slot)
        {
            Internal.DeclerationNode decl = ((Internal.DeclerationNode) self);
            ProtectionLevel = decl.ProtectionLevel;
        }

    }
}