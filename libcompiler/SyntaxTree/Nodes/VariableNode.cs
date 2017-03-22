namespace libcompiler.SyntaxTree.Nodes
{
    public class VariableNode : ExpressionNode
    {
        public string Name { get; }

        public VariableNode(CrawlSyntaxNode parent, Internal.VariableNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            Name = self.Name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }
    }
}