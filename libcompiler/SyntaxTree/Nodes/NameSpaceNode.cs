using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class NameSpaceNode : ImportNode
    {
        protected internal NameSpaceNode(CrawlSyntaxNode parent, GreenNameSpaceNode self, int indexInParent) : base(parent, self, indexInParent)
        {
        }

        public override string ToString()
        {
            return "Namespace: " + Package;
        }
    }
}
