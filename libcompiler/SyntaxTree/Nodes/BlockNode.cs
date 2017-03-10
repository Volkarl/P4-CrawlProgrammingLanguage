using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class BlockNode : ListNode<CrawlSyntaxNode>
    {
        //TODO: Probably some kind of (generated) Scope information here
        
        public BlockNode(CrawlSyntaxNode parrent, GreenNode self, int slot)
            : base(parrent, self, slot)
        {
        }
    }
}