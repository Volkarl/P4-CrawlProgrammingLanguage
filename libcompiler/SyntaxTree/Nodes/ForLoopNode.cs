namespace libcompiler.SyntaxTree.Nodes
{
    public class ForLoopNode : FlowNode
    {
        private TypeNode _type;
        private VariableNode _field;
        private ExpressionNode _iterator;
        private BlockNode _block;

        public TypeNode InducedFieldType => GetRed(ref _type, 0);
        public VariableNode InducedFieldName => GetRed(ref _field, 1);
        public ExpressionNode Iteratior => GetRed(ref _iterator, 2);
        public BlockNode Block => GetRed(ref _block, 3);

        public ForLoopNode(CrawlSyntaxNode parent, Internal.FlowNode self, int indexInParent) : base(parent, self, indexInParent) 
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return InducedFieldType;
                case 1: return InducedFieldName;;
                case 2: return Iteratior;
                case 3: return Block;
                default: return default(CrawlSyntaxNode);
            }
        }
    }
}