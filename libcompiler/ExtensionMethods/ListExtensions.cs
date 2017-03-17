using System.Collections.Generic;

namespace libcompiler.ExtensionMethods
{
    internal static class ListExtensions
    {
        public static T RemoveHead<T>(this List<T> list)
        {
            T head = list[0];
            list.RemoveAt(0);
            return head;
        }
    }
}
