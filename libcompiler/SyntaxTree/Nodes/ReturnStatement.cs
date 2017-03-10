using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ReturnStatement : CrawlSyntaxNode
    {
        private ExpressionNode _retval;
        public ExpressionNode ReturnValue => GetRed(ref _retval, 0);

        public ReturnStatement(CrawlSyntaxNode parrent, Internal.ReturnStatement self, int slot) : base(parrent, self, slot)
        {
            
        }
    }
}