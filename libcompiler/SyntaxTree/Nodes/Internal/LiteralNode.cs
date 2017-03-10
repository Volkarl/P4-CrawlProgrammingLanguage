using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class LiteralNode : ExpressionNode
    {
        public string Value { get; }
        public LiteralType LiteralType { get; }

        public LiteralNode(Interval interval, string value, LiteralType literalType)
            : base(interval, NodeType.Literal, ExpressionType.Constant)
        {
            Value = value;
            LiteralType = literalType;
        }

        

        public override string ToString()
        {
            return Value;
        }

        public override GreenNode GetSlot(int slot)
        {
            throw new System.NotImplementedException();
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.LiteralNode(parrent, this, slot);
        }
    }
}