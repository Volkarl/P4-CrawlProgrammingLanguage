using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class MethodDeclerationNode : DeclerationNode, INodeThatTakesGenericParameters
    {
        private TypeNode _methodSignature;
        private ListNode<IdentifierNode> _parameterIdentifiers;
        private ListNode<GenericParameterNode> _genericParameters;
        private VariableNode _identifier;
        private BlockNode _body;

        public TypeNode MethodSignature => GetRed(ref _methodSignature, 0);
        public ListNode<IdentifierNode> ParameterIdentifiers => GetRed(ref _parameterIdentifiers, 1);
        public ListNode<GenericParameterNode> GenericParameters => GetRed(ref _genericParameters, 2);
        public VariableNode Identfier => GetRed(ref _identifier, 3);
        public BlockNode Body => GetRed(ref _body, 4);

        public MethodDeclerationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0:
                    return MethodSignature;
                case 1:
                    return ParameterIdentifiers;
                case 2:
                    return GenericParameters;
                case 3:
                    return Identfier;
                case 4:
                    return Body;
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}