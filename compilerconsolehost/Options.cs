using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace compilerconsolehost
{
    public class Options
    {
        public List<string> Files;

        private static readonly string[] HelpFlags = {"-h", "--help"};
        public bool Help = false;

        private static readonly string[] WriteAstFlags = {"-a", "--abstract-tree"};
        public bool WriteAST = true;

        private static readonly string[] WriteStFlags = {"-t", "--tree"};
        public bool WriteST = false;

        private static readonly string[] CompileFlags = {"-c", "--compile"};
        public bool Compile = false;

        public readonly string Instructions =
            "-h --help             what you see now\n" +
            "Parse trees:\n" +
            "-a --abstract-tree    print abstract syntax tree\n" +
            "-t --tree             print full syntax tree\n" +
            "Compiling:\n" +
            "-c --compile          compile source program(not implemented)\n";

        public Options(List<string> flags, List<string> files)
        {
            Files = files;

            //Otherwise keep default values.
            if(flags.Count>0)
            {
                Help = false;
                WriteAST = false;
                WriteST = false;
                Compile = false;
                foreach (string s in flags)
                {
                    if (Corresponding(s, HelpFlags))
                        Help = true;
                    if (Corresponding(s, WriteAstFlags))
                        WriteAST = true;
                    if(Corresponding(s, WriteStFlags))
                        WriteST = true;
                    if (Corresponding(s, CompileFlags))
                        Compile = true;
                }
            }
        }

        private bool Corresponding(string s, string[] flags)
        {
            bool result = false;
            foreach (string flag in flags)
            {
                if (flag.CompareTo(s) == 0)
                    result = true;
            }
            return result;
        }
    }
}