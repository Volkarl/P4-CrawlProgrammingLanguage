using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenSingleVariableDecleration : GreenCrawlSyntaxNode
    {
        public GreenVariableNode Identifier { get; }
        public GreenExpressionNode DefaultValue { get; }

        public GreenSingleVariableDecleration(Interval interval, GreenVariableNode identifier, GreenExpressionNode defaultValue = null) : base(NodeType.VariableDeclerationSingle, interval)
        {
            Identifier = identifier;
            DefaultValue = defaultValue;


            ChildCount = defaultValue == null ? 1 : 2;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return Identifier;
                case 1: return DefaultValue;

                default:
                    return default(GreenCrawlSyntaxNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.SingleVariableDecleration(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new GreenSingleVariableDecleration(Interval, (GreenVariableNode) newChild, DefaultValue);
                case 1: return new GreenSingleVariableDecleration(Interval, Identifier, (GreenExpressionNode) newChild);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}