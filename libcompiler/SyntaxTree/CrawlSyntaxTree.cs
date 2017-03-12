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

        public static CrawlSyntaxTree ParseTree(TextReader tr, string compilationUnitName)
        {
            
            ITokenSource ts = new CrawlLexer(new AntlrInputStream(tr));
            ITokenStream tstream = new CommonTokenStream(ts);
            CrawlParser parser = new CrawlParser(tstream);

            CrawlParser.Translation_unitContext rootContext = parser.translation_unit();
            
            return new CrawlSyntaxTree(rootContext, compilationUnitName);
        }

        
    }
}
