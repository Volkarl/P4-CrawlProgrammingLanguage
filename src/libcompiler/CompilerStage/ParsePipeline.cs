using System;
using System.Collections.Concurrent;
using System.Text;
using Antlr4.Runtime;
using libcompiler.Antlr;
using libcompiler.Parser;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage
{
    internal class ParsePipeline
    {
        private readonly ConcurrentBag<CompilationMessage> _messages;

        public ParsePipeline(ConcurrentBag<CompilationMessage> messages)
        {
            _messages = messages;
        }

        public static Action<string> CreateParsePipeline(ConcurrentBag<AstData> astDestination, ConcurrentBag<CompilationMessage> messages, TargetStage stopAt)
        {
            ParsePipeline pipeline = new ParsePipeline(messages);


            Func<string, ParseTreeData> first = pipeline.ReadFileToPt;
            if (stopAt == TargetStage.ParseTree)
                return first.EndWith(Console.WriteLine);

            var second = first.Then(pipeline.CreateAst);
            if (stopAt == TargetStage.AbstractSyntaxTree)
                return second.EndWith(Console.WriteLine);

            return second.EndWith(astDestination.Add);
        }

        public AstData CreateAst(ParseTreeData pt)
        {
            CrawlSyntaxTree tree = CrawlSyntaxTree.ParseTree(pt.ParseTree, pt.Filename);

            return new AstData(pt.TokenStream, pt.Filename, tree);
        }

        public ParseTreeData ReadFileToPt(string path)
        {

            ICharStream charStream = new AntlrFileStream(path, Encoding.UTF8);
            CrawlLexer tokenSource = new CrawlLexer(charStream);
            ITokenStream tokenStream = new CommonTokenStream(tokenSource);

            CompilationMessageErrorListner cm = new CompilationMessageErrorListner(_messages, path);


            CrawlParser parser = new CrawlParser(tokenStream);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(cm);

            CrawlParser.Translation_unitContext translationUnit = parser.translation_unit();

            return new ParseTreeData(tokenStream, translationUnit, path);

        }
    }
}