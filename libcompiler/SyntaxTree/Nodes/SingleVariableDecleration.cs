using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class SingleVariableDecleration : CrawlSyntaxNode
    {
        private VariableNode _id;
        private ExpressionNode _default;


        public VariableNode Identifier => GetRed(ref _id, 0);
        public ExpressionNode DefaultValue => GetRed(ref _default, 1);

        public SingleVariableDecleration(CrawlSyntaxNode parrent, GreenNode self, int slot) : base(parrent, self, slot)
        {
            
        }
    }
}