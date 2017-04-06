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
    // This file contains the static part of CrawlType: Methods to create Types and static data.
    public abstract partial class CrawlType
    {
        /// <summary>
        /// Represents a type error
        /// </summary>
        public static CrawlType Error = ErrorType.Instance;

        /// <summary>
        /// Represents the lack of a type
        /// </summary>
        public static CrawlType Void;

        /// <summary>
        /// Represents the string type. This is the same as the System.String
        /// </summary>
        public static CrawlType String;

        /// <summary>
        /// Represents the integer type. This is (most likely) the same type as System.Int32
        /// </summary>
        public static CrawlType Int;

        /// <summary>
        /// Parses the decleration of a type from the string the programmer has written for the type, taking into consideriation in which scope it was declared.
        /// </summary>
        /// <param name="context">The IScope this type is decleared in. Types are not globally visible so this is required</param>
        /// <param name="name">The string the programmer has written for the type. This is most often just the name, but can contain other details, such as []</param>
        /// <returns>A CrawlType representing name</returns>
        public static CrawlType ParseDecleration(IScope context, string name)
        {
            //Parse [] and count ,, inside, then create array with remaining parts
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

            TypeInformation[] info = context.GetScope(name);

            throw new NotImplementedException();
        }

        private static readonly ConcurrentDictionary<string, ClrType> TranslatedTypes =
            new ConcurrentDictionary<string, ClrType>();

        private static readonly ConcurrentDictionary<Type, ClrType> TypeCache =
            new ConcurrentDictionary<Type, ClrType>();

        static CrawlType()
        {
            Void = new ClrType(typeof(void), "intet", new KeyValuePair<string, string>[0]);

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
                if(!TranslatedTypes.TryAdd(type.NameForUser, type)) {throw new Exception();}
                if(!TypeCache.TryAdd(type.RepresentedType, type)) {throw new Exception();}
            }
        }

        private class ErrorType : CrawlType
        {
            private ErrorType() {}

            public override bool IsArrayType => false;
            public override bool IsGenericType => false;
            public override bool IsValueType => false;
            public override bool IsBuiltInType => false;
            public static CrawlType Instance { get; } = new ErrorType();

            public override TypeInformation[] GetScope(string identifier)
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