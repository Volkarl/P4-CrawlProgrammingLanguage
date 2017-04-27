using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace libcompiler
{
    [DebuggerDisplay("{ToString()}")]
    public class CompilationMessage
    {
        public MessageSeverity Severity { get; }
        public MessageCode MessageCode { get; }

        private readonly string beforeerror, error, aftererror;

        public CompilationMessage(MessageSeverity severity, MessageCode messageCode, string beforeerror, string error, string aftererror)
        {
            Severity = severity;
            MessageCode = messageCode;
            this.beforeerror = beforeerror;
            this.error = error;
            this.aftererror = aftererror;
        }

        public static CompilationMessage Create(ITokenStream source, Interval errorLocation, MessageCode code,
            MessageSeverity severity = MessageSeverity.Error)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));
            if(errorLocation.Equals(Interval.Invalid)) throw new ArgumentOutOfRangeException(nameof(errorLocation));

            //Interval of 10 before, 10 after error
            //TODO: check against 0, source.Lenght
            Interval before = new Interval(errorLocation.a - 11, errorLocation.a -1);
            Interval after = new Interval(errorLocation.b + 1, errorLocation.b + 11);

            string beforestring = source.GetText(before);
            string error = source.GetText(errorLocation);
            string afterstring = source.GetText(after);


            return new CompilationMessage(severity, code, beforestring, error, afterstring);

        }

        public override string ToString()
        {
            return
                $"{GetFriendlyName(Severity)}: {GetErrorCodeText(MessageCode)}\n{GetErrorDescription(MessageCode)}\n{beforeerror}{error}{aftererror}";
        }

        public void WriteToConsole()
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ForSeverity(Severity, originalColor);
            Console.Write(GetFriendlyName(Severity));

            Console.ForegroundColor = originalColor;
            Console.Write(": ");
            Console.WriteLine(GetErrorCodeText(MessageCode));
            Console.WriteLine(GetErrorDescription(MessageCode));


            Console.Write(beforeerror);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(error);
            Console.ForegroundColor = originalColor;
            Console.WriteLine(aftererror);
            
        }

        private ConsoleColor ForSeverity(MessageSeverity severity, ConsoleColor normal)
        {
            if (severity == MessageSeverity.Warning) return ConsoleColor.Yellow;

            if(severity == MessageSeverity.Error || severity == MessageSeverity.Fatal)
                return ConsoleColor.Red;

            return normal;
        }

        private string GetFriendlyName(MessageSeverity severity) => severity.ToString();
        
        /// <summary>
        /// Unique error code for each error type
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Error code as string</returns>
        private string GetErrorCodeText(MessageCode code) => $"CRAWL{(int) code}";

        private string GetErrorDescription(MessageCode code) => $"An error happened with {code}";
    }

    public enum MessageCode
    {
        GenericParseError = (1 << 16) * 0,


        GenericScopeError = (1 << 16) * 1,


        GenericTypeError = (1 << 16) * 2

    }

    public enum MessageSeverity
    {
        /// <summary>
        /// General information that might be helpfull.
        /// </summary>
        Info,

        /// <summary>
        /// Most likeley an error, but it might not be
        /// </summary>
        Warning,

        /// <summary>
        /// Error that prevents successfull compleetion
        /// </summary>
        Error,

        /// <summary>
        /// Error that aborts current work
        /// </summary>
        Fatal
    }
}