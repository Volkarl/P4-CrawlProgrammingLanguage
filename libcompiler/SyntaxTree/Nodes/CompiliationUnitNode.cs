using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class CompiliationUnitNode : CrawlSyntaxNode
    {
        //This should plausibly be 2 lists. 1 of All declarations (functions/classes/namespaces) and 1 of statements;
        //And maybe even a third, imports;
        public CompiliationUnitNode(CrawlSyntaxTree owningTree, Interval interval, BlockNode codeChildren,
            IEnumerable<ImportNode> imports) : base(owningTree, NodeType.CompilationUnit, interval)
        {
            Code = codeChildren;
            Imports = imports.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ImportNode> Imports;
        public BlockNode Code { get; }
    }
}