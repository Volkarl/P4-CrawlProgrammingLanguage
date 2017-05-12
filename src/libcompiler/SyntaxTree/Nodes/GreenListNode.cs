using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree
{
    partial class CrawlSyntaxNode
    {
        public class GreenListNode<T> : GreenCrawlSyntaxNode where T : CrawlSyntaxNode
        {
            private readonly GreenCrawlSyntaxNode[] _children;

            public GreenListNode(NodeType type, Interval interval, IEnumerable<GreenCrawlSyntaxNode> children) : base(
                type, interval)
            {
                _children = children.ToArray();
                ChildCount = _children.Length;
            }

            protected GreenListNode(Interval interval, GreenCrawlSyntaxNode[] children) : base(NodeType.List,
                interval)
            {
                _children = children;
                ChildCount = _children.Length;
            }

            protected GreenCrawlSyntaxNode[] ChildCopy()
            {
                GreenCrawlSyntaxNode[] newArray = new GreenCrawlSyntaxNode[ChildCount];
                Array.Copy(_children, newArray, ChildCount);

                return newArray;
            }

            internal override GreenCrawlSyntaxNode GetChildAt(int slot)
            {
                if (slot >= 0 || ChildCount > slot)
                {
                    return _children[slot];
                }
                return default(GreenCrawlSyntaxNode);
            }

            internal override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
            {
                return new ListNode<T>(parent, this, indexInParent);
            }

            internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
            {
                GreenCrawlSyntaxNode[] newArray = ChildCopy();

                newArray[index] = newChild;

                return new GreenListNode<T>(Interval, newArray);
            }
        }
    }
}