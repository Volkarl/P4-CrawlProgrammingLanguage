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

        public override GreenNode GetSlot(int slot)
        {
            if (slot >= 0 || ChildCount > slot)
            {
                return _children[slot];
            }
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.ListNode<T>(parrent, this, slot);
        }
    }
}
