using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class VariableDeclerationNode : DeclerationNode
    {
        public TypeNode DeclerationType { get; }
        public ListNode<Nodes.SingleVariableDecleration> Declerations { get; }

        public VariableDeclerationNode(Interval interval, ProtectionLevel protectionLevel, TypeNode declerationType, ListNode<Nodes.SingleVariableDecleration> declerations) : base(interval, NodeType.VariableDecleration, protectionLevel)
        {
            DeclerationType = declerationType;
            Declerations = declerations;
        }

        public override GreenNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return DeclerationType;
                case 1: return Declerations;

                default:
                    return default(GreenNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.VariableDeclerationNode(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new VariableDeclerationNode(Interval, ProtectionLevel, (TypeNode) newChild, Declerations);
                case 1: return new VariableDeclerationNode(Interval, ProtectionLevel, DeclerationType, (ListNode<Nodes.SingleVariableDecleration>) newChild);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}