using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenNameSpaceNode : GreenImportNode
    {
        private const NodeType THE_TYPE_OF_THIS_NODE = NodeType.NameSpace;
        public GreenNameSpaceNode(Interval interval, string package) : base(interval, package)
        {
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.NameSpaceNode(parent, this, indexInParent);
        }
    }
}
