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
        public override bool IsBuiltInType => true;
        public string NameForUser { get; }
        public Type RepresentedType{ get; }

        private readonly ConcurrentDictionary<string, TypeInformation[]> _symbolCache =
            new ConcurrentDictionary<string, TypeInformation[]>();

        private readonly Dictionary<string, string> _translations =
            new Dictionary<string, string>();

        /// <summary>
        /// Searches for identifier from what is available in this type.
        /// </summary>
        public override TypeInformation[] GetScope(string identifier)
        {
            TypeInformation[] info;
            if (_symbolCache.TryGetValue(identifier, out info))
            {
                return info;
            }

            string translated;
            _translations.TryGetValue(identifier, out translated);

            string use = translated ?? identifier;
            info = ExtractFromRepresentedType(use);
            return _symbolCache.GetOrAdd(use, info);
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
                this._translations.Add(pair.Key, pair.Value);
            }

        }

        private TypeInformation[] ExtractFromRepresentedType(string member)
        {
            MemberInfo[] members = RepresentedType.GetMember(member);
            TypeInformation[] inf = new TypeInformation[members.Length];
            for (int i = 0; i < members.Length; i++)
            {
                PropertyInfo propertyInfo = members[i] as PropertyInfo;
                if (propertyInfo != null)
                {
                    PropertyInfo property = propertyInfo;
                    inf[i] = new TypeInformation(CrawlType.FromClrType(property.PropertyType));
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
