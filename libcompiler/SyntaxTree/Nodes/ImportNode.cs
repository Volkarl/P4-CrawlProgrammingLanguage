using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ImportNode : CrawlSyntaxNode
    {
        public string Module { get; }

        public ImportNode(CrawlSyntaxNode parrent, Internal.ImportNode self, int slot) : base(parrent, self, slot)
        {
            Module = self.Module;
        }

        public override CrawlSyntaxNode GetChild(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}