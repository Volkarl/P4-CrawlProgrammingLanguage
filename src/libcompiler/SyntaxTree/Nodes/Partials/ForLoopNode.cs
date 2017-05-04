using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.Scope;

namespace libcompiler.SyntaxTree
{
    partial class ForLoopNode : IScope
    {
        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation[] typeInformation = Scope?.FindSymbol(symbol);

            if (typeInformation == null)
            {
                IScope scope = Parent.FindFirstScope();
                return scope?.FindSymbol(symbol);
            }

            return typeInformation;
        }
        
        public IEnumerable<string> LocalSymbols() => this.Scope?.LocalSymbols();
    }
}
