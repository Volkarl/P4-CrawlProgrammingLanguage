using libcompiler.TypeChecker;

namespace libcompiler.TypeSystem
{
    //TODO: Should probably be in a TypeSystem folder
    public abstract partial class CrawlType : IScope
    {
        public abstract bool IsArrayType { get; }
        public abstract bool IsGenericType { get; }
        public abstract bool IsValueType { get; }
        public abstract bool IsBuildInType { get; }

        public abstract TypeInformation[] GetScope(string symbol);
    }
}