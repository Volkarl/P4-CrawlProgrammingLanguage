using System.Collections.Generic;
using libcompiler.Scope;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.SyntaxTree
{
    public partial class TranslationUnitNode : IScope
    {
        public static readonly Dictionary<string, TypeInformation> SimpleTypes =
            new Dictionary<string, TypeInformation>()
            {
                {
                    "tal",
                    new TypeInformation( CrawlSimpleType.Tal, ProtectionLevel.Public, -1)
                },
                {
                    "kommatal",
                    new TypeInformation( CrawlSimpleType.Kommatal, ProtectionLevel.Public, -1)
                },
                {
                    "bool",
                    new TypeInformation(  CrawlSimpleType.Bool, ProtectionLevel.Public, -1)
                },
                {
                    "tegn",
                    new TypeInformation( CrawlSimpleType.Tegn, ProtectionLevel.Public, -1)
                },
                {
                    "tekst",
                    new TypeInformation( CrawlSimpleType.Tekst, ProtectionLevel.Public, -1)
                }
            };

        //TODO Also look in imports.
        public TypeInformation[] FindSymbol(string symbol)
        {
            return ImportedNamespaces.FindSymbol(symbol);
        }

        public IEnumerable<string> LocalSymbols() => new List<string>();
    }
}