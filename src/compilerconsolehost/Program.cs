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
            CrawlCompilerConfiguration configuration;
            if (ParseOptions(args, out configuration))
            {
                CompilationResult result = CrawlCompiler.Compile(configuration);

                string line = new String('=', Console.BufferWidth);
                foreach (CompilationMessage message in result.Messages.OrderBy(message => message.Severity))
                {
                    message.WriteToConsole();
                    Console.Write(line);
                }
            }
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
        }

        private static bool ParseOptions(string[] args, out CrawlCompilerConfiguration crawlCompilerConfiguration)
        {
            try
            {
                crawlCompilerConfiguration = CrawlCompilerConfiguration.Parse(args);
                if (crawlCompilerConfiguration.PrintHelp)
                {
                    Console.WriteLine(CrawlCompilerConfiguration.Instructions);
                    return false;
                }
                return true;


            }
            catch (CrawlCompilerConfiguration.UnknownOption ex)  
            {
                Console.WriteLine("Unknown option {0}. See --help for all options", ex.Message);   
            }
            catch (CrawlCompilerConfiguration.MutalExcluseiveOptionsException ex)
            {
                Console.WriteLine("The 2 arguments {0} and {1} are mutally exclusive.", ex.FirstOption, ex.SecondOption);
            }
            catch (CrawlCompilerConfiguration.RequiresArgumentException ex)
            {
                Console.WriteLine("The option {0} requires an argument. See --help for details", ex.Message);
            }
            crawlCompilerConfiguration = null;
            return false;
        }
    }
}
