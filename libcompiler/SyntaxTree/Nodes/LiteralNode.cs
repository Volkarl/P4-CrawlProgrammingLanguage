using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class LiteralNode : ExpressionNode
    {
        public string Value { get; }
        public LiteralType Type { get; }

        public LiteralNode(CrawlSyntaxTree owningTree, Interval interval, string value, LiteralType type)
            : base(owningTree, interval, NodeType.Literal)
        {
            Value = value;
            Type = type;
        }

        public enum LiteralType
        {
            String,
            Int,
            Float,
            Boolean,
            Real
        }

        public override string ToString()
        {
            return Value;
        }
    }
}