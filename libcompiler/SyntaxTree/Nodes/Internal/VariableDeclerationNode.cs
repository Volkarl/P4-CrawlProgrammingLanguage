using System.Collections.Generic;
using System.Linq;
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

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return DeclerationType;
                case 1: return Declerations;

                default:
                    return default(GreenNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.VariableDeclerationNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new System.NotImplementedException();
        }
    }
}