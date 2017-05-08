using System.Collections.Generic;

namespace libcompiler.ExtensionMethods
{
    public static class EnumerableHelpers
    {
        public static IEnumerable<T> AsSingleIEnumerable<T>(this T val)
        {
            yield return val;
        }
    }
}