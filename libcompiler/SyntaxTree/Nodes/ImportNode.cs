namespace libcompiler.SyntaxTree.Nodes
{
    public class ImportNode : CrawlSyntaxNode
    {
        public string Module { get; }

        protected internal ImportNode(CrawlSyntaxNode parent, Internal.GreenImportNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            Module = self.Module;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }

        public override string ToString()
        {
            return "Import: " + Module;
        }
    }
}