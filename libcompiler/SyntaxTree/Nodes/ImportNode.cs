namespace libcompiler.SyntaxTree.Nodes
{
    public class ImportNode : CrawlSyntaxNode
    {
        public string Module { get; }

        public ImportNode(CrawlSyntaxNode parent, Internal.ImportNode self, int slot) : base(parent, self, slot)
        {
            Module = self.Module;
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }
    }
}