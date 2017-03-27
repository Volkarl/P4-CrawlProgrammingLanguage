using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenForLoopNode : GreenFlowNode
    {
        private GreenTypeNode InducedFieldType { get; }
        private GreenVariableNode InducedFieldName { get; }
        private GreenExpressionNode Iteratior { get; }
        private GreenBlockNode GreenBlock { get;  }

        public GreenForLoopNode(Interval interval, GreenTypeNode inducedFieldType, GreenVariableNode inducedFieldName, GreenExpressionNode iteratior, GreenBlockNode greenBlock)
            : base(NodeType.Forloop, interval )
        {
            InducedFieldType = inducedFieldType;
            InducedFieldName = inducedFieldName;
            Iteratior = iteratior;
            GreenBlock = greenBlock;
            ChildCount = 4;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return InducedFieldType;
                case 1: return InducedFieldName;
                case 2: return Iteratior;
                case 3: return GreenBlock;

                default:
                    return default(GreenCrawlSyntaxNode);
            }

            
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.ForLoopNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new GreenForLoopNode(Interval, (GreenTypeNode) newChild, InducedFieldName, Iteratior, GreenBlock);
                case 1: return new GreenForLoopNode(Interval, InducedFieldType, (GreenVariableNode) newChild, Iteratior, GreenBlock);
                case 2: return new GreenForLoopNode(Interval, InducedFieldType, InducedFieldName, (GreenExpressionNode) newChild, GreenBlock);
                case 3: return new GreenForLoopNode(Interval, InducedFieldType, InducedFieldName, Iteratior, (GreenBlockNode) newChild);

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}