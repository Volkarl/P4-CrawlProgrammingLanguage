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
            //Arrays cannot be cast up or down
            //Array of Animals, Containing a Tiger, assigned to an array of Dogs?
            //Array of Dogs, assigned to array of Animal, then a Tiger is inserted?

            CrawlArrayType t = target as CrawlArrayType;
            return t != null &&
                   t.Dimensions == Dimensions &&
                   t.ArrayElementType.Equals(ArrayElementType);
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            //Casting an entire array in any way dosen't make sense. 
            //We could try, but it just leads to _weird_ erros when trying
            //to save something in a casted array.

            //CrawlArrayType t = target as CrawlArrayType;
            //if (t != null &&
            //    t.Dimensions == Dimensions &&
            //    t.ArrayElementType.ImplicitlyCastableTo(ArrayElementType) )
            //    return true;
            //else
            return false;
        }

        public override bool CastableTo(CrawlType target)
        {

            //CrawlArrayType t = target as CrawlArrayType;
            //if (t != null &&
            //    t.Dimensions == Dimensions &&
            //    t.ArrayElementType.CastableTo(ArrayElementType) )
            //    return true;
            //else
            return false;
        }
    }
}