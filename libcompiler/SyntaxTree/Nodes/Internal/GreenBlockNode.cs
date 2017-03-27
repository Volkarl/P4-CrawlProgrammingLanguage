using System.Collections.Generic;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    
    public class GreenBlockNode : GreenListNode<CrawlSyntaxNode>
    {   
        public GreenBlockNode(Interval interval, IEnumerable<GreenCrawlSyntaxNode> children) : base(interval, children)
        {
            
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.BlockNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            GreenCrawlSyntaxNode[] newArray = ChildCopy();

            newArray[index] = newChild;

            return new GreenBlockNode(Interval, newArray);
        }
    }
}