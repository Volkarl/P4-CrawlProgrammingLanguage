using libcompiler.SyntaxTree.Nodes;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Internal
{
    public class SyntaxNodeTreeInjector : CrawlSyntaxNode
    {
        public SyntaxNodeTreeInjector(CrawlSyntaxTree tree, GreenNode self, int slot) : base(tree, self, slot)
        {
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}