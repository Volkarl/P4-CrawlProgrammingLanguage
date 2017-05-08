using System.Collections.Generic;
using libcompiler.Scope;

namespace libcompiler.SyntaxTree
{
    public partial class ClassTypeDeclerationNode //: IScope
    {

        //BUG: This looks in its Body(a child) which then looks in this. Infinite recursion -> StackOverflow
        public IEnumerable<string> LocalSymbols() => default(IEnumerable<string>);

        public TypeInformation[] FindSymbol(string symbol)
        {
            //Kig i eget scope
            TypeInformation[] result = FindSymbolOnlyInThisScope(symbol);

            //TODO Kig i forfader-klasse

            //Spørg parent-scope
            if (result == null)
            {
                IScope scope = Parent.FindFirstScope();
                return scope?.FindSymbol(symbol);
            }
            return result;
        }

        public TypeInformation[] FindSymbolOnlyInThisScope(string symbol)
        {
            return Body.FindSymbol(symbol);
        }
    }
}