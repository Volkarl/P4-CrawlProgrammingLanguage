using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.TypeSystem;

namespace libcompiler.TypeChecker
{
    public interface IScope
    {
        TypeInformation[] GetScope(string symbol);
    }

    public class TypeInformation
    {
        public CrawlType Type { get; set; }
    }
}
