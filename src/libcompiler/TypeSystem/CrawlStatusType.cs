using System;
using System.Collections.Generic;
using libcompiler.Scope;

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
            return false;
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            return false;
        }

        public override bool CastableTo(CrawlType target)
        {
            return false;
        }

        public override IEnumerable<KeyValuePair<string, TypeInformation[]>> Members()
        {
            throw new InvalidOperationException("An error type has no members");
        }

        public override TypeInformation[] FindSymbol(string symbol)
        {
            throw new InvalidOperationException("An error type has no members");
        }
    }
}