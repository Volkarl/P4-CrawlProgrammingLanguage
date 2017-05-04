using System;

namespace libcompiler.SyntaxTree
{
    public class CrawlSimpleType : CrawlType
    {
        private readonly Type _clrType;

        public static CrawlSimpleType Tal { get; } = new CrawlSimpleType(typeof(int));

        public static CrawlSimpleType Kommatal { get; } = new CrawlSimpleType(typeof(double));

        public static CrawlSimpleType Bool { get; } = new CrawlSimpleType(typeof(bool));

        public static CrawlSimpleType Tegn { get; } = new CrawlSimpleType(typeof(char));

        public static CrawlSimpleType Tekst { get; } = new CrawlSimpleType(typeof(string));

        public CrawlSimpleType(Type type) : base(type.FullName , type.Namespace, type.Assembly.FullName)
        {
            if(type == null) throw new NullReferenceException(nameof(type));
            _clrType = type;
        }

        public override bool IsAssignableTo(CrawlType target)
        {
            return Equals(target);
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            throw new NotImplementedException();
        }

        public override bool CastableTo(CrawlType target)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return _clrType.FullName;
        }
    }
}