using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    
    public class BlockNode : ListNode<CrawlSyntaxNode>
    {   
        public BlockNode(Interval interval, IEnumerable<GreenNode> children) : base(interval, children)
        {
            
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.BlockNode(parrent, this, slot);
        }
    }
}