using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class MethodDeclerationNode : CallableDeclarationNode, INodeThatTakesGenericParameters
    {
        private ListNode<IdentifierNode> _parameterIdentifiers;
        private ListNode<GenericParameterNode> _genericParameters;
        

        public ListNode<IdentifierNode> ParameterIdentifiers => GetRed(ref _parameterIdentifiers, 3);
        public ListNode<GenericParameterNode> GenericParameters => GetRed(ref _genericParameters, 4);
        

        public MethodDeclerationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        { }

        public override string ToString()
        {
            return $"decl {MethodType.ExportedType.Textdef} {Identfier} =";
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
                case 3:
                    return ParameterIdentifiers;
                case 4:
                    return GenericParameters;
                
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}