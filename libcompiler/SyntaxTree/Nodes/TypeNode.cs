using libcompiler.TypeSystem;

namespace libcompiler.SyntaxTree.Nodes
{
    public class TypeNode : CrawlSyntaxNode
    {
        public TypeNode(CrawlSyntaxNode parent, Internal.GreenTypeNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            ExportedType = self.ExportedType;
        }

        public CrawlType ExportedType { get; }
        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }

        public override string ToString()
        {
            return ExportedType.ToString();
        }
    }
}