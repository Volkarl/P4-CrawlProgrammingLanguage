using System.Diagnostics;

namespace libcompiler
{
    static class TraceListners
    {
        public static TraceListener ParserTraceListner { get; set; } = null; // new ConsoleTraceListener();

        public static TraceListener AssemblyResolverListner { get; set; } = null;
    }
}
