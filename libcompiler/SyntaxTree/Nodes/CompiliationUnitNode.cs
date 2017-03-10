using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class CompiliationUnitNode : CrawlSyntaxNode
    {
        private ListNode<ImportNode> _imports;
        private BlockNode _code;

        //This should plausibly be 2 lists. 1 of All declarations (functions/classes/namespaces) and 1 of statements;
        //And maybe even a third, imports;
        public CompiliationUnitNode(CrawlSyntaxNode parrent, GreenNode self, int slot) : base(parrent, self, slot)
        {
            
        }

        public ListNode<ImportNode> Imports => GetRed(ref _imports, 0);
        public BlockNode Code => GetRed(ref _code, 1);
    }
}