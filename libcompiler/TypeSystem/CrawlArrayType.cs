using System.Linq;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.TypeSystem
{
    public class CrawlArrayType : CrawlType
    {
        private readonly SingleVariableDecleration _decleration;

        public int Dimensions { get; }
        public CrawlType ArrayElementType { get; }

        public CrawlArrayType(SingleVariableDecleration declaration, int dimensions, CrawlType arrayElementType)
            : base(declaration.Identifier.Name, declaration.FindNameSpace().Package)
        {
            _decleration = declaration;
            Dimensions = dimensions;
            ArrayElementType = arrayElementType;
        }

        public override bool IsAssignableTo(CrawlType target)
        {
            CrawlArrayType t = target as CrawlArrayType;
            if (t != null &&
                t.Dimensions == Dimensions &&
                t.ArrayElementType.IsAssignableTo(ArrayElementType)
            )
                return true;
            else
                return false;
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            CrawlArrayType t = target as CrawlArrayType;
            if (t != null &&
                t.Dimensions == Dimensions &&
                t.ArrayElementType.ImplicitlyCastableTo(ArrayElementType) )
                return true;
            else
                return false;
        }

        public override bool CastableTo(CrawlType target)
        {
            CrawlArrayType t = target as CrawlArrayType;
            if (t != null &&
                t.Dimensions == Dimensions &&
                t.ArrayElementType.CastableTo(ArrayElementType) )
                return true;
            else
                return false;
        }
    }
}