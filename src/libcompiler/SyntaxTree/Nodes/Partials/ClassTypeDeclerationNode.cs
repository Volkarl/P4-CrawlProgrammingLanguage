using libcompiler.TypeChecker;

namespace libcompiler.SyntaxTree
{
    public partial class ClassTypeDeclerationNode : IScope
    {


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