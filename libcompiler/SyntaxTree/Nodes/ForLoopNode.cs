using System.Runtime.InteropServices;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

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

        public ForLoopNode(CrawlSyntaxNode parrent, Internal.FlowNode self, int slot) : base(parrent, self, slot) 
        {
            
        }

        public override CrawlSyntaxNode GetChild(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}