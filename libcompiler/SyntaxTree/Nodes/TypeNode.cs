using System;
using libcompiler.TypeSystem;

namespace libcompiler.SyntaxTree.Nodes
{
    public class TypeNode : CrawlSyntaxNode
    {
        public TypeNode(CrawlSyntaxNode parent, Internal.GreenTypeNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            TypeName = self.TypeName;
            _type = new Lazy<CrawlType>(() => CrawlType.ParseDecleration(this, TypeName));
        }

        private readonly Lazy<CrawlType> _type;

        public CrawlType ExportedType => _type.Value;

        public string TypeName { get; }

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