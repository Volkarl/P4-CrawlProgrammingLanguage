using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenVariableDeclerationNode : GreenDeclerationNode
    {
        public GreenTypeNode DeclerationType { get; }
        public GreenListNode<Nodes.SingleVariableDecleration> Declerations { get; }

        public GreenVariableDeclerationNode(Interval interval, ProtectionLevel protectionLevel, GreenTypeNode declerationType, GreenListNode<Nodes.SingleVariableDecleration> declerations) : base(interval, NodeType.VariableDecleration, protectionLevel)
        {
            DeclerationType = declerationType;
            Declerations = declerations;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return DeclerationType;
                case 1: return Declerations;

                default:
                    return default(GreenCrawlSyntaxNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.VariableDeclerationNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new GreenVariableDeclerationNode(Interval, ProtectionLevel, (GreenTypeNode) newChild, Declerations);
                case 1: return new GreenVariableDeclerationNode(Interval, ProtectionLevel, DeclerationType, (GreenListNode<Nodes.SingleVariableDecleration>) newChild);
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}