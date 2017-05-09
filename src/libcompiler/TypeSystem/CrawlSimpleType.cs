using System;

namespace libcompiler.TypeSystem
{
    public class CrawlSimpleType : CrawlType
    {
        private readonly Type _clrType;

        public static CrawlSimpleType Tal { get; } = new CrawlTypeTal();

        public static CrawlSimpleType Kommatal { get; } = new CrawlTypeKommatal();

        public static CrawlSimpleType Bool { get; } = new CrawlTypeBool();

        public static CrawlSimpleType Tegn { get; } = new CrawlTypeTegn();

        public static CrawlSimpleType Tekst { get; } = new CrawlTypeTekst();

        public CrawlSimpleType(Type type) : base(type.FullName , type.Namespace, type.Assembly.FullName)
        {
            if(type == null) throw new NullReferenceException(nameof(type));
            _clrType = type;
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
    }
}