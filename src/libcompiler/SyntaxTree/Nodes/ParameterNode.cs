using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ParameterNode : CrawlSyntaxNode
    {
        private TypeNode _parameterType;

        public bool Reference { get; }
        public TypeNode ParameterType => GetRed(ref _parameterType, 0);
        public string Identifier { get; }


        public ParameterNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            GreenParameterNode specificSelf = (GreenParameterNode) self;
            Reference = specificSelf.Reference;
            Identifier = specificSelf.Identifier;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            if (index == 0)
                return ParameterType;

            return null;
        }

        public override string ToString()
        {
            return (Reference ? "ref ": "") + Identifier;
        }
    }
}
