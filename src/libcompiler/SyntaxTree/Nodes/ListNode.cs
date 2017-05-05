using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree
{
    public class ListNode<T> : CrawlSyntaxNode, IReadOnlyList<T> where T : CrawlSyntaxNode
    {
        public ListNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent) : base(parent, self, indexInParent)
        {
            _childNodes = new T[self.ChildCount];
        }

        private readonly T[] _childNodes;
        public T this[int index]
        {
            get
            {
                if (index >= 0 && _childNodes.Length > index)
                {
                    var tmp = GetRed(ref _childNodes[index], index);
                    return tmp;
                }

                return default(T);
            }
        }

        public ListNode<T> Update(Interval interval, IEnumerable<T> children)
        {
            List<T> newchildren = children.ToList();

            if (Interval.Equals(interval) && AreEqual(newchildren)) return this;

            var green = new GreenListNode<T>(NodeType.List, interval, newchildren.Select(ExtractGreenNode));

            return (ListNode<T>) Translplant(green.CreateRed(null, 0));
        }

        protected bool AreEqual(List<T> n)
        {
            if (_childNodes.Length != n.Count) return false;

            for (int i = 0; i < _childNodes.Length; i++)
            {
                if (_childNodes[i] != n[i]) return false;
            }

            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _childNodes.Length; i++)
            {
                yield return this[i];
            }
        }

        int IReadOnlyCollection<T>.Count => _childNodes.Length;
        public override CrawlSyntaxNode GetChildAt(int index)
        {
            return this[index];
        }

        public override string ToString()
        {
            return $"List of {typeof(T).Name}s";
        }
    }
}
