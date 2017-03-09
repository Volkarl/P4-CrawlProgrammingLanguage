using System;
using Antlr4.Runtime.Misc;

namespace libcompiler
{
    public abstract class CrawlSyntaxException : Exception
    {
        public Interval Place { get; }
        protected CrawlSyntaxException (string description, Interval place) : base(description)
        {
            Place = place;
        }
    }
}