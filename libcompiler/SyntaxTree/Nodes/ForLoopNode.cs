using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ForLoopNode : FlowNode
    {
        public CrawlType InducedFieldType { get; }
        public string InducedFieldName { get; }
        public ExpressionNode Iteratior { get; }
        public BlockNode Block { get; }

        public ForLoopNode(
            CrawlType type,
            string inducedField,
            ExpressionNode iteratior,
            BlockNode block,
            Interval interval,
            CrawlSyntaxTree owningTree)
            : base(
                owningTree,
                NodeType.Forloop,
                interval)
        {
            InducedFieldType = type;
            InducedFieldName = inducedField;
            Iteratior = iteratior;
            Block = block;
        }
    }
}