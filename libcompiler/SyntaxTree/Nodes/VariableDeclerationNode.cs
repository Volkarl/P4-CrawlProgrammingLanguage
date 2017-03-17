using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class VariableDeclerationNode : DeclerationNode
    {
        private TypeNode _declType;
        private ListNode<SingleVariableDecleration> _decls;
        public TypeNode DeclerationType => GetRed(ref _declType, 0);
        public ListNode<SingleVariableDecleration> Declerations => GetRed(ref _decls, 1);

        public VariableDeclerationNode(CrawlSyntaxNode parent, GreenNode self, int slot) : base(parent, self, slot)
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}