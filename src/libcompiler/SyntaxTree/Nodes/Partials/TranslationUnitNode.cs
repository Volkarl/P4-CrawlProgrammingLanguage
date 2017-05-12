using System.Collections.Generic;
using libcompiler.Scope;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.SyntaxTree
{
    partial class TranslationUnitNode : IScope
    {
        public TypeInformation[] FindSymbol(string symbol)
        {
            return ImportedNamespaces.FindSymbol(symbol);
        }

        public IEnumerable<string> LocalSymbols() => new List<string>();
    }
}