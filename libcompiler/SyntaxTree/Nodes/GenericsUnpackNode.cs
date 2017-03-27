using System;
using System.Collections.Generic;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class GenericsUnpackNode : ExpressionNode
    {
        /// <summary> The generic-dependent type we are unpacking </summary>
        public ExpressionNode Target => GetRed(ref _target, 0);
        private ExpressionNode _target;

        /// <summary> Arguments to the unpacking of the generic type </summary>
        public ListNode<TypeNode> Generics => GetRed(ref _generics, 1);
        private ListNode<TypeNode> _generics;


        public GenericsUnpackNode(CrawlSyntaxNode parent, GreenExpressionNode self, int indexInParent) : base(parent, self, indexInParent)
        {
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0:
                    return Target;
                case 1:
                    return Generics;
                default:
                    return default(CrawlSyntaxNode);
            }
        }
    }
}