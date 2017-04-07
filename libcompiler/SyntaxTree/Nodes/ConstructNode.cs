using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ConstructNode : CallableDeclarationNode
    {
        public ConstructNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
                        
        }


        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0:
                    throw new NotImplementedException();
                case 1:
                    throw new NotImplementedException();
                case 2:
                    return BodyBlock;
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}
