using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Parser;

namespace libcompiler
{
    public class CrawlImpossibleStateException : CrawlSyntaxException
    {
        public CrawlImpossibleStateException(string description, Interval place) : base(description, place)
        {
            
        }
    }
}