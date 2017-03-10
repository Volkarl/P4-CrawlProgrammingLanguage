using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class ForLoopNode : FlowNode
    {
        private TypeNode InducedFieldType { get; }
        private VariableNode InducedFieldName { get; }
        private ExpressionNode Iteratior { get; }
        private BlockNode Block { get;  }

        public ForLoopNode(Interval interval, TypeNode inducedFieldType, VariableNode inducedFieldName, ExpressionNode iteratior, BlockNode block)
            : base(NodeType.Forloop, interval )
        {
            InducedFieldType = inducedFieldType;
            InducedFieldName = inducedFieldName;
            Iteratior = iteratior;
            Block = block;
        }

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return InducedFieldType;
                case 1: return InducedFieldName;
                case 2: return Iteratior;
                case 3: return Block;

                default:
                    return default(GreenNode);
            }

            
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.ForLoopNode(parrent, this, slot);
        }
    }
}