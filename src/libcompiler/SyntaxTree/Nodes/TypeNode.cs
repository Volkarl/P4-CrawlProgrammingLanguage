using System;

namespace libcompiler.SyntaxTree
{
    public partial class TypeNode
    {
        private readonly Lazy<CrawlType> _type;

        /// <summary>
        /// The actual type, within context
        /// </summary>
        public CrawlType ExportedType => _type.Value;
    }
}