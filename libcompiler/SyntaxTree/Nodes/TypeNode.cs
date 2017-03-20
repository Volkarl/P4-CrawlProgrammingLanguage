namespace libcompiler.SyntaxTree.Nodes
{
    public class TypeNode : CrawlSyntaxNode
    {
        public TypeNode(CrawlSyntaxNode parent, Internal.TypeNode self, int slot) : base(parent, self, slot)
        {
            ExportedType = self.ExportedType;
        }

        public CrawlType ExportedType { get; }
        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }
    }
}