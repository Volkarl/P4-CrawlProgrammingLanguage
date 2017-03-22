namespace libcompiler.SyntaxTree.Nodes
{
    public class TypeNode : CrawlSyntaxNode
    {
        public TypeNode(CrawlSyntaxNode parent, Internal.TypeNode self, int indexInParent) : base(parent, self, indexInParent)
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