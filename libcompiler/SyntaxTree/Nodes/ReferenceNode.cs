using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ReferenceNode : ExpressionNode
    {
        private ExpressionNode _target;
        public ExpressionNode Target => GetRed(ref _target, 0);

        public ReferenceNode(CrawlSyntaxNode parent, Internal.ExpressionNode self, int slot) : base(parent, self, slot)
        {
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            if (index == 0) return Target;
            return default(CrawlSyntaxNode);
        }
    }
}
