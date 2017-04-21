using System;
using System.Collections;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenMethodDeclerationNode : GreeenCallableDeclarationNode
    {
        public GreenListNode<IdentifierNode> ParameterIdentifiers { get; }
        public GreenListNode<GenericParameterNode> GenericParameters { get; }

  
        public GreenMethodDeclerationNode(
            Interval interval,
            ProtectionLevel protectionLevel,
            GreenTypeNode methodSignature,
            GreenListNode<IdentifierNode> parameterIdentifiers,
            GreenListNode<GenericParameterNode> genericParameters,
            GreenIdentifierNode identfier,
            GreenBlockNode body
        )
            : base(interval, protectionLevel, methodSignature ,identfier, body, NodeType.MethodDecleration )
        {
            ParameterIdentifiers = parameterIdentifiers;
            GenericParameters = genericParameters;
            ChildCount = 5;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0:
                    return MethodSignature;
                case 1:
                    return Identfier;
                case 2:
                    return Body;
                case 3:
                    return ParameterIdentifiers;
                case 4:
                    return GenericParameters;
                
                default:
                    return default(GreenCrawlSyntaxNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new MethodDeclerationNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel,
                        (GreenTypeNode) newChild, ParameterIdentifiers, GenericParameters, Identfier, Body);
                case 1:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel,
                        MethodSignature, (GreenListNode<IdentifierNode>) newChild, GenericParameters, Identfier, Body);
                case 2:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel,
                        MethodSignature, ParameterIdentifiers, (GreenListNode<GenericParameterNode>) newChild,
                        Identfier, Body);
                case 3:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel,
                        MethodSignature, ParameterIdentifiers, GenericParameters, (GreenIdentifierNode) newChild, Body);
                case 4:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel,
                        MethodSignature, ParameterIdentifiers, GenericParameters, Identfier, (GreenBlockNode) newChild);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}