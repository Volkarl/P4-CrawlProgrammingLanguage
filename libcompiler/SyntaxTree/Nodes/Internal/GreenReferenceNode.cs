using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenReferenceNode : GreenExpressionNode
    {
        public GreenExpressionNode Target { get; }

        public GreenReferenceNode(GreenExpressionNode target) : base(target.Interval, NodeType.Reference, target.ExpressionType)
        {
            Target = target;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            if (slot == 0) return Target;
            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.ReferenceNode(parent, this, slot);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            if (index == 0) return new GreenReferenceNode((GreenExpressionNode) newChild);
            throw new ArgumentOutOfRangeException();
        }
    }
}
