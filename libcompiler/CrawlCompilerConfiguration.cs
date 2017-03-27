using System;
using System.Collections.Generic;

namespace libcompiler
{
    public class CrawlCompilerConfiguration
    {
        //TODO: Non insane way of having this be a tuple of (path, Stream)
        public List<string> Files { get; } = new List<string>();

        public TargetStage TargetStage { get; set; } = TargetStage.Compile;

        public HashSet<string> Optimizations { get; } = new HashSet<string>();

        public bool PrintHelp { get; set; }

        public string OutputFile { get; set; }

        public bool ForceSingleThreaded { get; set; }
        

        public static readonly string Instructions =
            /*
             1         2         3         4         5         6         7         8
             12345678901234567890123456789012345678901234567890123456789012345678901234567890*/
            "-h --help                          Print this help message and exit\n" +
            "\n" +
            "Compiler options:" +
            "   --output=file                   Save output to file\n" +
            "-o --optimize                      Optimize compiled exe\n" +
            "   --optimize=optimization1[,...]  Turn on specific optimizations\n" +
            "   --force-single-thread           Forces the compiler to do all compilation on a single thread\n" +
            "\n" +
            "Diagnostic information:\n" +
            "-a --print-ast                     Print all abstract syntax tree\n" +
            "-t --tree                          Print all parse trees\n";

        public static CrawlCompilerConfiguration Parse(string[] args)
        {
            CrawlCompilerConfiguration configuration = new CrawlCompilerConfiguration();
            TargetStage defaultTargetStage = configuration.TargetStage;
            configuration.TargetStage = (TargetStage) (-1);
            for (int index = 0; index < args.Length; index++)
            {
                string arg = args[index];
                if (arg.StartsWith("--"))
                {
                    string longarg = arg.Substring(2);
                    SwitchOnLongargs(configuration, longarg);
                }
                else if (arg.StartsWith("-"))
                {
                    string multiargs = arg.Substring(1);
                    foreach (char c in multiargs)
                    {
                        SwitchOnCharargs(configuration, c);
                    }
                }
                else
                {
                    configuration.Files.Add(arg);
                }
            }

            if (configuration.TargetStage < 0)
                configuration.TargetStage = defaultTargetStage;

            if (args.Length == 0)
                configuration.PrintHelp = true;

            return configuration;
        }

        private static void SwitchOnCharargs(CrawlCompilerConfiguration crawlCompilerConfiguration, char c)
        {
            switch (c)
            {
                case 'h':
                    crawlCompilerConfiguration.PrintHelp = true;
                    break;
                case 'p':
                    SetStage(crawlCompilerConfiguration, TargetStage.ParseTree);
                    break;
                case 'a':
                    SetStage(crawlCompilerConfiguration, TargetStage.AbstractSyntaxTree);
                    break;
                default:
                    throw new UnknownOption(c.ToString());
            }
        }

        private static void SwitchOnLongargs(CrawlCompilerConfiguration crawlCompilerConfiguration, string longarg)
        {
            string[] parts = longarg.Split('=');
            try
            {
                switch (parts[0])
                {
                    case "help":
                        crawlCompilerConfiguration.PrintHelp = true;
                        break;

                    case "print-ast":
                        SetStage(crawlCompilerConfiguration, TargetStage.AbstractSyntaxTree);
                        break;

                    case "print-pt":
                        SetStage(crawlCompilerConfiguration, TargetStage.ParseTree);
                        break;

                    case "typecheck":
                        SetStage(crawlCompilerConfiguration, TargetStage.TypeCheck);
                        break;
                    case "optimize":
                        if (parts.Length == 1)
                            crawlCompilerConfiguration.Optimizations.Add("*");
                        else
                            foreach (string optimiaztion in parts[1].Split(','))
                                crawlCompilerConfiguration.Optimizations.Add(optimiaztion);

                        break;

                    case "output":
                        crawlCompilerConfiguration.OutputFile = parts[1];
                        break;

                    case "force-single-thread":
                        crawlCompilerConfiguration.ForceSingleThreaded = true;
                        break;

                    default:
                        throw new UnknownOption(parts[0]);
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new RequiresArgumentException(parts[0]);
            }
        }

        private static void SetStage(CrawlCompilerConfiguration crawlCompilerConfiguration, TargetStage stage)
        {
            if(crawlCompilerConfiguration.TargetStage >= 0)
                throw new MutalExcluseiveOptionsException(crawlCompilerConfiguration.TargetStage.ToString(), stage.ToString());

            crawlCompilerConfiguration.TargetStage = stage;
        }

        public class MutalExcluseiveOptionsException : Exception
        {
            public string FirstOption { get; }
            public string SecondOption { get; }

            public MutalExcluseiveOptionsException(string firstOption, string secondOption)
            {
                FirstOption = firstOption;
                SecondOption = secondOption;
            }
        }

        public class UnknownOption : Exception
        {
            public UnknownOption(string message) : base(message)
            {
            }
        }

        public class RequiresArgumentException : Exception
        {
            public RequiresArgumentException(string option) : base(option)
            {
            }
        }
    }

    public enum TargetStage
    {
        ParseTree,
        AbstractSyntaxTree,
        //Rest of this is "Probably this order"
        ScopeCheck,
        TypeCheck,
        OptimizedAbstractSyntaxTree,
        Compile
    }
}