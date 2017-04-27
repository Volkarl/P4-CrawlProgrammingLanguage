using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace libcompiler
{
    public class ParserErrorHandler : Antlr4.Runtime.IAntlrErrorStrategy
    {
        public void Reset(Antlr4.Runtime.Parser recognizer)
        {
            throw new NotImplementedException();
        }

        public IToken RecoverInline(Antlr4.Runtime.Parser recognizer)
        {
            throw new NotImplementedException();
        }

        public void Recover(Antlr4.Runtime.Parser recognizer, RecognitionException e)
        {
            throw new NotImplementedException();
        }

        public void Sync(Antlr4.Runtime.Parser recognizer)
        {
            throw new NotImplementedException();
        }

        public bool InErrorRecoveryMode(Antlr4.Runtime.Parser recognizer)
        {
            throw new NotImplementedException();
        }

        public void ReportMatch(Antlr4.Runtime.Parser recognizer)
        {
            throw new NotImplementedException();
        }

        public void ReportError(Antlr4.Runtime.Parser recognizer, RecognitionException e)
        {
            throw new NotImplementedException();
        }
    }
}
