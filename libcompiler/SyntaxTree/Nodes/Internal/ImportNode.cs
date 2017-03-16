using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class ImportNode : GreenNode
    {
        public string Module { get; }

        public ImportNode(Interval interval, string module)
            : base(NodeType.Import, interval)
        {
            Module = module;
        }

        public override GreenNode GetSlot(int slot)
        {
            throw new System.NotImplementedException();
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            throw new System.NotImplementedException();
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new System.NotImplementedException();
        }
    }
}