using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class ListNode<T> : GreenNode where T : CrawlSyntaxNode
    {
        private readonly GreenNode[] _children;

        public ListNode(Interval interval, IEnumerable<GreenNode> children) : base(NodeType.NodeList, interval)
        {
            _children = children.ToArray();
            ChildCount = _children.Length;
        }

        protected ListNode(Interval interval, GreenNode[] children) : base(NodeType.NodeList, interval)
        {
            _children = children;
            ChildCount = _children.Length;
        }

        protected GreenNode[] ChildCopy()
        {
            GreenNode[] newArray = new GreenNode[ChildCount];
            Array.Copy(_children, newArray, ChildCount);

            return newArray;
        }

        public override GreenNode GetSlot(int slot)
        {
            if (slot >= 0 || ChildCount > slot)
            {
                return _children[slot];
            }
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.ListNode<T>(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            GreenNode[] newArray = ChildCopy();

            newArray[index] = newChild;

            return new ListNode<T>(Interval, newArray);
        }
    }
}
