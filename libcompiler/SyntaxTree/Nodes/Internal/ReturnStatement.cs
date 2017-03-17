using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class ReturnStatement : GreenNode
    {
        public ExpressionNode ReturnValue { get; }

        public ReturnStatement(Interval interval, ExpressionNode returnValue = null)
            : base(NodeType.Return, interval)
        {
            ReturnValue = returnValue;
        }

        public override GreenNode GetChildAt(int slot)
        {
            if(slot == 0) return ReturnValue;
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.ReturnStatement(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new System.NotImplementedException();
        }
    }
}