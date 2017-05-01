using System.Collections.Generic;
using libcompiler.TypeChecker;
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
                    new TypeInformation( CrawlSimpleType.Tal, ProtectionLevel.Public)
                },
                {
                    "kommatal",
                    new TypeInformation( CrawlSimpleType.Kommatal, ProtectionLevel.Public)
                },
                {
                    "bool",
                    new TypeInformation(  CrawlSimpleType.Bool, ProtectionLevel.Public)
                },
                {
                    "tegn",
                    new TypeInformation( CrawlSimpleType.Tegn, ProtectionLevel.Public)
                },
                {
                    "tekst",
                    new TypeInformation( CrawlSimpleType.Tekst, ProtectionLevel.Public)
                }
            };

        //TODO Also look in imports.
        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation result;
            SimpleTypes.TryGetValue(symbol, out result);

            return new[] {result};
        }
    }
}