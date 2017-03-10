using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class DeclerationNode : GreenNode
    {
        public ProtectionLevel ProtectionLevel { get; }

        protected DeclerationNode(Interval interval, NodeType type, ProtectionLevel protectionLevel) : base(type, interval)
        {
            ProtectionLevel = protectionLevel;
        }

    }
}