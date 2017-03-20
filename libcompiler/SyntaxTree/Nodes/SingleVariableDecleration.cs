using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class SingleVariableDecleration : CrawlSyntaxNode
    {
        private VariableNode _id;
        private ExpressionNode _default;


        public VariableNode Identifier => GetRed(ref _id, 0);
        public ExpressionNode DefaultValue => GetRed(ref _default, 1);

        public SingleVariableDecleration(CrawlSyntaxNode parent, GreenNode self, int slot) : base(parent, self, slot)
        {
            
        }

        public override string ToString()
        {
            if (DefaultValue == null)
                return Identifier.Name;
            else return Identifier.Name + " = ";
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return Identifier;
                case 1: return DefaultValue;;
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}