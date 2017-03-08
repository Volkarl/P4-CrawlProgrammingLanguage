using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;

namespace libcompiler.ExtensionMethods
{
    static class IParseTreeExtensions
    {
        public static IParseTree LastChild(this IParseTree tree)
        {
            return tree.GetChild(tree.ChildCount - 1);
        }

        public static IEnumerable AsIEnumerable(this IParseTree tree)
        {
            for (int i = 0; i < tree.ChildCount; i++)
            {
                yield return tree.GetChild(i);
            }
        }

        public static IEnumerable AsEdgeTrimmedIEnumerable(this IParseTree tree)
        {
            for (int i = 1; i < tree.ChildCount - 1; i++)
            {
                yield return tree.GetChild(i);
            }
        }
    }
}
