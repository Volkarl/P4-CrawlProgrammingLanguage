using System;
using System.Linq;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;

namespace libcompiler.TypeSystem
{
    public class CrawlArrayType : CrawlType
    {
        public int Rank { get; }
        public CrawlType ElementType { get; }

        public CrawlArrayType(int rank, CrawlType elementType)
            : base(elementType.Identifier + "[" + new string(',', rank - 1) + "]", elementType.Namespace)
        {
            Rank = rank;
            ElementType = elementType;
        }

        public override bool IsAssignableTo(CrawlType target)
        {
            //Arrays cannot be cast up or down
            //Array of Animals, Containing a Tiger, assigned to an array of Dogs?
            //Array of Dogs, assigned to array of Animal, then a Tiger is inserted?

            CrawlArrayType t = target as CrawlArrayType;
            return t != null &&
                   t.Rank == Rank &&
                   t.ElementType.Equals(ElementType);
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

        public override string ToString()
        {
            return $"{ElementType}[{new String(',', Rank -1 )}]";
        }
    }
}