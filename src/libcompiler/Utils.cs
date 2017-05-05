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

        public static Action<T1> EndWith<T1, T2>(this Func<T1, T2> first, Action<T2> sink)
        {
            return x =>
            {
                if (ReferenceEquals(x, default(T1)))
                    return;

                T2 f = first(x);

                if (ReferenceEquals(f, default(T2)))
                    return;

                sink(f);    
            };
        }

        public static Func<T1, T3> Then<T1, T2, T3>(this Func<T1, T2> first, Func<T2, T3> second)
        {
            return x =>
            {
                //second(first(x)) with jump out if anything is null
                if (ReferenceEquals(x, default(T1))) return default(T3);
                T2 f = first(x);
                if (ReferenceEquals(f, default(T2))) return default(T3);
                return second(f);
            };
        }
    }
}