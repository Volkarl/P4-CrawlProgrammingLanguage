﻿using libcompiler.TypeChecker;

namespace libcompiler.TypeSystem
{
    public class ArrayType : CrawlType
    {
        public ArrayType(CrawlType contains, int dimensions)
        {
            Contains = contains;
            Dimensions = dimensions;
        }

        public override bool IsArrayType => true;
        public override bool IsGenericType => false;
        public override bool IsValueType => false;
        public override bool IsBuildInType => false; //TODO: I think this is right but...

        public CrawlType Contains { get; }
        public int Dimensions { get; }

        public override TypeInformation[] GetScope(string symbol)
        {
            throw new System.NotImplementedException();
        }
    }
}