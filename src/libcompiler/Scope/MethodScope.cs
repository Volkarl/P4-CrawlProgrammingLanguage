using System.Collections.Generic;
using System.Linq;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.Scope
{
    class MethodScope : IScope
    {
        private readonly Dictionary<string, TypeInformation[]> _scopeInfo;
        public MethodScope(MethodDeclerationNode m)
        {
            //TODO: Save real info in typeinformation....
            _scopeInfo = m.Parameters.ToDictionary(x => x.Identifier.Value, y => new TypeInformation[1]{new TypeInformation(new FutureType(m.Identifier.Value, "NONE"), m.ProtectionLevel, m.Interval.a)});
        }

        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation[] typeArray;
            if (_scopeInfo.TryGetValue(symbol, out typeArray) == true)
            {
                return typeArray;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<string> LocalSymbols() => _scopeInfo.Keys;
    }
}
