using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ClassDeclerationNode : DeclerationNode
    {
        private TokenNode _identifier;
        private BlockNode _body;

        //TODO: List of constructors? Probably as extension method to not calculate unless required

        public TokenNode Identifier => GetRed(ref _identifier, 0);
        public BlockNode BodyBlock => GetRed(ref _body, 1);
        

        public ClassDeclerationNode(CrawlSyntaxNode parent, GreenNode self, int slot) : base(parent, self, slot)
           
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}