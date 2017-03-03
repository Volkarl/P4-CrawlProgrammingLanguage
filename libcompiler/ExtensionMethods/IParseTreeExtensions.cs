using System;
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
    }
}
