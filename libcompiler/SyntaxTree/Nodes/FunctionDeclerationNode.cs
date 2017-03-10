using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class FunctionDeclerationNode : DeclerationNode
    {
        private TypeNode _type;
        private VariableNode _id;
        private BlockNode _body;

        //TODO: do something about parameters
        public TypeNode FunctionType => GetRed(ref _type, 0);
        public VariableNode Identfier => GetRed(ref _id, 1);
        public BlockNode BodyBlock => GetRed(ref _body, 2);

        public FunctionDeclerationNode(CrawlSyntaxNode parrent, GreenNode self, int slot) : base(parrent, self, slot)
        {
            
        }

        public override string ToString()
        {
            return $"decl {FunctionType.ExportedType.Textdef} {Identfier} =";
        }
    }
}