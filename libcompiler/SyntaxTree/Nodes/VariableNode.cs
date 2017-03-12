using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class VariableNode : ExpressionNode
    {
        public string Name { get; }

        public VariableNode(CrawlSyntaxNode parrent, Internal.VariableNode self, int slot) : base(parrent, self, slot)
        {
            Name = self.Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override CrawlSyntaxNode GetChild(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}