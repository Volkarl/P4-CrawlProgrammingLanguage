using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.TypeChecker;

namespace libcompiler.TypeSystem
{
    public abstract partial class CrawlType
    {
        public static CrawlType Error = ErrorType.Instance;
        public static CrawlType Void = new ClrType(typeof(void), "intet", new KeyValuePair<string, string>[0]);

        public static CrawlType String;
        public static CrawlType Int;

        public static CrawlType ParseDecleration(CrawlSyntaxNode context, string name)
        {
            if (name.Last() == ']')
            {
                int arrayEnd = name.FromBackGetMatchingIndex('[');

                string typepart = name.Substring(0, arrayEnd);
                string arrayDef = name.Substring(arrayEnd);
                int dimensions = arrayDef.Count(x => x == ',') + 1;
                return new ArrayType(ParseDecleration(context, typepart), dimensions);
            }


            ClrType returnvalue;
            if (TranslatedTypes.TryGetValue(name, out returnvalue))
            {
                return returnvalue;
            }

            TypeInformation[] info = context.FindFirstScope().GetScope(name);

            throw new NotImplementedException();
        }

        private static readonly ConcurrentDictionary<string, ClrType> TranslatedTypes = new ConcurrentDictionary<string, ClrType>();
        private static readonly ConcurrentDictionary<Type, ClrType> TypeCache = new ConcurrentDictionary<Type, ClrType>();

        static CrawlType()
        {

            String = new ClrType(typeof(String), "tekst", new[]
            {
                new KeyValuePair<string, string>("Længde", "Length"),
            });

            Int = new ClrType(typeof(int), "tal", new KeyValuePair<string, string>[0]);

            List<ClrType> translatedTypes = new List<ClrType>()
            {
                (ClrType)String,
                (ClrType)Int,
                new ClrType(typeof(void), "intet", new KeyValuePair<string, string>[0]),

            };

            foreach (ClrType type in translatedTypes)
            {
                if (!TranslatedTypes.TryAdd(type.NameForUser, type)) {throw new Exception();}
                if(!TypeCache.TryAdd(type.RepresentedType, type)) {throw new Exception();}
            }
        }

        private class ErrorType : CrawlType
        {
            private ErrorType() {}

            public override bool IsArrayType => false;
            public override bool IsGenericType => false;
            public override bool IsValueType => false;
            public override bool IsBuildInType => false;
            public static CrawlType Instance { get; } = new ErrorType();

            public override TypeInformation[] GetScope(string symbol)
            {
                return new TypeInformation[0];
            }

            public override bool IsAssignableTo(CrawlType type)
            {
                return false;
            }

            public override string ToString()
            {
                return "Error";
            }
        }

        protected static CrawlType FromClrType(Type type)
        {
            return TypeCache.GetOrAdd(type, clrType => new ClrType(clrType));
        }
    }
}