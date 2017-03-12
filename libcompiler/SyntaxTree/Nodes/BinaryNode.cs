using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class BinaryNode : ExpressionNode
    {
        private ExpressionNode _lhs;
        private ExpressionNode _rhs;

        public ExpressionNode LeftHandSide => GetRed(ref _lhs, 0);
        public ExpressionNode RightHandSide => GetRed(ref _rhs, 1);

        public BinaryNode(CrawlSyntaxNode parrent, Internal.ExpressionNode self, int slot)
            : base(parrent, self, slot)
        {
            
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }

        public override CrawlSyntaxNode GetChild(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}