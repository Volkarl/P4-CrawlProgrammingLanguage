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
        private readonly string helpfullmessage;
        public string File { get; }
        public int FirstPoint { get; }

        public CompilationMessage(MessageSeverity severity, MessageCode messageCode, string beforeerror, string error, string aftererror, string helpfullmessage, string file, int firstPoint)
        {
            Severity = severity;
            MessageCode = messageCode;
            this.beforeerror = beforeerror;
            this.error = error;
            this.aftererror = aftererror;
            this.helpfullmessage = helpfullmessage;
            File = file;
            FirstPoint = firstPoint;
        }

        public static CompilationMessage Create(ITokenStream source, Interval errorLocation, MessageCode code, string file,
            string helpfullmessage = null, 
            MessageSeverity severity = MessageSeverity.Error)
        {
            if(source == null) throw new ArgumentNullException(nameof(source));
            if(errorLocation.Equals(Interval.Invalid)) throw new ArgumentOutOfRangeException(nameof(errorLocation));

            //Interval of 10 before, 10 after error
            //TODO: check against 0, source.Lenght
            Interval before = new Interval(Math.Max(errorLocation.a - 11, 0), Math.Max(errorLocation.a -1, 0));
            Interval after = new Interval(Math.Min(errorLocation.b + 1, source.Size), Math.Min(errorLocation.b + 11, source.Size));

            string beforestring = source.GetText(before);
            string error = source.GetText(errorLocation);
            string afterstring = source.GetText(after);


            return new CompilationMessage(severity, code, beforestring, error, afterstring, helpfullmessage, file, errorLocation.a);

        }

        public override string ToString()
        {
            string s = 
                $"In {File}\n{GetFriendlyName(Severity)}: {GetErrorCodeText(MessageCode)}\n{GetErrorDescription(MessageCode)}\n{beforeerror}{error}{aftererror}";

            if (helpfullmessage != null)
            {
                s += "\n";
                s += helpfullmessage;
            }

            return s;
        }

        public void WriteToConsole()
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            Console.WriteLine(File);

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

            if (helpfullmessage != null)
            {
                Console.WriteLine(helpfullmessage);
            }
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

        private string GetErrorDescription(MessageCode code) => $"An error of type {code} ";

        public static CompilationMessage CreateNonCodeMessage(
            MessageCode code,
            string helpfullmessage = null,
            MessageSeverity severity = MessageSeverity.Error)
        {
            return new CompilationMessage(severity, code, "", "", "", helpfullmessage, null, -1);
        }
    }

    public enum MessageCode
    {
        GenericParseError = (1 << 16) * 0,
        UnexpectedSymbol,

        GenericScopeError = (1 << 16) * 1,
        HidesOtherSymbol,
        NoSuchSymbol,
        UseBeforeDecleration,

        GenericTypeError = (1 << 16) * 2,
        TypeNotFound,
        NamespaceNotFound,
        NotMethod,
        InvalidParameterCount,

        OtherError = (1 << 16) * 3,
        FileNotFound,
        InternalCompilerError

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