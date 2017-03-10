using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class FunctionDeclerationNode : DeclerationNode
    {
        //TODO: do something about parameters
        public TypeNode FunctionType { get; }
        public VariableNode Identfier { get; }
        public BlockNode BodyBlock { get; }

        public FunctionDeclerationNode(Interval interval, ProtectionLevel protectionLevel, TypeNode functionType, VariableNode identfier, BlockNode bodyBlock)
            : base(interval, NodeType.FunctionDecleration, protectionLevel)
        {
            FunctionType = functionType;
            Identfier = identfier;
            BodyBlock = bodyBlock;
        }

        public override string ToString()
        {
            return $"decl {FunctionType.ExportedType.Textdef} {Identfier} =";
        }

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return FunctionType;
                case 1: return Identfier;
                case 2: return BodyBlock;
                default:
                    return default(GreenNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.FunctionDeclerationNode(parrent, this, slot);
        }
    }
}