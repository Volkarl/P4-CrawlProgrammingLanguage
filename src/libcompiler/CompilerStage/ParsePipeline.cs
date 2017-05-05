using System.Text;
using Antlr4.Runtime;
using libcompiler.Antlr;
using libcompiler.Parser;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage
{
    internal static class ParsePipeline
    {
        public static  AstData CreateAst(ParseTreeData pt, SideeffectHelper helper)
        {
            CrawlSyntaxTree tree = CrawlSyntaxTree.ParseTree(pt.ParseTree, pt.Filename);

            return new AstData(pt.TokenStream, pt.Filename, tree);
        }

        public static ParseTreeData ReadFileToPt(string path, SideeffectHelper helper)
        {
            ICharStream charStream = new AntlrFileStream(path, Encoding.UTF8);
            CrawlLexer tokenSource = new CrawlLexer(charStream);
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);

            CompilationMessageErrorListner cm = new CompilationMessageErrorListner(helper.CompilationMessages, path);

            CrawlParser parser = new CrawlParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(cm);

            CrawlParser.Translation_unitContext translationUnit = parser.translation_unit();

            return new ParseTreeData(tokenStream, translationUnit, path);

        }
    }
}