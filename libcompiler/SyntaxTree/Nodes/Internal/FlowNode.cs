using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class FlowNode : GreenNode
    {
        protected FlowNode(NodeType type, Interval interval)
            : base(type, interval)
        {
        }
    }
}