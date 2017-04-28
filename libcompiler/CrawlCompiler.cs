using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using libcompiler.CompilerStage;

namespace libcompiler
{
    public static class CrawlCompiler
    {
        public static CompilationResult Compile(CrawlCompilerConfiguration configuration)
        {
            ConcurrentBag<CompilationMessage> messages = new ConcurrentBag<CompilationMessage>();
            CompilationStatus status = CompilationStatus.Success;

            try
            {
                ConcurrentBag<AstData> files = new ConcurrentBag<AstData>();
                bool parallel = !configuration.ForceSingleThreaded;
                
                Execute(configuration.Files, ParsePipeline.CreateParsePipeline(files, messages, configuration.TargetStage), parallel);

                //TODO: Collect information on referenced assemblies

                //TODO: Semantic analysis

                //TODO: Interpeter or code generation

                if (messages.Count(message => message.Severity >= MessageSeverity.Error) > 0)
                    status = CompilationStatus.Failure;
                
            }
            catch (Exception e)
            {
                messages.Add(CompilationMessage.CreateNonCodeMessage(MessageCode.InternalCompilerError, e.StackTrace, MessageSeverity.Fatal));
                status = CompilationStatus.Failure;
            }

            return new CompilationResult(status, messages);
        }

        private static void Execute<TIn>(IEnumerable<TIn> indata, Action<TIn> action, bool parallel) where TIn : class
        {
            if (parallel)
            {
                Task.WaitAll(indata.Select(item => Task.Run(() => action(item))).ToArray());
            }
            else
            {
                foreach (TIn @in in indata)
                {
                    action(@in);
                }
            }
        }
    }
}
 