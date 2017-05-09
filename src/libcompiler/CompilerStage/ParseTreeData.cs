using Antlr4.Runtime;
using libcompiler.Parser;

namespace libcompiler.CompilerStage
{
    internal class ParseTreeData
    {
        public ParseTreeData(ITokenStream tokenStream, CrawlParser.Translation_unitContext parseTree, string filename)
        {
            TokenStream = tokenStream;
            ParseTree = parseTree;
            Filename = filename;
        }

        public override string ToString()
        {
            return $"{Filename}:\n{Utils.MakeIndents(ParseTree.ToStringTree(CrawlParser.ruleNames))}";
        }

        public ITokenStream TokenStream { get; }
        public CrawlParser.Translation_unitContext ParseTree { get; }
        public string Filename { get; }
    }
}