using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class VariableDeclerationNode : DeclerationNode
    {
        private TypeNode _declType;
        private ListNode<SingleVariableDecleration> _decls;

        public TypeNode DeclerationType => GetRed(ref _declType, 0);
        public ListNode<SingleVariableDecleration> Declerations => GetRed(ref _decls, 1);

        public VariableDeclerationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return DeclerationType;
                case 1: return Declerations;
                default: return default(CrawlSyntaxNode);
            }
        }
    }
}