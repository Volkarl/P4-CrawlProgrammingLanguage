using System.IO;
using Antlr4.Runtime;
using libcompiler.Parser;

namespace libcompiler
{
    public static class ParseTreeHelper
    {
        private static CrawlParser.Translation_unitContext ParseFile(Stream file)
        {
            //First we need to open specified file.
            AntlrInputStream fs = new AntlrInputStream(file);
            //An ITokenSource lets us get the tokens one at a time.
            ITokenSource ts = new CrawlLexer(fs);
            //An ITokenStream lets us go forwards and backwards in the token-series.
            ITokenStream tstream = new CommonTokenStream(ts);
            //That's what our parser wants.
            CrawlParser parser = new CrawlParser(tstream);

            return parser.translation_unit();
        }

        internal static void WriteParseTrees(TextWriter output, CrawlCompilerConfiguration configuration)
        {
            foreach (string inputfile in configuration.Files)
            {
                output.WriteLine("Parsing file {0}", inputfile);
                CrawlParser.Translation_unitContext tu = ParseFile(File.OpenRead(inputfile));
                //foreach (string line in Utils.MakeIndents(tu.ToStringTree(CrawlParser.ruleNames)).Split('\n'))
                //{
                //    output.WriteLine(line);
                //}
                output.WriteLine(Utils.MakeIndents(tu.ToStringTree(CrawlParser.ruleNames)));
            }
        }
    }
}