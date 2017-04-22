using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree
{
    public partial class CrawlSyntaxNode
    {
        public static BlockNode Block(Interval interval, params CrawlSyntaxNode[] statements)
        {
            return Block(interval, (IEnumerable<CrawlSyntaxNode>)statements);
        }

        public static BlockNode Block(Interval interval, IEnumerable<CrawlSyntaxNode> statements)
        {
            GreenBlockNode greenBlock = new GreenBlockNode(NodeType.Block, interval, statements.Select(s => s.Green));
            return (BlockNode) greenBlock.CreateRed(null, 0);

        }

        public class GreenBlockNode : GreenListNode<CrawlSyntaxNode>
        {
            internal GreenBlockNode(NodeType type, Interval interval, IEnumerable<GreenCrawlSyntaxNode> children) : base(
                type, interval, children)
            {

            }

            internal override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
            {
                return new BlockNode(parent, this, indexInParent);
            }

            internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
            {
                switch (index)
                {
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}