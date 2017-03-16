using System;
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

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.BlockNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            GreenNode[] newArray = ChildCopy();

            newArray[index] = newChild;

            return new BlockNode(Interval, newArray);
        }
    }
}