using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class BlockNode : ListNode<CrawlSyntaxNode>
    {
        //TODO: Probably some kind of (generated) Scope information here
        
        public BlockNode(CrawlSyntaxNode parent, GreenNode self, int slot)
            : base(parent, self, slot)
        {
        }
    }
}