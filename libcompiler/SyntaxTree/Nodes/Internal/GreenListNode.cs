using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenListNode<T> : GreenCrawlSyntaxNode where T : CrawlSyntaxNode
    {
        private readonly GreenCrawlSyntaxNode[] _children;

        public GreenListNode(Interval interval, IEnumerable<GreenCrawlSyntaxNode> children) : this(interval, children, NodeType.NodeList)
        { }

        protected GreenListNode(Interval interval, IEnumerable<GreenCrawlSyntaxNode> children, NodeType nodeType) : base(nodeType, interval)
        {
            _children = children.ToArray();
            ChildCount = _children.Length;
        }

        protected GreenListNode(Interval interval, GreenCrawlSyntaxNode[] children) : base(NodeType.NodeList, interval)
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

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            if (slot >= 0 || ChildCount > slot)
            {
                return _children[slot];
            }
            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.ListNode<T>(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            GreenCrawlSyntaxNode[] newArray = ChildCopy();

            newArray[index] = newChild;

            return new GreenListNode<T>(Interval, newArray);
        }
    }
}
