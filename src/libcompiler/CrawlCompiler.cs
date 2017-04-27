using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using libcompiler.Parser;
using libcompiler.SyntaxTree;

namespace libcompiler
{
    public static class CrawlCompiler
    {
        public static CompilationResult Compile(CrawlCompilerConfiguration configuration)
        {
            //The ConcurrentBag is an unordered 
            ConcurrentBag<CompilationMessage> messages = new ConcurrentBag<CompilationMessage>();
            TextWriter output = Utils.GetPrimaryOutputStream(configuration);

            if (configuration.TargetStage == TargetStage.ParseTree)
            {
                ParseTreeHelper.WriteParseTrees(output, configuration);
            }

            //TODO: If filenotfound, log Fatal message and exit
            List<TreeAndStream> parsedFiles = configuration.Files.ConfigureableParallelSelect(!configuration.ForceSingleThreaded,
                    ParseFileToAst).ToList();

            if (configuration.TargetStage == TargetStage.AbstractSyntaxTree)
            {
                foreach (TreeAndStream crawlSyntaxTree in parsedFiles)
                {
                    SuperPrettyPrintVisitor printer = new SuperPrettyPrintVisitor(true);
                    string s = printer.PrettyPrint(crawlSyntaxTree.Tree.RootNode);
                    output.WriteLine("File {0}:", crawlSyntaxTree.Tree.CompilationUnitName);
                    output.WriteLine(s);
                }
            }

            if (configuration.TargetStage == TargetStage.TypeCheck)
            {
                foreach (TreeAndStream syntaxTree in parsedFiles)
                {
                    SuperPrettyPrintVisitor printer = new SuperPrettyPrintVisitor(false);
                    string s = printer.PrettyPrint(syntaxTree.Tree.RootNode);
                    output.WriteLine("File {0}:", syntaxTree.Tree.CompilationUnitName);
                    output.WriteLine(s);
                }
            }


            return new CompilationResult(CompilationStatus.Failure, messages);

        }

        //TODO: A method that takes same arguments as compile but returns set of decorated ast instead of writing to file
        //TODO: if ast is different from decorated ast, 2 methods that take each other

        //TODO: A method that takes decorated ast instead and writes output to file

        private static TreeAndStream ParseFileToAst(string arg)
        {
            TextReader textReader = new StreamReader(File.OpenRead(arg));
            //An ITokenSource lets us get the tokens one at a time.
            ITokenSource tSource = new CrawlLexer(new AntlrInputStream(textReader));
            //An ITokenStream lets us go forwards and backwards in the token-series.
            ITokenStream tStream = new CommonTokenStream(tSource);

            return new TreeAndStream(CrawlSyntaxTree.ParseTree(tStream, arg), tStream);
        }

        private class TreeAndStream
        {
            public CrawlSyntaxTree Tree { get; }
            public ITokenStream TokenStream { get; }

            public TreeAndStream(CrawlSyntaxTree tree, ITokenStream tStream)
            {
                this.Tree = tree;
                this.TokenStream = tStream;
            }
        }
    }

    public class ReplaceLocalVariablesRewriter : SyntaxRewriter
    {
        protected override CrawlSyntaxNode VisitIntegerLiteral(IntegerLiteralNode integerLiteral)
        {
            return CrawlSyntaxNode.IntegerLiteral(integerLiteral.Interval, 9001);
        }
    }
}
