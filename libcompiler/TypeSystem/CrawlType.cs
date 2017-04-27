using System;
using System.Collections.Generic;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.TypeChecker;

namespace libcompiler
{
    public abstract class CrawlType
    {
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