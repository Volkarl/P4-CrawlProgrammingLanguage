using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class ReferenceNode : ExpressionNode
    {
        public ExpressionNode Target { get; }

        public ReferenceNode(ExpressionNode target) : base(target.Interval, NodeType.Reference, target.ExpressionType)
        {
            Target = target;
        }

        public override GreenNode GetChildAt(int slot)
        {
            if (slot == 0) return Target;
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.ReferenceNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if (index == 0) return new ReferenceNode((ExpressionNode) newChild);
            throw new ArgumentOutOfRangeException();
        }
    }
}
