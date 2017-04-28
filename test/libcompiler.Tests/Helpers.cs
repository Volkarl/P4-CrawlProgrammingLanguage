using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;
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
            return (TranslationUnitNode) CrawlSyntaxTree.ReadFile(fullpath).RootNode;
        }
    }
}
