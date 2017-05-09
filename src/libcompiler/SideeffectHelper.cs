using System;
using System.Collections.Concurrent;

namespace libcompiler
{
    public class SideeffectHelper
    {
        public ConcurrentBag<CompilationMessage> CompilationMessages { get; } = new ConcurrentBag<CompilationMessage>();

        public Exception FailWith(CompilationMessage message)
        {
            CompilationMessages.Add(message);
            return new ExitStageException();
        }
    }
}