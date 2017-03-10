using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class FlowNode : CrawlSyntaxNode
    {
        protected FlowNode(CrawlSyntaxNode parrent, Internal.FlowNode self, int slot) : base(parrent, self, slot)
        {
        }
    }
}