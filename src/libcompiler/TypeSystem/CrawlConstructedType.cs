using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using libcompiler.Scope;
using libcompiler.SyntaxTree;

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


        private ConcurrentDictionary<string, TypeInformation[]> _scope;
        public CrawlType Ancestor { get; private set; }
        public IReadOnlyList<CrawlType> Interfaces { get; }


        public TypeInformation[] FindSymbol(string symbol)
        {
            if (_scope == null) return null;
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
            //CrawlConstructedType ancestor = Ancestor;
            //while (ancestor.Identifier != f$OBJECT")
            //{
            //    if(ancestor.Equals(target))
            //        return true;
            //    ancestor = ancestor.Ancestor;
            //}

            ////Is this implicitly castable to target?
            //if(ImplicitlyCastableTo(target))
            //    return true;

            //return false;
        }

        public override string ToString()
        {
            if (_scope == null)
                return $"Not initialized type with name {Identifier}";

            else return $"[{Assembly}] {Identifier}:{Ancestor} {{{string.Join(",", _scope)}}}";
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

            ////Are they equal?
            //if (Equals(target))
            //    return true;

            ////Is this a descendant of target?
            //CrawlConstructedType ancestor = Ancestor;
            //while (ancestor.Identifier != "$OBJECT")
            //{
            //    if(ancestor.Equals(target))
            //        return true;
            //    ancestor = ancestor.Ancestor;
            //}

            //return false;
        }

        public void Initialize(ClassTypeDeclerationNode self)
        {
            if(Ancestor != null) throw new InvalidOperationException("This class has already been initialized once");

            IScope scope = self.FindFirstScope();

            if (self.BaseTypes.Count() > 1)
            {
                throw new NotImplementedException("Not supporting multiple inheritance in any form, yet!");
            }
            else if (self.BaseTypes.Count() == 1)
            {
                //Bug? parrent class declared later in method?
                TypeInformation baseInformation = scope.FindSymbol(self.BaseTypes[0].Value).Single();
                if (baseInformation.NeedsABetterNameType == NeedsABetterNameType.Class)
                { Ancestor = baseInformation.Type;}
                else
                {
                   throw new Exception("Derives from something not a type"); //TODO: Err msg 
                }
            }
            else
            {
                Ancestor = CrawlSimpleType.Ting;
            }


            List<KeyValuePair<string, TypeInformation[]>> members = Ancestor.Members().ToList();
            members.AddRange(self.Body.Scope.LocalSymbols().Select(name => new KeyValuePair<string, TypeInformation[]>(name, self.Body.Scope.FindSymbol(name))));
                
            _scope = new ConcurrentDictionary<string, TypeInformation[]>(members);
        }

        public override IEnumerable<KeyValuePair<string, TypeInformation[]>> Members()
        {
            return _scope;
        }
    }
}