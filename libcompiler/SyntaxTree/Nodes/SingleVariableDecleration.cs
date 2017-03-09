using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class SingleVariableDecleration : CrawlSyntaxNode
    {
        public string Identifier { get; }
        public Interval Interval { get; }
        public ExpressionNode DefaultValue { get; }

        public SingleVariableDecleration(CrawlSyntaxTree owningTree, string name, Interval interval,
            ExpressionNode defaultValue = null) : base(owningTree, NodeType.VariableDeclerationSingle, interval)
        {
            Identifier = name;
            Interval = interval;
            DefaultValue = defaultValue;
        }
    }
}