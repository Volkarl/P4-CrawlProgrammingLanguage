using System;
using System.Collections.Generic;
using libcompiler.Scope;

namespace libcompiler.TypeSystem
{
    public class CrawlConstructedType : CrawlType, IScope
    {
        public CrawlConstructedType(string name, string ns)
            : base(name, ns)
        {
            //_declaration = declaration;

            //TODO Will nullref for classes inheriting from $OBJECT
            //Ancestor = (CrawlConstructedType) declaration.FindFirstScope().FindSymbol(declaration.Ancestor.Identifier)[0].Type;
        }


        public CrawlConstructedType Ancestor { get; }
        public IReadOnlyList<CrawlType> Interfaces { get; }


        public TypeInformation[] FindSymbol(string symbol)
        {
            throw new NotImplementedException();
            //return _declaration.FindSymbolOnlyInThisScope(symbol);
        }

        public IEnumerable<string> LocalSymbols() => default(IEnumerable<string>);

        /// <summary>
        /// Checks if assigning this to target is legal(according to Cräwl specification).
        /// </summary>
        public override bool IsAssignableTo(CrawlType target)
        {
            throw new NotImplementedException();


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

        public override string ToString()
        {
            return $"[{Assembly}] {Identifier} {"Not initialized"}";
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
            throw new NotImplementedException();
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