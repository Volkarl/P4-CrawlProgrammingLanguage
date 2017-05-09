using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using libcompiler.Scope;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.Namespaces
{
    [DebuggerDisplay("{Name}({_scope.Count} items)")]
    public class Namespace : IScope
    {
        public static Namespace BuiltinNamespace { get; }

        static Namespace()
        {
            BuiltinNamespace = new Namespace("");

            BuiltinNamespace._scope.TryAdd("tal", new[] { new TypeInformation(CrawlSimpleType.Tal, ProtectionLevel.Public, -1, DeclaringScope.ClassLike) });
            BuiltinNamespace._scope.TryAdd("tekst", new[] { new TypeInformation(CrawlSimpleType.Tekst, ProtectionLevel.Public, -1, DeclaringScope.ClassLike) });
            BuiltinNamespace._scope.TryAdd("tegn", new[] { new TypeInformation(CrawlSimpleType.Tegn, ProtectionLevel.Public, -1, DeclaringScope.ClassLike) });
            BuiltinNamespace._scope.TryAdd("kommatal", new[] { new TypeInformation(CrawlSimpleType.Kommatal, ProtectionLevel.Public, -1, DeclaringScope.ClassLike) });
            BuiltinNamespace._scope.TryAdd("bool", new[] {new TypeInformation(CrawlSimpleType.Bool, ProtectionLevel.Public, -1, DeclaringScope.ClassLike) });

            //
        }

        public string Name { get; }
        private readonly ConcurrentDictionary<string, TypeInformation[]> _scope = new ConcurrentDictionary<string, TypeInformation[]>();

        public Namespace(string name, IEnumerable<CrawlType> contents) : this(name)
        {
            foreach (CrawlType type in contents)
            {
                _scope.TryAdd(type.Identifier, new[] {new TypeInformation(type, ProtectionLevel.Public, -1, DeclaringScope.ClassLike, NeedsABetterNameType.Class)});
            }
        }

        public Namespace(string name)
        {
            Name = name;
        }


        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation[] stuff;
            if (_scope.TryGetValue(symbol, out stuff))
                return stuff;
            return null;
        }

        public static Namespace Merge(params Namespace[] namespaces)
        {
            if (namespaces.Length == 1)
                return namespaces[0];

            //BUG: If multiple namespaces contain the same type, this will crash. Should really contain special value
            //that says not sure, don't try this pls
            string name = string.Join(";", namespaces.Select(n => n.Name).Distinct());

            //Pull out every value in each namespace, then pull out individual TypeInformation, then pull out the CrawlType
            Namespace returnvalue = new Namespace(name);

            foreach (var symbols in
                namespaces
                    .SelectMany(ns => ns._scope)
                    .GroupBy(ns => ns.Key))
            {
                TypeInformation[] allInformations = symbols.SelectMany(x => x.Value).ToArray();

                returnvalue._scope.TryAdd(symbols.Key, allInformations);
            }

            return returnvalue;
        }

        public IEnumerable<string> LocalSymbols() => _scope.Keys;
    }
}