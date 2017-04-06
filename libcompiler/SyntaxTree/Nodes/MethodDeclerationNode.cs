using libcompiler.SyntaxTree.Nodes.Internal;
using libcompiler.TypeChecker;

namespace libcompiler.SyntaxTree.Nodes
{
    public class MethodDeclerationNode : DeclerationNode, INodeThatTakesGenericParameters, IScope
    {
        private MethodScope _scope;

        private TypeNode _type;
        private VariableNode _id;
        private ListNode<ParameterNode> _parameters;
        private ListNode<GenericParameterNode> _genericParameters;
        private BlockNode _body;

        //TODO: Needs to save parameters' identifiers.
        public TypeNode FunctionType => GetRed(ref _type, 0);
        public VariableNode Identfier => GetRed(ref _id, 1);
        public ListNode<ParameterNode> Parameters => GetRed(ref _parameters, 2);
        public ListNode<GenericParameterNode> GenericParameters => GetRed(ref _genericParameters, 3);
        public BlockNode BodyBlock => GetRed(ref _body, 4);

        public MethodDeclerationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            _scope = new MethodScope(this);
        }

        public override string ToString()
        {
            return $"decl {FunctionType.ExportedType} {Identfier} =";
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

        public TypeInformation[] GetScope(string identifier)
        {
            return  _scope.GetScope(identifier) ?? Parent.FindFirstScope()?.GetScope(identifier);
        }
    }
}