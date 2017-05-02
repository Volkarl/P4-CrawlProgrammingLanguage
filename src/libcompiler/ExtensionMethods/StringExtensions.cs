using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcompiler.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string AddStringForeach(string str, int count)
        {
            var s = new StringBuilder(count);
            for (int i = 0; i < count; i++)
            {
                s.Append(str);
            }
            return s.ToString();
        }
    }
}