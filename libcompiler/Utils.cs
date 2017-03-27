using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace libcompiler
{
    internal static class Utils
    {
        public static TextWriter GetPrimaryOutputStream(CrawlCompilerConfiguration configuration)
        {
            if(string.IsNullOrWhiteSpace(configuration.OutputFile))
                return Console.Out;



            return new StreamWriter(File.OpenWrite(configuration.OutputFile));
        }

        public static string MakeIndents(string lispyString)
        {
            StringBuilder sb = new StringBuilder();
            int indent = 0;
            foreach (char c in lispyString)
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

            string retstring = sb.ToString();
            return retstring;
        }

        public static  IEnumerable<TOut> ConfigureableParallelSelect<TIn, TOut>(
            this IEnumerable<TIn> input, 
            bool parallel,
            Func<TIn, TOut> transform)
        {
            if (parallel)
            {
                return input.AsParallel().Select(transform);
            }
            else
            {
                return input.Select(transform);
            }
        }
    }
}