using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class VariableNode : ExpressionNode
    {
        public string Name { get; }

        public VariableNode(CrawlSyntaxTree owningTree, string name, Interval interval)
            : base(owningTree, interval, NodeType.Variable)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}