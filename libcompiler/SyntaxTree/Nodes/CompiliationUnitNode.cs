using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class CompiliationUnitNode : CrawlSyntaxNode
    {
        private ListNode<ImportNode> _imports;
        private BlockNode _code;

        //This should plausibly be 2 lists. 1 of All declarations (functions/classes/namespaces) and 1 of statements;
        //And maybe even a third, imports;
        public CompiliationUnitNode(CrawlSyntaxNode parent, GreenNode self, int slot) : base(parent, self, slot)
        {
            
        }

        public ListNode<ImportNode> Imports => GetRed(ref _imports, 0);
        public BlockNode Code => GetRed(ref _code, 1);
        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return Imports;
                case 1: return Code;
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}