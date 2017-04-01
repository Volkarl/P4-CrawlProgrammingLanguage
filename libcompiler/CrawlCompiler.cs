using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.TypeSystem;

namespace libcompiler
{
    public static class CrawlCompiler
    {
        public static void Compile(CrawlCompilerConfiguration configuration)
        {
            TextWriter output = Utils.GetPrimaryOutputStream(configuration);
            if (configuration.TargetStage == TargetStage.ParseTree)
            {
                ParseTreeHelper.WriteParseTrees(output, configuration);
            }

            List<CrawlSyntaxTree> parsedFiles =
                configuration.Files.ConfigureableParallelSelect(!configuration.ForceSingleThreaded,
                    ParseFileToAst).ToList();

            if (configuration.TargetStage == TargetStage.AbstractSyntaxTree)
            {
                foreach (CrawlSyntaxTree crawlSyntaxTree in parsedFiles)
                {
                    SuperPrettyPrintVisitor printer = new SuperPrettyPrintVisitor(true);
                    string s = printer.PrettyPrint(crawlSyntaxTree.RootNode);
                    output.WriteLine("File {0}:", crawlSyntaxTree.CompilationUnitName);
                    output.WriteLine(s);
                }
                return;
            }

            //TODO: Some kind of merge of top scopes so files can see methods/classes/whatever decleared in all files here i think

            List<string> v = parsedFiles
                .ConfigureableParallelSelect(!configuration.ForceSingleThreaded,
                    Stage_TypeCheckAst)
                .Aggregate(Enumerable.Concat).ToList();

            if (configuration.TargetStage == TargetStage.TypeCheck)
            {
                foreach (CrawlSyntaxTree syntaxTree in parsedFiles)
                {
                    SuperPrettyPrintVisitor printer = new SuperPrettyPrintVisitor(false, node =>
                    {
                        TypeChecker.TypeChecker typechecker = new TypeChecker.TypeChecker();
                        try
                        {
                            CrawlType type = typechecker.Visit(node);
                            if (type == CrawlType.Void) return null;
                            return type?.ToString();
                        }
                        catch (Exception e)
                        {
                            return null;
                        }
                    });

                    string s = printer.PrettyPrint(syntaxTree.RootNode);
                    output.WriteLine("File {0}:", syntaxTree.CompilationUnitName);
                    output.WriteLine(s);

                }
            }

        }

        private static IEnumerable<string> Stage_TypeCheckAst(CrawlSyntaxTree tree)
        {
            TypeChecker.TypeChecker checker = new TypeChecker.TypeChecker();
            checker.Visit(tree.RootNode);
            return checker.TypeErrors;
        }

        //TODO: A method that takes same arguments as compile but returns set of decorated ast instead of writing to file
        //TODO: if ast is different from decorated ast, 2 methods that take each other

        //TODO: A method that takes decorated ast instead and writes output to file

        private static CrawlSyntaxTree ParseFileToAst(string arg)
        {
            return CrawlSyntaxTree.ParseTree(new StreamReader(File.OpenRead(arg)), arg);
        }
    }
}
