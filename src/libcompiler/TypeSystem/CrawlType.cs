using System;
using System.Collections.Generic;
using System.Linq;
using libcompiler.Scope;
using libcompiler.SyntaxTree;

namespace libcompiler.TypeSystem
{
    public abstract class CrawlType : IScope
    {
        public static CrawlType ParseDecleration(IScope declerationScope, string text)
        {
            if (text.Last() == ']')
            {
                int arrayMarkStart = text.LastIndexOf('[');
                string remainingType = text.Substring(0, arrayMarkStart);
                string arrayPart = text.Substring(arrayMarkStart);
                int rank = arrayPart.Count(x => x == ',') + 1;

                return new CrawlArrayType(rank, ParseDecleration(declerationScope, remainingType));
            }
            else if (text.Last() == ')')
            {
                int methodMarkStart = text.LastIndexOf('(');
                string remainingType = text.Substring(0, methodMarkStart);
                string parametersSingleString = text.Substring(methodMarkStart + 1, text.Length - methodMarkStart - 2);
                //Uhh, probably correct. Well not, but i don't think we can get anything that breaks it. "foo(,)" would lead to no parameters...
                string[] parameters = parametersSingleString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                CrawlType returnType;
                if (remainingType.Trim() == "intet")
                {
                    returnType = CrawlSimpleType.Intet;
                }
                else
                {
                    returnType = ParseDecleration(declerationScope, remainingType);
                }
                CrawlType[] parameterTypes = new CrawlType[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterTypes[i] = ParseDecleration(declerationScope, parameters[i]);
                }

                return new CrawlMethodType(returnType, parameterTypes);
            }
            else
            {
                var results = declerationScope.FindSymbol(text.Trim());

                if (results == null || results.Length == 0)
                {
                    throw new TypeNotFoundException("Unknown type " + text); //TODO: ERROR MSG
                }
                else if (results.Length == 1)
                {
                    return results[0].Type;
                }
                else
                    throw new Exception("Ambigious types " + text); //TODO: ERROR MSG
            }

        }

        public static CrawlStatusType UnspecifiedType { get; } =
            new CrawlStatusType("$UNSPECIFIED_TYPE", "$UNSPECIFIED_TYPE", "$UNSPECIFIED_TYPE");
        public static CrawlType ErrorType { get; } =
            new CrawlStatusType("$TYPE_ERROR", "$TYPE_ERROR", "$TYPE_ERROR");

        protected CrawlType(string identifier, string @namespace, string assembly="CrawlCode")
        {
            Identifier = identifier;
            Namespace = @namespace;
            Assembly = assembly;
        }

        public string Identifier { get; }
        public string Namespace { get; }
        public string Assembly { get; }

        public string CanonicalName => $"[{Assembly}]{Namespace}.{Identifier}";

        public CrawlType Ancestor { get; protected set; }
        public IReadOnlyList<CrawlType> Interfaces { get; protected set; }

        public override bool Equals(object obj)
        {
            return (obj as CrawlType)?.CanonicalName == CanonicalName;
        }

        public override int GetHashCode()
        {
            return CanonicalName.GetHashCode();
        }

        public override string ToString() => $"{Identifier}";


        /// <summary>
        /// Checks if assigning this to target is legal(according to Cräwl specification).
        /// </summary>
        public abstract bool IsAssignableTo(CrawlType target);

        /// <summary>
        /// Checks if an implicit cast from this to target is legal(according to the Cräwl specification).
        /// </summary>
        public abstract bool ImplicitlyCastableTo(CrawlType target);

        /// <summary>
        /// Checks if a cast from this to target is legal(according to the Cräwl specification).
        /// </summary>
        public abstract bool CastableTo(CrawlType target);

        public abstract IEnumerable<KeyValuePair<string, TypeInformation[]>> Members();
        public abstract TypeInformation[] FindSymbol(string symbol);
        public IEnumerable<string> LocalSymbols() => Members().Select(x => x.Key);
    }
}