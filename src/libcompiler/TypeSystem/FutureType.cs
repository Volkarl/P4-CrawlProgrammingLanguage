using System;
using libcompiler.SyntaxTree;

namespace libcompiler.TypeSystem
{
    public class FutureType : CrawlType
    {
        public FutureType(string identifier, string ns) : base(identifier, ns, "<<<===NONE===>>>")
        {

        }

        public override bool IsAssignableTo(CrawlType target)
        {
            throw new InvalidOperationException();
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            throw new InvalidOperationException();
        }

        public override bool CastableTo(CrawlType target)
        {
            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return $"Future {Namespace}.{Identifier}";
        }
    }
}