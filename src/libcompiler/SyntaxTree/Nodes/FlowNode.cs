namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class FlowNode : CrawlSyntaxNode
    {
        protected FlowNode(CrawlSyntaxNode parent, Internal.GreenFlowNode self, int indexInParent) : base(parent, self, indexInParent)
        {
        }
    }
}