using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// Represents an entire translatio unit (Single source file of a program)
    /// </summary>
    public class TranslationUnitNode : CrawlSyntaxNode
    {
        private ListNode<ImportNode> _imports;
        private NameSpaceNode _nameSpace;
        private BlockNode _code;

        public TranslationUnitNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            
        }


        public ListNode<ImportNode> Imports => GetRed(ref _imports, 0);
        public NameSpaceNode NameSpace => GetRed(ref _nameSpace, 1);
        public BlockNode Code => GetRed(ref _code, 2);

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return Imports;
                case 1: return NameSpace;
                case 2: return Code;
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}