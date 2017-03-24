using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class SingleVariableDecleration : GreenNode
    {
        public VariableNode Identifier { get; }
        public ExpressionNode DefaultValue { get; }

        public SingleVariableDecleration(Interval interval, VariableNode identifier, ExpressionNode defaultValue = null) : base(NodeType.VariableDeclerationSingle, interval)
        {
            Identifier = identifier;
            DefaultValue = defaultValue;


            ChildCount = defaultValue == null ? 1 : 2;
        }

        public override GreenNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return Identifier;
                case 1: return DefaultValue;

                default:
                    return default(GreenNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.SingleVariableDecleration(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new SingleVariableDecleration(Interval, (VariableNode) newChild, DefaultValue);
                case 1: return new SingleVariableDecleration(Interval, Identifier, (ExpressionNode) newChild);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}