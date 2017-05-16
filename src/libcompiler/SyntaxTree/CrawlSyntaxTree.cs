using System.Text;
using Antlr4.Runtime;
using libcompiler.Parser;
using libcompiler.SyntaxTree.Parser;
using libcompiler.TypeChecker;

namespace libcompiler.SyntaxTree
{
    public class CrawlSyntaxTree
    {
        public string CompilationUnitName { get; private set; }
        public CrawlSyntaxNode RootNode { get; }
        

        internal CrawlSyntaxTree(CrawlSyntaxNode node, string name)
        {
            RootNode = node;
            CompilationUnitName = name;
        }

        public static CrawlSyntaxTree ReadFile(string filename)
        {
            ICharStream charStream = new AntlrFileStream(filename, Encoding.UTF8);
            CrawlLexer tokenSource = new CrawlLexer(charStream);
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);
            CrawlParser parser = new CrawlParser(tokenStream);
            
            
            //The translation_unit is the top rule in our grammar.
            //Asking the parser to match that from the token stream leaves us at the top of the parse tree.
            CrawlParser.Translation_unitContext rootContext = parser.translation_unit();


            TranslationUnitNode node = ParseTreeParser.ParseTranslationUnit(rootContext);

            node.OwningTree.CompilationUnitName = filename;

            return node.OwningTree;
        }

        internal static CrawlSyntaxTree ParseTree(CrawlParser.Translation_unitContext context, string name)
        {
            TranslationUnitNode node = ParseTreeParser.ParseTranslationUnit(context);
            node.OwningTree.CompilationUnitName = name;

            return node.OwningTree;
        }
    }
}
