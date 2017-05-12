using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using libcompiler.Datatypes;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.Scope
{
    public interface IScope
    {
        /// <summary>
        /// Search for symbol.
        /// </summary>
        /// <returns>An array of possible matches for symbol, at its first occurence. Empty if none could be found. I believe the only time the array has more than 1 element is when multiple overloads of the same method are avaiable.</returns>
        TypeInformation[] FindSymbol(string symbol);

        /// <summary>
        /// Get all symbols declared in <em>this</em>scope
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> LocalSymbols();
    }


    public class TypeInformation
    {
        public TypeInformation(CrawlType type, ProtectionLevel protectionLevel, int declarationLocation, UniqueItem item, DeclaringScope declaringScope = DeclaringScope.MethodLike, DeclaredAs declaredAs = DeclaredAs.Variable)
        {
            Type = type;
            ProtectionLevel = protectionLevel;
            DeclarationLocation = declarationLocation;
            Item = item;
            DeclaringScope = declaringScope;
            DeclaredAs = declaredAs;
        }

        public int DeclarationLocation { get; }
        public CrawlType Type { get; }
        public ProtectionLevel ProtectionLevel { get; }
        public DeclaringScope DeclaringScope { get; }
        public DeclaredAs DeclaredAs { get; }
        public UniqueItem Item { get; }

        //TODO Save place where it was declared
    }

    public enum DeclaredAs
    {
        Variable,
        Class,
        Member
    }

    public enum DeclaringScope
    {
        MethodLike,
        ClassLike
    }
}
