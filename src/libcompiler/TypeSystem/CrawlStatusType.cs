using System;

namespace libcompiler.TypeSystem
{
    /// <summary>
    /// Symbolizes a status within the type system. Either Unspecified or Error.
    /// </summary>
    public class CrawlStatusType : CrawlType
    {
        public CrawlStatusType(string identifier, string @namespace, string assembly = "CrawlCode") : base(identifier, @namespace, assembly)
        {
        }

        public override bool IsAssignableTo(CrawlType target)
        {
            throw new InvalidOperationException("Tried to use unspecified type.");
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            throw new InvalidOperationException("Tried to use unspecified type.");
        }

        public override bool CastableTo(CrawlType target)
        {
            throw new InvalidOperationException("Tried to use unspecified type.");
        }
    }
}