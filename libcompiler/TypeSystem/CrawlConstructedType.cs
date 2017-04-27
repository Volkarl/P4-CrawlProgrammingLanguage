using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.TypeChecker;

namespace libcompiler.TypeSystem
{
    public class CrawlConstructedType : CrawlType, IScope
    {
        private readonly ClassDeclerationNode _declaration;

        protected CrawlConstructedType(ClassDeclerationNode declaration)
            : base(declaration.Identifier.Value, declaration.FindNameSpace().Package)
        {
            _declaration = declaration;

            //TODO Will nullref for classes inheriting from $OBJECT
            Ancestor = (CrawlConstructedType) declaration.FindFirstScope().FindSymbol(declaration.Ancestor.Value)[0].Type;
        }


        public CrawlConstructedType Ancestor { get; }
        //public IReadOnlyList<CrawlType> Interfaces { get; }


        public TypeInformation[] FindSymbol(string symbol)
        {
            return _declaration.FindSymbolOnlyInThisScope(symbol);
        }


        /// <summary>
        /// Checks if assigning this to target is legal(according to Cräwl specification).
        /// </summary>
        public override bool IsAssignableTo(CrawlType target)
        {
            //Are they equal?
            if (Equals(target))
                return true;

            //Is this a descendant of target?
            CrawlConstructedType ancestor = Ancestor;
            while (ancestor.Identifier != "$OBJECT")
            {
                if(ancestor.Equals(target))
                    return true;
                ancestor = ancestor.Ancestor;
            }

            //Is this implicitly castable to target?
            if(ImplicitlyCastableTo(target))
                return true;

            return false;
        }

        /// <summary>
        /// Checks if an implicit cast from this to target is legal(according to the Cräwl specification).
        /// </summary>
        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            //TODO Check additional rules for simple types
            return false;
        }

        /// <summary>
        /// Checks if a cast from this to target is legal(according to the Cräwl specification).
        /// </summary>
        public override bool CastableTo(CrawlType target)
        {
            //TODO Check additional rules for simple types

            //Are they equal?
            if (Equals(target))
                return true;

            //Is this a descendant of target?
            CrawlConstructedType ancestor = Ancestor;
            while (ancestor.Identifier != "$OBJECT")
            {
                if(ancestor.Equals(target))
                    return true;
                ancestor = ancestor.Ancestor;
            }

            return false;
        }
    }
}