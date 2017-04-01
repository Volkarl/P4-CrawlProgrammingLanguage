using System;
using System.Collections;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenMethodDeclerationNode : GreenDeclerationNode
    {
        //TODO: do something about parameters
        public GreenTypeNode ReturnType { get; }
        public GreenListNode<ParameterNode> Parameters { get; }
        public GreenListNode<GenericParameterNode> GenericParameters { get; }
        public GreenVariableNode Identfier { get; }
        public GreenBlockNode Body { get; }

        public GreenMethodDeclerationNode(
            Interval interval,
            ProtectionLevel protectionLevel,
            GreenTypeNode returnType,
            GreenListNode<ParameterNode> parameters,
            GreenListNode<GenericParameterNode> genericParameters,
            GreenVariableNode identfier,
            GreenBlockNode body
        )
            : base(interval, NodeType.MethodDecleration,  protectionLevel)
        {
            ReturnType = returnType;
            Parameters = parameters;
            GenericParameters = genericParameters;
            Identfier = identfier;
            Body = body;
            ChildCount = 5;
        }

        public override string ToString()
        {
            return $"decl {ReturnType.TypeName} {Identfier} =";
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0:
                    return ReturnType;
                case 1:
                    return Identfier;
                case 2:
                    return Parameters;
                case 3:
                    return GenericParameters;
                case 4:
                    return Body;
                default:
                    return default(GreenCrawlSyntaxNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.MethodDeclerationNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel, (GreenTypeNode) newChild, Parameters, GenericParameters, Identfier, Body);
                case 1:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel, ReturnType, Parameters, (GreenListNode<GenericParameterNode>) newChild, Identfier, Body);
                case 2:
                    throw new NotImplementedException();

                case 3:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel, ReturnType, Parameters, GenericParameters, (GreenVariableNode) newChild, Body);
                case 4:
                    return new GreenMethodDeclerationNode(Interval, ProtectionLevel, ReturnType, Parameters, GenericParameters, Identfier, (GreenBlockNode) newChild);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}