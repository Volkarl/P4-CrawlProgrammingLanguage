using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using libcompiler.Parser;

namespace compilerconsolehost
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string s in args)
            {
                Console.WriteLine("Testing on {0}", s);
                try
                {
                    AntlrFileStream fs = new AntlrFileStream(s, Encoding.UTF8);
                    ITokenSource ts = new CrawlLexer(fs);
                    ITokenStream tstream = new CommonTokenStream(ts);
                    CrawlParser parser = new CrawlParser(tstream);

                    CrawlParser.ProgramContext rootContext = parser.program();


                    Console.WriteLine(Unfuck(rootContext.ToStringTree(parser)));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            Console.WriteLine("Finished parsing. Press enter to exit...");
            Console.ReadLine();
        }

        private static string Unfuck(string toStringTree)
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
