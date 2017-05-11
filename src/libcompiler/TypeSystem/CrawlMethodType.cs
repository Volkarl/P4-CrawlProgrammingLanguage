using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libcompiler.Scope;

namespace libcompiler.TypeSystem
{
    public class CrawlMethodType : CrawlType
    {
        public CrawlType ReturnType { get; }
        public List<CrawlType> Parameters { get; }

        public MethodInfo MethodInfo { get; private set; }

        public CrawlMethodType(CrawlType returnType, IEnumerable<CrawlType> parameters)
            : base("", "")
        {
            ReturnType = returnType;
            Parameters = parameters.ToList();
        }

        public CrawlMethodType(MethodInfo info) : base("", "")
        {
            MethodInfo = info;
            ReturnType = CrawlSimpleType.Get(info.ReturnType);
            Parameters = info.GetParameters().Select(x => CrawlSimpleType.Get(x.ParameterType)).ToList();
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

        public override IEnumerable<KeyValuePair<string, TypeInformation[]>> Members()
        {
            yield break;
        }

        public override TypeInformation[] FindSymbol(string symbol)
        {
            throw new NotImplementedException();
        }

        public override Type ClrType => null;
    }
}