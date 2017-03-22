using System;
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
            ChildCount = returnValue == null ? 0 : 1;
        }

        public override GreenNode GetChildAt(int slot)
        {
            if(slot == 0) return ReturnValue;
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.ReturnStatement(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new ReturnStatement(Interval, (ExpressionNode) newChild);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}