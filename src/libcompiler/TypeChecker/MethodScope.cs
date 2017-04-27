﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;

namespace libcompiler.TypeChecker
{
    class MethodScope : IScope
    {
        private readonly Dictionary<string, TypeInformation[]> _scopeInfo;
        public MethodScope(MethodDeclerationNode m)
        {
            //TODO: Save real info in typeinformation....
            _scopeInfo = m.Parameters.ToDictionary(x => x.Value, y => new TypeInformation[1]);
        }

        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation[] typeArray;
            if (_scopeInfo.TryGetValue(symbol, out typeArray) == true)
            {
                return typeArray;
            }
            else
            {
                return null;
            }
        }
    }
}
