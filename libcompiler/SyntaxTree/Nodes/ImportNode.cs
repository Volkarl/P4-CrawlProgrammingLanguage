namespace libcompiler.SyntaxTree.Nodes
{
    public class ImportNode : CrawlSyntaxNode
    {
        public string Package { get; }

        protected internal ImportNode(CrawlSyntaxNode parent, Internal.GreenImportNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            Package = self.Package;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }

        public override string ToString()
        {
            return "Import: " + Package;
        }
    }
}