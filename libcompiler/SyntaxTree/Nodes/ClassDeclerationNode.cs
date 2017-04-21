﻿using libcompiler.SyntaxTree.Nodes.Internal;
using libcompiler.TypeChecker;

namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// This declares a class.
    /// </summary>
    //TODO: IScope
    public class ClassDeclerationNode : DeclerationNode, INodeThatTakesGenericParameters, IScope
    {
        private IdentifierNode _identifier;
        private ListNode<GenericParameterNode> _genericParameters;
        private BlockNode _body;

        //TODO: List of constructors? Probably as extension method to not calculate unless required

        /// <summary>
        /// The name of this class.
        /// </summary>
        public IdentifierNode Identifier => GetRed(ref _identifier, 0);

        public ListNode<GenericParameterNode> GenericParameters => GetRed(ref _genericParameters, 1);

        /// <summary>
        /// All the contents inside the class (functions, variables and stuff)
        /// </summary>
        public BlockNode BodyBlock => GetRed(ref _body, 2);
        

        public ClassDeclerationNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
        }

        public override CrawlSyntaxNode GetChildAt(int index)
        {
            switch (index)
            {
                case 0: return Identifier;
                case 1: return BodyBlock;
                default: return default(CrawlSyntaxNode);
            }
        }
    }
}