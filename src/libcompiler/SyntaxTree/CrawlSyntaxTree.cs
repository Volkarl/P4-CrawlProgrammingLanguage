using System.IO;
using Antlr4.Runtime;
using libcompiler.Parser;
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

        public static CrawlSyntaxTree ParseTree(ITokenStream tStream, string compilationUnitName)
        {
            CrawlParser parser = new CrawlParser(tStream);

            //The translation_unit is the top rule in our grammar.
            //Asking the parser to match that from the token stream leaves us at the top of the parse tree.
            CrawlParser.Translation_unitContext rootContext = parser.translation_unit();


            TranslationUnitNode node = ParseTreeParser.ParseTranslationUnit(rootContext);

            node.OwningTree.CompilationUnitName = compilationUnitName;
            return node.OwningTree;
        }
    }
}
