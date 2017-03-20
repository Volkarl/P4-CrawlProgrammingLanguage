using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class ForLoopNode : FlowNode
    {
        private TypeNode InducedFieldType { get; }
        private VariableNode InducedFieldName { get; }
        private ExpressionNode Iteratior { get; }
        private BlockNode Block { get;  }

        public ForLoopNode(Interval interval, TypeNode inducedFieldType, VariableNode inducedFieldName, ExpressionNode iteratior, BlockNode block)
            : base(NodeType.Forloop, interval )
        {
            InducedFieldType = inducedFieldType;
            InducedFieldName = inducedFieldName;
            Iteratior = iteratior;
            Block = block;
            ChildCount = 4;
        }

        public override GreenNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return InducedFieldType;
                case 1: return InducedFieldName;
                case 2: return Iteratior;
                case 3: return Block;

                default:
                    return default(GreenNode);
            }

            
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.ForLoopNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new ForLoopNode(Interval, (TypeNode) newChild, InducedFieldName, Iteratior, Block);
                case 1: return new ForLoopNode(Interval, InducedFieldType, (VariableNode) newChild, Iteratior, Block);
                case 2: return new ForLoopNode(Interval, InducedFieldType, InducedFieldName, (ExpressionNode) newChild, Block);
                case 3: return new ForLoopNode(Interval, InducedFieldType, InducedFieldName, Iteratior, (BlockNode) newChild);

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}