namespace libcompiler.SyntaxTree.Nodes
{
    public class TypeNode : CrawlSyntaxNode
    {
        public TypeNode(CrawlSyntaxNode parent, Internal.GreenTypeNode self, int indexInParent)
            : base(parent, self, indexInParent)
        {
            TypeTextDefinition = self.TypeTextDefinition;
            ExportedType = self.ExportedType;
            IsReference = self.IsReference;
        }

        public string TypeTextDefinition { get; }
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
                result = $"ref {ExportedType?.ToString() ?? TypeTextDefinition}";
            else
                result = $"{ExportedType?.ToString() ?? TypeTextDefinition}";

            return result;
        }
    }
}