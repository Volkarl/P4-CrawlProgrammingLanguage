namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class FlowNode : CrawlSyntaxNode
    {
        protected FlowNode(CrawlSyntaxNode parent, Internal.FlowNode self, int indexInParent) : base(parent, self, indexInParent)
        {
        }
    }
}