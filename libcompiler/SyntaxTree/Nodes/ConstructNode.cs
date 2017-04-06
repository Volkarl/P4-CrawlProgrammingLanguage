using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ConstructNode : CrawlSyntaxNode
    {
        private ProtectionLevel Protectionlevel { get; }
        private TypeNode type;
        private BlockNode body;


        public ProtectionLevel Acceslevel => GetRed(ref Acceslevel);
        public TypeNode ConstructorType => GetRed(ref type, 0);
        public BlockNode BlockBody => GetRed(ref body, 2);

        public ConstructNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {

        }
        public void Construct(TypeNode type, VariableNode vn, BlockNode body)
        {
            
        }

        //public ProtectionLevel


        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0:
                    return ConstructorType;
                case 1:
                    return Identifier;
                case 2:
                    return BlockBody;
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}
