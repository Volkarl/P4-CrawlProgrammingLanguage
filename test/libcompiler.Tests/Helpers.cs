using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using libcompiler.Parser;
using libcompiler.SyntaxTree;
using NUnit.Framework;

namespace libcompiler.Tests
{
    static class Helpers
    {
        
        private static string GetFileName([CallerFilePath] string path = "")
        {
            return path;
        }

        private static readonly string TestCaseFolderPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(GetFileName())), "testcases");

        public static TranslationUnitNode ReadTestFile(string name)
        {
            string fullpath = Path.Combine(TestCaseFolderPath, name);

            TextReader textReader = new StreamReader(File.OpenRead(fullpath));
            //An ITokenSource lets us get the tokens one at a time.
            ITokenSource tSource = new CrawlLexer(new AntlrInputStream(textReader));
            //An ITokenStream lets us go forwards and backwards in the token-series.
            ITokenStream tStream = new CommonTokenStream(tSource);
            return (TranslationUnitNode) CrawlSyntaxTree.ParseTree(tStream, fullpath).RootNode;
        }
    }
}
