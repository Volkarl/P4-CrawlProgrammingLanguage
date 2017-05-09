using System;

namespace libcompiler.TypeSystem
{
    public class CrawlTypeTal: CrawlSimpleType
    {
        public CrawlTypeTal() : base(typeof(int))
        {
        }

        public override bool CastableTo(CrawlType target)
        {
            if (target is CrawlTypeKommatal)
                return true;

            return base.CastableTo(target);
        }
    }


    public class CrawlTypeKommatal: CrawlSimpleType
    {
        public CrawlTypeKommatal() : base(typeof(double))
        {
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            if (target is CrawlTypeTal)
                return true;
            return base.ImplicitlyCastableTo(target);
        }

        public override bool CastableTo(CrawlType target)
        {
            if (target is CrawlTypeTal)
                return true;
            return base.CastableTo(target);
        }
    }


    public class CrawlTypeBool: CrawlSimpleType
    {
        public CrawlTypeBool() : base(typeof(bool))
        {
        }
    }
    public class CrawlTypeTegn: CrawlSimpleType
    {
        public CrawlTypeTegn() : base(typeof(char))
        {
        }

        public override bool ImplicitlyCastableTo(CrawlType target)
        {
            if (target is CrawlTypeTekst)
                return true;
            return base.ImplicitlyCastableTo(target);
        }

        public override bool CastableTo(CrawlType target)
        {
            if (target is CrawlTypeTekst)
                return true;
            return base.CastableTo(target);
        }
    }


    public class CrawlTypeTekst: CrawlSimpleType
    {
        public CrawlTypeTekst() : base(typeof(string))
        {
        }
    }
}