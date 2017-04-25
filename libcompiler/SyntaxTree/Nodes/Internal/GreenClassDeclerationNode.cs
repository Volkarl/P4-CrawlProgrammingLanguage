using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenClassDeclerationNode : GreenDeclerationNode
    {
        public GreenIdentifierNode Identifier { get; }
        public GreenIdentifierNode Ancestor { get; }
        public GreenListNode<GenericParameterNode> GenericParameters { get; }
        public GreenBlockNode Body { get; }


        public GreenClassDeclerationNode(
            Interval interval,
            ProtectionLevel protectionLevel,
            GreenIdentifierNode identifier,
            GreenIdentifierNode ancestor,
            GreenListNode<GenericParameterNode> genericParameters,
            GreenBlockNode body
        )
            : base(interval, NodeType.ClassDecleration, protectionLevel)
        {
            ChildCount = 4;
            Identifier = identifier;
            Ancestor = ancestor;
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
                    return Ancestor;
                case 2:
                    return GenericParameters;
                case 3:
                    return Body;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new ClassDeclerationNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0:
                    return new GreenClassDeclerationNode(Interval, ProtectionLevel, (GreenIdentifierNode)newChild, Ancestor,  GenericParameters, Body);
                case 1:
                    return new GreenClassDeclerationNode(Interval, ProtectionLevel, Identifier, (GreenIdentifierNode)newChild, GenericParameters, Body);
                case 2:
                    return new GreenClassDeclerationNode(Interval, ProtectionLevel, Identifier, Ancestor, (GreenListNode<GenericParameterNode>)newChild, Body);
                case 3:
                    return new GreenClassDeclerationNode(Interval, ProtectionLevel, Identifier, Ancestor, GenericParameters, (GreenBlockNode)newChild);
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}