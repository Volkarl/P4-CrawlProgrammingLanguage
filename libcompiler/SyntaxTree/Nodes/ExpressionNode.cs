using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class ExpressionNode : CrawlSyntaxNode
    {
        public ExpressionType ExpressionType { get; }

        protected ExpressionNode(CrawlSyntaxNode parrent, Internal.ExpressionNode self, int slot) : base(parrent, self, slot)
        {
            ExpressionType = self.ExpressionType;
        }
    }
}