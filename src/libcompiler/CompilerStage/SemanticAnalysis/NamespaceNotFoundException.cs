using System;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage.SemanticAnalysis
{
    internal class NamespaceNotFoundException : Exception
    {
        public ImportNode FaultyNode { get; }

        public NamespaceNotFoundException(ImportNode faultyNode)
        {
            FaultyNode = faultyNode;
        }
    }
}