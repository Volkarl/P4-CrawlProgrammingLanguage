using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace libcompiler
{
    public class CompilationResult
    {
        public CompilationResult(CompilationStatus status, IEnumerable<CompilationMessage> messages)
        {
            Status = status;
            //This should really not be needed and just accept a IReadonlyCollecection
            //but for some reason C# belives nothing implements IReadonlyCollection...
            Messages = messages.ToList().AsReadOnly();
        }

        public CompilationStatus Status { get; }
        public IReadOnlyCollection<CompilationMessage> Messages { get; }
    }

    

    public enum CompilationStatus
    {
        Failure,
        Success
    }
}