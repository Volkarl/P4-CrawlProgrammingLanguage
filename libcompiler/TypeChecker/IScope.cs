using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcompiler.TypeChecker
{
    public interface IScope
    {
        TypeInformation[] GetScope(string symbol);
    }

    //TODO: This isn't how you TypeInformation
    public class TypeInformation
    {
    }
}
