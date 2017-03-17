using System.IO;
using Antlr4.Runtime;
using libcompiler.Parser;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.SyntaxTree.Parser;

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

        public static CrawlSyntaxTree ParseTree(TextReader textReader, string compilationUnitName)
        {
            //An ITokenSource lets us get the tokens one at a time.
            ITokenSource tSource = new CrawlLexer(new AntlrInputStream(textReader));
            //An ITokenStream lets us go forwards and backwards in the token-series.
            ITokenStream tStream = new CommonTokenStream(tSource);
            //That's what our parser wants.
            CrawlParser parser = new CrawlParser(tStream);

            //The translation_unit is the top rule in our grammar.
            //Asking the parser to match that from the token stream leaves us at the top of the parse tree.
            CrawlParser.Translation_unitContext rootContext = parser.translation_unit();
            Nodes.CompiliationUnitNode node = ParseTreeParser.ParseTranslationUnit(rootContext);

            node.OwningTree.CompilationUnitName = compilationUnitName;
            return node.OwningTree;
        }
    }
}
