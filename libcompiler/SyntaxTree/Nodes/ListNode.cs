using System.Collections;
using System.Collections.Generic;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
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
                    return GetRed(ref _childNodes[index], index);
                }

                return default(T);
            }
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
            return $"{base.ToString()} <{typeof(T).Name}>";
        }
    }
}
