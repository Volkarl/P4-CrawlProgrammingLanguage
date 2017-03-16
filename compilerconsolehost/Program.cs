using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using libcompiler;
using libcompiler.Parser;
using libcompiler.SyntaxTree;

namespace compilerconsolehost
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            List<string> flags = new List<string>();
            List<string> files = new List<string>();
            while ((i < args.Length) && (args[i].FirstOrDefault()=='-'))
            {
                flags.Add(args[i]);
                i++;
            }
            for (i=i; i < args.Length; i++)
            {
                files.Add(args[i]);
            }

            Options options = new Options(flags, files);

            if(options.Help)
                Console.WriteLine(options.Instructions);
            foreach (string file in options.Files)
            {
                if(options.Compile)
                    Console.WriteLine(new NotImplementedException());
                if(options.WriteAST)
                    PrintAbstractSyntaxTree(file);
                if(options.WriteST)
                    PrintFullSyntaxTree(file);
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static void PrintFullSyntaxTree(string s)
        {
            Console.WriteLine("Full syntax tree for {0}", s);
            try
            {
                //First we need to open specified file.
                AntlrFileStream fs = new AntlrFileStream(s, Encoding.UTF8);
                //An ITokenSource lets us get the tokens one at a time.
                ITokenSource ts = new CrawlLexer(fs);
                //An ITokenStream lets us go forwards and backwards in the token-series.
                ITokenStream tstream = new CommonTokenStream(ts);
                //And then we can have our parser.
                CrawlParser parser = new CrawlParser(tstream);

                //The translation_unit is the top rule in our grammar.
                //Asking the parser to match that from the token stream will have it go through all children too.
                CrawlParser.Translation_unitContext rootContext = parser.translation_unit();

                string debugText = MakeIndents(rootContext.ToStringTree(parser));
                Console.WriteLine(debugText);
                //File.WriteAllText("debug.txt", debugText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void PrintAbstractSyntaxTree(string s)
        {
            CrawlSyntaxTree tree = CrawlSyntaxTree.ParseTree(new StreamReader(File.OpenRead(s)), s);

            var t = new SyntaxTreePrinter();
            t.Visit(tree.RootNode);
            Console.WriteLine(t.BuildString.ToString());
        }

        private static string MakeIndents(string toStringTree)
        {
            StringBuilder sb = new StringBuilder();
            int indent = 0;
            foreach (char c in toStringTree)
            {
                if (c == '(')
                {
                    indent++;
                    sb.Append('\n');
                    for (int i = 0; i < indent; i++)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(c);
                }
                else if (c == ')')
                {
                    indent--;
                    sb.Append(c);
                    sb.Append('\n');
                    for (int i = 0; i < indent; i++)
                    {
                        sb.Append(' ');
                    }
                }
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
