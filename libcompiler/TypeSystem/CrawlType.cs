using libcompiler.TypeChecker;

namespace libcompiler.TypeSystem
{
    /// <summary>
    /// The base class all types derive from.
    /// Also contains static methods to properly create different types
    /// </summary>
    // The other half of the class is found in TypeSystem/CrawlTypeStaticFunction.cs
    public abstract partial class CrawlType : IScope
    {
        /// <summary>
        /// Does this type represent an array
        /// </summary>
        public abstract bool IsArrayType { get; }

        /// <summary>
        /// Does this type represent a generic type
        /// </summary>
        public abstract bool IsGenericType { get; }

        /// <summary>
        /// Is this a value(stack allocated) type
        /// </summary>
        public abstract bool IsValueType { get; }

        /// <summary>
        /// Does this type come from a library? (Otherwise its from Cräwl code)
        /// </summary>
        public abstract bool IsBuiltInType { get; }

        /// <summary>
        /// Gets all symbols this type exports
        /// </summary>
        public abstract TypeInformation[] GetScope(string identifier);

        /// <summary>
        /// Check if 2 types can be assigned to each other.
        /// Please note that x.IsAssignableTo(y) != y.IsAssignableTo(x) unless x == y
        /// </summary>
        public abstract bool IsAssignableTo(CrawlType type);
    }
}