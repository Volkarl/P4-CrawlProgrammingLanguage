using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class ClassDeclerationNode : DeclerationNode
    {
        //TODO: List of constructors? Probably as extension method to not calculate unless required

            //This could probably be turned into a BinaryNode, but class is likely to get more information thrown at it
        public TokenNode Identifier { get; }
        public BlockNode BodyBlock { get; }

        public ClassDeclerationNode(Interval interval, ProtectionLevel protectionLevel, TokenNode identifier, BlockNode bodyBlock) : base(interval, NodeType.ClassDecleration, protectionLevel)
            
        {
            Identifier = identifier;
            BodyBlock = bodyBlock;
        }

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return Identifier;
                case 1: return BodyBlock;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.ClassDeclerationNode(parrent, this, slot);
        }
    }
}