using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// Represents a block of code, surrounded by Indent .. Dedent
    /// This would be one possible place to store scope information.
    /// </summary>
    public class BlockNode : ListNode<CrawlSyntaxNode>
    {
        //TODO: Probably some kind of (generated) Scope information here
        
        public BlockNode(CrawlSyntaxNode parent, GreenNode self, int slot)
            : base(parent, self, slot)
        {
        }
    }
}