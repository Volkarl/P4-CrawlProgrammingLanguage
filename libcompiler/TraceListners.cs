using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcompiler
{
    static class TraceListners
    {
        public static TraceListener ParserTraceListner { get; set; } = null; // new ConsoleTraceListener();
    }
}
