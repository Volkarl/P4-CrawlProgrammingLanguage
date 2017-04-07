using System;
using System.Linq.Expressions;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenGenericUnpackNode : GreenExpressionNode
    {
        /// <summary> The generic-dependent type we are unpacking </summary>
        public GreenExpressionNode Target { get; }
        /// <summary> Arguments to the unpacking of the generic type </summary>
        public GreenListNode<TypeNode> Generics { get; }

        public GreenGenericUnpackNode(Interval interval, GreenExpressionNode target, GreenListNode<TypeNode> generics) : base(interval, NodeType.GenericUnpack, SyntaxTree.ExpressionType.GenericUnpack)
        {
            Target = target;
            Generics = generics;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0:
                    return Target;
                case 1:
                    return Generics;
                default:
                    return default(GreenCrawlSyntaxNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new GenericsUnpackNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                case 0:
                    return new GreenGenericUnpackNode(Interval, (GreenExpressionNode) newChild, Generics);
                case 1:
                    return new GreenGenericUnpackNode(Interval, Target, (GreenListNode<TypeNode>) newChild);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}