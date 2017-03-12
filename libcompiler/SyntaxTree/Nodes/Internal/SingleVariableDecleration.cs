using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class SingleVariableDecleration : GreenNode
    {
        public VariableNode Identifier { get; }
        public ExpressionNode DefaultValue { get; }

        public SingleVariableDecleration(Interval interval, VariableNode identifier, ExpressionNode defaultValue = null) : base(NodeType.VariableDeclerationSingle, interval)
        {
            Identifier = identifier;
            DefaultValue = defaultValue;
        }

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return Identifier;
                case 1: return DefaultValue;

                default:
                    return default(GreenNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.SingleVariableDecleration(parrent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new System.NotImplementedException();
        }
    }
}