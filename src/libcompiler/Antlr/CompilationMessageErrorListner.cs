using System.Collections.Concurrent;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using libcompiler.Parser;

namespace libcompiler.Antlr
{
    class CompilationMessageErrorListner : IAntlrErrorListener<IToken>
    {
        private readonly string _file;

        public CompilationMessageErrorListner(ConcurrentBag<CompilationMessage> messages, string file)
        {
            _file = file;
            Errors = messages;
        }

        ConcurrentBag<CompilationMessage> Errors { get;  }
        public bool AnyErrors { get; private set; }
        
        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            AnyErrors = true;
            CrawlParser parser = (CrawlParser) recognizer;

            IntervalSet set = parser.GetExpectedTokens();

            string help = 
                $"Symbolet {0} er ikke tilladt på dette punkt. En af de følgende var forventet:\n\t" +
                string.Join("\n\t", set.ToArray().Select(x => parser.Vocabulary.GetDisplayName(x)));

            int intrestinglocation = offendingSymbol.TokenIndex;
            //for (int i = intrestinglocation; i < offendingSymbol.TokenIndex + 20; i++)
            //{
            //    if (parser.TokenStream.Get(i).Channel == Lexer.DefaultTokenChannel)
            //    {
            //        intrestinglocation = i;
            //        break;
            //    }
            //}


            Errors.Add(CompilationMessage.Create(
                parser.TokenStream,
                Interval.Of(intrestinglocation, intrestinglocation), 
                MessageCode.UnexpectedSymbol, 
                $"{_file}:{line},{charPositionInLine}",
                help, 
                MessageSeverity.Error));
        }
    }
}
