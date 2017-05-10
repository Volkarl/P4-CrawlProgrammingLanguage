using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libcompiler.Scope;
using libcompiler.SyntaxTree;

namespace libcompiler.TypeSystem
{
    public class CrawlSimpleType : CrawlType
    {
        //TODO: make all instances of crawlType that is based on Type use this.
        //Examine type and if prudent, return CrawlArrayType or CrawlMethodType instead
        public static CrawlType Get(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            //This is horible code, but the refactor is bigish
            //If it dosen't exist, create the type and return it. 
            //The type is then responsible of adding itself during construction....
            //RACE CONDITION. If 2 methods try to get a new type at the same time, an uninitalized object can be returned
            //or 2 types representing the same type can be created

            //Really need to refactor it so this is the only way to get CrawlSimpleTypes...
            return new CrawlSimpleType(type);
        }

        
        private readonly Type _clrType;
        private ConcurrentDictionary<string, TypeInformation[]> _members;

        public static CrawlSimpleType Tal { get; } = new CrawlTypeTal();

        public static CrawlSimpleType Kommatal { get; } = new CrawlTypeKommatal();

        public static CrawlSimpleType Bool { get; } = new CrawlTypeBool();

        public static CrawlSimpleType Tegn { get; } = new CrawlTypeTegn();

        public static CrawlSimpleType Tekst { get; } = new CrawlTypeTekst();

        public static CrawlSimpleType Ting { get; } = new CrawlSimpleType(typeof(object));

        public static CrawlType Intet { get; } = new CrawlSimpleType(typeof(void));


        protected CrawlSimpleType(Type type) : base(type.Name , type.Namespace, type.Assembly.FullName)
        {
            if(type == null) throw new NullReferenceException(nameof(type));

            _clrType = type;

        }

        private void Initialize()
        {
            //Uhh, black magic linq.
            //For all members, convert them into a typeInformation.
            //Group members that share names
            var members = _clrType.GetMembers()
                .Select(MakeCrawlMember)
                .Where(x => x.Key != null)
                .GroupBy(x => x.Key);


            //Finally, convert groups of KeyValuePair<string, TypeInformation> into KeyValuePair<string, TypeInformation[]>
            _members = new ConcurrentDictionary<string, TypeInformation[]>(
                members.Select(x =>
                    new KeyValuePair<string, TypeInformation[]>(
                        x.Key,
                        x.Select(y => y.Value).ToArray()
                    )
                )
            );
        }

        private KeyValuePair<string, TypeInformation> MakeCrawlMember(MemberInfo member)
        {
            //TODO: This code is very very fragile... 
            //It assumes that every type is just a normal class. (Move handling to shared place someday)
            //This is incorrect if the type is an array or derives form System.Delegate
            CrawlType result;
            PropertyInfo property = member as PropertyInfo;
            ConstructorInfo ctor = member as ConstructorInfo;
            MethodInfo method = member as MethodInfo;
            EventInfo info  = member as EventInfo; //WTF DO?
            FieldInfo field = member as FieldInfo;
            Type type = member as Type;

            if (property != null)
            {
                result = CrawlSimpleType.Get(property.PropertyType);
            }
            else if (ctor != null)
            {
                result = new CrawlMethodType(Get(member.DeclaringType),
                    ctor.GetParameters().Select(x => Get(x.ParameterType)));
            }
            else if(method != null)
            {
                result = new CrawlMethodType(
                    CrawlSimpleType.Get(method.ReturnType),
                    method.GetParameters().Select(x => Get(x.ParameterType)));
            }
            else if (info != null)
            {
                return new KeyValuePair<string, TypeInformation>(null, null);  //Event
            }
            else if (field != null)
            {
                result = CrawlSimpleType.Get(field.FieldType);
            }
            else if (type != null)
            {
                return new KeyValuePair<string, TypeInformation>(null, null); //Nested type
            }
            else
            {
                throw new NotImplementedException();
            }
            
            return new KeyValuePair<string, TypeInformation>(member.Name, new TypeInformation(result, ProtectionLevel.Public, -1, DeclaringScope.ClassLike, NeedsABetterNameType.Member));
        }

        public override bool IsAssignableTo(CrawlType target)
        {
            return Equals(target) || ImplicitlyCastableTo(target);
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
            return _clrType.FullName;
        }

        public override IEnumerable<KeyValuePair<string, TypeInformation[]>> Members()
        {
            if(_members == null) Initialize();

            return _members;
        }

        public override TypeInformation[] FindSymbol(string symbol)
        {
            if(_members == null) Initialize();

            TypeInformation[] result;
            if (_members.TryGetValue(symbol, out result))
                return result;

            return null;
        }
    }
}