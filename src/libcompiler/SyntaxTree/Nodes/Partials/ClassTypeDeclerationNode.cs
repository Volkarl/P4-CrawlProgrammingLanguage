using System.Collections.Generic;
using libcompiler.Scope;

namespace libcompiler.SyntaxTree
{
    public partial class ClassTypeDeclerationNode : IScope
    {

        //BUG: This looks in its Body(a child) which then looks in this. Infinite recursion -> StackOverflow
        public IEnumerable<string> LocalSymbols() => ClassType.Ancestor.LocalSymbols();

        public TypeInformation[] FindSymbol(string symbol)
        {
            //Only look in ancestor. BlockNode child contains scope for this
            TypeInformation[] typeInformation = ClassType?.Ancestor?.FindSymbol(symbol);

            if (typeInformation == null)
            {
                IScope scope = Parent.FindFirstScope();
                return  scope?.FindSymbol(symbol);
            }

            return typeInformation;
        }
    }
}