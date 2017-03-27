using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class GenericParameterNode : IdentifierNode
    {
        public string Limitation { get; }


        public GenericParameterNode(CrawlSyntaxNode parent, GreenIdentifierNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            Limitation = ((GreenGenericParameterNode) Green).Limitation;
        }
        
        public override string ToString()
        {
            return $"{Value} : {Limitation}";
        }
    }
}