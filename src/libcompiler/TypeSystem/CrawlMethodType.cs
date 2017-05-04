using System.Collections.Generic;
using System.Linq;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;

namespace libcompiler.SyntaxTree
{
    public class CrawlMethodType : CrawlType
    {
        public CrawlType ReturnType { get; }
        public List<CrawlType> Parameters { get; }

        public CrawlMethodType(CrawlType returnType, IEnumerable<CrawlType> parameters)
            : base("", "")
        {
            ReturnType = returnType;
            Parameters = parameters.ToList();
        }

        public override bool IsAssignableTo(CrawlType target)
        {
            return Equals(target);
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            return Equals(target);
        }

        public override bool CastableTo(CrawlType target)
        {
            return Equals(target);
        }

        public override string ToString()
        {
            return $"{ReturnType}({string.Join(",", Parameters)})";
        }
    }
}