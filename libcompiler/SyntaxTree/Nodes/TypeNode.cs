namespace libcompiler.SyntaxTree.Nodes
{
    public class TypeNode : CrawlSyntaxNode
    {
        public TypeNode(CrawlSyntaxNode parent, Internal.GreenTypeNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            ExportedType = self.ExportedType;
            IsReference = self.IsReference;
        }

        public CrawlType ExportedType { get; }
        public bool IsReference { get; }
        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return default(CrawlSyntaxNode);
        }

        public override string ToString()
        {
            string result;
            if (IsReference)
                result = $"ref {ExportedType.ToString()}";
            else
                result = $"{ExportedType.ToString()}";

            return result;
        }
    }
}