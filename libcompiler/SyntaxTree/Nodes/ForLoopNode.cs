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

        public ForLoopNode(CrawlSyntaxNode parent, Internal.FlowNode self, int slot) : base(parent, self, slot) 
        {
            
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}