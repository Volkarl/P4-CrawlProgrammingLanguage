using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.TypeChecker;

namespace libcompiler.TypeSystem
{
    class BuildInType : CrawlType
    {
        public override bool IsArrayType => false;
        public override bool IsGenericType => false;
        public override bool IsValueType { get; }
        public override bool IsBuildInType => true;

        public override TypeInformation[] GetScope(string symbol)
        {
            throw new NotImplementedException();
        }

        public BuildInType(Type CLRType, string name, IEnumerable<KeyValuePair<string, string>> translations)
        {
            IsValueType = CLRType.IsValueType;
        }
    }
}
