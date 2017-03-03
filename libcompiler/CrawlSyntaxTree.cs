using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using libcompiler.Parser;

namespace libcompiler
{
    public class CrawlSyntaxTree
    {
        public string CompilationUnitName { get; }
        public CrawlSyntaxNode RootNode { get; private set; }


        private CrawlSyntaxTree(CrawlParser.Translation_unitContext rootContext, string compilationUnitName)
        {
            CompilationUnitName = compilationUnitName;
            RootNode = CrawlSyntaxNode.Parse(rootContext, this);
        }

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
