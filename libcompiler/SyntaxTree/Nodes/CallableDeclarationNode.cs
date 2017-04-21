using libcompiler.SyntaxTree.Nodes.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class CallableDeclarationNode: DeclerationNode
    {

        private TypeNode _type;
        private IdentifierNode _id;
        private BlockNode _body;

        //TODO: Needs to save parameters' identifiers.
        public TypeNode MethodType => GetRed(ref _type, 0);
        public IdentifierNode Identfier => GetRed(ref _id, 1);
        public BlockNode BodyBlock => GetRed(ref _body, 2);



        protected CallableDeclarationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {

        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0:
                    return MethodType;
                case 1:
                    return Identfier;
                case 2:
                    return BodyBlock;
                default:
                    return default(CrawlSyntaxNode);
            }
        }









    }
}
