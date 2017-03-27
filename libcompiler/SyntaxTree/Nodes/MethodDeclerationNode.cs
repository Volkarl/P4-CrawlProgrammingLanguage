using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class MethodDeclerationNode : DeclerationNode, INodeThatTakesGenericParameters
    {
        private TypeNode _type;
        private VariableNode _id;
        private ListNode<GenericParameterNode> _genericParameters;
        private BlockNode _body;

        //TODO: Needs to save parameters' identifiers.
        public TypeNode FunctionType => GetRed(ref _type, 0);
        public VariableNode Identfier => GetRed(ref _id, 1);
        public ListNode<GenericParameterNode> GenericParameters => GetRed(ref _genericParameters, 2);
        public BlockNode BodyBlock => GetRed(ref _body, 3);

        public MethodDeclerationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            
        }

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