using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenReturnStatement : GreenCrawlSyntaxNode
    {
        public GreenExpressionNode ReturnValue { get; }

        public GreenReturnStatement(Interval interval, GreenExpressionNode returnValue = null)
            : base(NodeType.Return, interval)
        {
            ReturnValue = returnValue;
            ChildCount = returnValue == null ? 0 : 1;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            if(slot == 0) return ReturnValue;
            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.ReturnStatement(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new GreenReturnStatement(Interval, (GreenExpressionNode) newChild);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}