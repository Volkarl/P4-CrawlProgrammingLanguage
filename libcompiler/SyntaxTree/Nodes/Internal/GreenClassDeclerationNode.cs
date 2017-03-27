using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenClassDeclerationNode : GreenDeclerationNode
    {
        public GreenIdentifierNode Identifier { get; }
        public GreenListNode<GenericParameterNode> GenericParameters { get; }
        public GreenBlockNode Body { get; }


        public GreenClassDeclerationNode(
            Interval interval,
            ProtectionLevel protectionLevel,
            GreenIdentifierNode identifier,
            GreenListNode<GenericParameterNode> genericParameters,
            GreenBlockNode body
        )
            : base(interval, NodeType.ClassDecleration, protectionLevel)
        {
            ChildCount = 3;
            Identifier = identifier;
            GenericParameters = genericParameters;
            Body = body;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0:
                    return Identifier;
                case 1:
                    return GenericParameters;
                case 2:
                    return Body;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.ClassDeclerationNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0:
                    return new GreenClassDeclerationNode(this.Interval, ProtectionLevel, (GreenIdentifierNode)newChild, GenericParameters, Body);
                case 1:
                    return new GreenClassDeclerationNode(this.Interval, ProtectionLevel, Identifier, (GreenListNode<GenericParameterNode>) newChild, Body);
                case 2:
                    return new GreenClassDeclerationNode(this.Interval, ProtectionLevel, Identifier, GenericParameters, (GreenBlockNode)newChild);
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}