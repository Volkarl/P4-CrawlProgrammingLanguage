using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Antlr4.Runtime.Misc;
using libcompiler.Scope;

namespace libcompiler.SyntaxTree
{
    partial class CrawlSyntaxNode
    {
        public static BlockNode Block(Interval interval, params CrawlSyntaxNode[] statements)
        {
            return Block(interval, (IEnumerable<CrawlSyntaxNode>)statements);
        }

        public static BlockNode Block(Interval interval, IEnumerable<CrawlSyntaxNode> statements)
        {
            GreenBlockNode greenBlock = new GreenBlockNode(NodeType.Block, interval, statements.Select(s => s.Green), null);
            return (BlockNode) greenBlock.CreateRed(null, 0);
        }

        public class GreenBlockNode : GreenListNode<CrawlSyntaxNode>
        {
            internal BlockScope Scope { get; }

            internal GreenBlockNode(NodeType type, Interval interval, IEnumerable<GreenCrawlSyntaxNode> children, BlockScope scope) : base(
                type, interval, children)
            {
                Scope = scope;
            }

            internal override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
            {
                return new BlockNode(parent, this, indexInParent);
            }

            internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
            {
                if(0 > index || index >= ChildCount)
                    throw new ArgumentOutOfRangeException();

                var newchildren = ChildCopy();
                newchildren[index] = newChild;

                return new GreenBlockNode(Type, Interval, newchildren, Scope);

            }
        }
    }
}