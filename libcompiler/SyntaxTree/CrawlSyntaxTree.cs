using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime;
using libcompiler.Parser;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.SyntaxTree.Nodes.Internal;
using libcompiler.SyntaxTree.Parser;
using BlockNode = libcompiler.SyntaxTree.Nodes.BlockNode;
using ImportNode = libcompiler.SyntaxTree.Nodes.ImportNode;

namespace libcompiler.SyntaxTree
{
    public class CrawlSyntaxTree
    {
        public string CompilationUnitName { get; }
        public CrawlSyntaxNode RootNode { get; private set; }
        

        private CrawlSyntaxTree(CrawlParser.Translation_unitContext rootContext, string compilationUnitName)
        {
            CompilationUnitName = compilationUnitName;
            
            CrawlParser.Import_directivesContext imports =
                (CrawlParser.Import_directivesContext)rootContext.GetChild(0);

            CrawlParser.StatementsContext statements =
                (CrawlParser.StatementsContext)rootContext.GetChild(1);

            
            BlockNode rootBlock = new ParseTreeParser(this).ParseBlockNode(statements);

            

            RootNode = NodeFactory.CompilationUnit(rootContext.SourceInterval, new List<ImportNode>(), rootBlock);
        }

        private CrawlSyntaxTree(GreenNode root, string name)
        {
            CompilationUnitName = name;
            RootNode = root.CreateRed(new Internal.SyntaxNodeTreeInjector(this, root, 0), 0);


        }

        internal CrawlSyntaxTree(CrawlSyntaxNode node, string name)
        {
            RootNode = node;
            CompilationUnitName = name;
        }

        internal static CrawlSyntaxTree FromGreen(GreenNode root, string name) => new CrawlSyntaxTree(root, name);


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
            
            return new CrawlSyntaxTree(rootContext, compilationUnitName);
        }

        
    }
}
