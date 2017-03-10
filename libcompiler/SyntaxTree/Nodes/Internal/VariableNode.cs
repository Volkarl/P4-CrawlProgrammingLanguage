using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class VariableNode : ExpressionNode
    {
        public string Name { get; }

        public VariableNode(Interval interval, string name)
            : base(interval, NodeType.Variable, SyntaxTree.ExpressionType.Variable)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override GreenNode GetSlot(int slot)
        {
            throw new System.NotImplementedException();
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.VariableNode(parrent, this, slot);
        }
    }
}