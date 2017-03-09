using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public abstract class CrawlSyntaxNode
    {
        /// <summary>
        /// The <see cref="CrawlSyntaxTree"/> this Node belongs to. 
        /// </summary>
        public CrawlSyntaxTree OwningTree { get; }

        /// <summary>
        /// The <see cref="NodeType"/> of this <see cref="CrawlSyntaxNode"/>. 
        /// It is unique to most <see cref="CrawlSyntaxNode"/> with the 
        /// exception being <see cref="ExpressionNode"/> which also contains
        /// an <see cref="ExpressionType"/>
        /// </summary>
        public NodeType Type { get; }

        /// <summary>
        /// The Interval this <see cref="CrawlSyntaxNode"/> covers in the source code
        /// <b>NOTICE: This API element is not stable and might change</b>
        /// </summary>
        public Interval CodeInterval { get; }

        protected CrawlSyntaxNode(CrawlSyntaxTree owningTree, NodeType type, Interval codeInterval)
        {
            OwningTree = owningTree;
            Type = type;
            CodeInterval = codeInterval;
        }
    }
}