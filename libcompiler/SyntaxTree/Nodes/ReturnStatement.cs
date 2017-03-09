using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ReturnStatement : CrawlSyntaxNode
    {
        public ExpressionNode ReturnValue { get; }

        public ReturnStatement(CrawlSyntaxTree owningTree, Interval interval, ExpressionNode returnValue = null)
            : base(owningTree, NodeType.Return, interval)
        {
            ReturnValue = returnValue;
        }
    }
}