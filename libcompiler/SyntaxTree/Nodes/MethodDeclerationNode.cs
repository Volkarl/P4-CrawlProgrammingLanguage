using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class MethodDeclerationNode : CallableDeclarationNode, INodeThatTakesGenericParameters
    {
        
        private ListNode<GenericParameterNode> _genericParameters;

        public MethodDeclerationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) 
            : base(parent, self, indexInParent)
        {
        }

        public ListNode<GenericParameterNode> GenericParameters => GetRed(ref _genericParameters, 2);
       

       

        public override string ToString()
        {
            return $"decl {FunctionType.ExportedType.Textdef} {Identfier} =";
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0:
                    return FunctionType;
                case 1:
                    return Identfier;
                case 2:
                    return GenericParameters;
                case 3:
                    return BodyBlock;
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}