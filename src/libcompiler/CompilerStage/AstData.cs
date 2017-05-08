using Antlr4.Runtime;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage
{
    internal class AstData
    {
        public AstData(ITokenStream tokenStream, string filename, CrawlSyntaxTree tree)
        {
            TokenStream = tokenStream;
            Filename = filename;
            Tree = tree;
        }

        public ITokenStream TokenStream { get; }
        public string Filename { get;  }
        public CrawlSyntaxTree Tree { get; }

        public override string ToString()
        {
            SuperPrettyPrintVisitor printer = new SuperPrettyPrintVisitor(true);
            return printer.PrettyPrint(Tree.RootNode);
        }
    }
}