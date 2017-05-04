using System;

namespace libcompiler.TypeSystem
{
    public abstract class CrawlType
    {
        public static CrawlStatusType UnspecifiedType { get; } =
            new CrawlStatusType("$UNSPECIFIED_TYPE", "$UNSPECIFIED_TYPE", "$UNSPECIFIED_TYPE");
        public static CrawlType ErrorType { get; } =
            new CrawlStatusType("$TYPE_ERROR", "$TYPE_ERROR", "$TYPE_ERROR");

        public static CrawlType ParseDecleration(string text)
        {
            throw new NotImplementedException();
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
    }
}