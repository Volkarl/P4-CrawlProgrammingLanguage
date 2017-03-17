using System.Diagnostics;

namespace libcompiler
{
    static class TraceListners
    {
        public static TraceListener ParserTraceListner { get; set; } = null; // new ConsoleTraceListener();
    }
}
