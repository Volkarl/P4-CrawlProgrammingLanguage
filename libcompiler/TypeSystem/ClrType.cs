using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Antlr4.Runtime;
using libcompiler.Parser;
using libcompiler.TypeChecker;

namespace libcompiler.TypeSystem
{
    class ClrType : CrawlType
    {
        public override bool IsArrayType => false;
        public override bool IsGenericType => false;
        public override bool IsValueType { get; }
        public override bool IsBuildInType => true;
        public string NameForUser { get; }
        public Type RepresentedType{ get; }

        private readonly ConcurrentDictionary<string, TypeInformation[]> SymbolCache = new ConcurrentDictionary<string, TypeInformation[]>();
        private readonly Dictionary<string, string> translations = new Dictionary<string, string>();

        public override TypeInformation[] GetScope(string symbol)
        {
            TypeInformation[] info;
            if (SymbolCache.TryGetValue(symbol, out info))
            {
                return info;
            }

            string translated;
            translations.TryGetValue(symbol, out translated);

            string use = translated ?? symbol;
            info = ExtractFromRepresentedType(use);
            return SymbolCache.GetOrAdd(use, info);
        }

        public override bool IsAssignableTo(CrawlType type)
        {
            return this == type;
        }

        public ClrType(Type clrRepresentedType, string name, IEnumerable<KeyValuePair<string, string>> translations)
        {
            IsValueType = clrRepresentedType.IsValueType;
            NameForUser = name;
            RepresentedType = clrRepresentedType;



            foreach (KeyValuePair<string,string> pair in translations)
            {
                this.translations.Add(pair.Key, pair.Value);
            }

        }

        private TypeInformation[] ExtractFromRepresentedType(string member)
        {
            MemberInfo[] members = RepresentedType.GetMember(member);
            TypeInformation[] inf = new TypeInformation[members.Length];
            for (int i = 0; i < members.Length; i++)
            {
                inf[i] = new TypeInformation();
                PropertyInfo propertyInfo = members[i] as PropertyInfo;
                if (propertyInfo != null)
                {
                    PropertyInfo property = propertyInfo;
                    inf[i].Type = CrawlType.FromClrType(property.PropertyType);
                }
                else throw new NotImplementedException();
            }
            return inf;
        }

        public ClrType(Type clrRepresentedType) : this(clrRepresentedType, clrRepresentedType.AssemblyQualifiedName,
            new KeyValuePair<string, string>[0])
        {

        }

        public override string ToString()
        {
            return NameForUser;
        }
    }
}
