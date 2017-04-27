using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;

namespace libcompiler.TypeChecker
{
    public interface IScope
    {
        /// <summary>
        /// Search for symbol.
        /// </summary>
        /// <returns>An array of possible matches for symbol, at its first occurence. Empty if none could be found. I believe the only time the array has more than 1 element is when multiple overloads of the same method are avaiable.</returns>
        TypeInformation[] FindSymbol(string symbol);
    }


    public class TypeInformation
    {
        public TypeInformation(CrawlType type, ProtectionLevel protectionLevel)
        {
            Type = type;
            ProtectionLevel = protectionLevel;
        }

        public CrawlType Type { get; }
        public ProtectionLevel ProtectionLevel { get; }
        //TODO Save place where it was declared
    }
}
