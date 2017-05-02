using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.Datatypes;

namespace libcompiler.Scope
{
    class GenericScope : IScope
    {
        private readonly ConcurrentDictionary<string, TypeInformation[]> _scope = new ConcurrentDictionary<string, TypeInformation[]>();
        public GenericScope(IEnumerable<KeyValuePair<string, TypeInformation>> items)
        {
            foreach (IGrouping<string, KeyValuePair<string, TypeInformation>> pairs in items.GroupBy(x => x.Key))
            {
                _scope.TryAdd(pairs.Key, pairs.Select(x => x.Value).ToArray());
            }
        }

        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation[] value;
            if (_scope.TryGetValue(symbol, out value))
            {
                return value;
            }

            return null;
        }

        public IEnumerable<string> LocalSymbols() => _scope.Keys;

        public override string ToString()
        {
            return "{" + string.Join(", ", _scope.Keys) + "}";
        }
    }
}
