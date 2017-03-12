using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ListNode<T> : CrawlSyntaxNode, IReadOnlyList<T> where T : CrawlSyntaxNode
    {
        public ListNode(CrawlSyntaxNode parrent, GreenNode self, int slot) : base(parrent, self, slot)
        {
            _childNodes = new T[self.ChildCount];
        }

        private readonly T[] _childNodes;
        public T this[int index]
        {
            get
            {
                if (index >= 0 || _childNodes.Length > index)
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
        public override CrawlSyntaxNode GetChild(int index)
        {
            return this[index];
        }
    }
}
