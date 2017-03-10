using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class DeclerationNode : CrawlSyntaxNode
    {
        public ProtectionLevel ProtectionLevel { get; }

        protected DeclerationNode(CrawlSyntaxNode parrent, GreenNode self, int slot) : base(parrent, self, slot)
        {
            Internal.DeclerationNode decl = ((Internal.DeclerationNode) self);
            ProtectionLevel = decl.ProtectionLevel;
        }

    }
}