namespace libcompiler.SyntaxTree.Nodes
{
    public class TypeNode : CrawlSyntaxNode
    {
        public TypeNode(CrawlSyntaxNode parrent, Internal.TypeNode self, int slot) : base(parrent, self, slot)
        {
            ExportedType = self.ExportedType;
        }

        public CrawlType ExportedType { get; }
    }
}