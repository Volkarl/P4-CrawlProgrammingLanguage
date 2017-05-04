using System;
using System.Linq;
using libcompiler.Scope;

namespace libcompiler.SyntaxTree
{
    public abstract class CrawlType
    {
        public static CrawlType Intet { get; }= new CrawlSimpleType(typeof(void));

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
                string[] parameters = parametersSingleString.Split(new []{','}, StringSplitOptions.RemoveEmptyEntries);

                CrawlType returnType;
                if (remainingType.Trim() == "intet")
                {
                    returnType = CrawlType.Intet;
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
                    throw new Exception("Unknown type " + text); //TODO: ERROR MSG
                }
                else if (results.Length == 1)
                {
                    return results[0].Type;
                }
                else 
                    throw new Exception("Ambigious types " + text); //TODO: ERROR MSG
            }


        }

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

        //public IReadOnlyList<CrawlType> Interfaces { get; }


        public override bool Equals(object obj)
        {
            return (obj as CrawlType)?.CanonicalName == CanonicalName;
        }

        public override int GetHashCode()
        {
            return CanonicalName.GetHashCode();
        }


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
    }
}