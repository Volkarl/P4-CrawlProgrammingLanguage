using System.Linq;

namespace libcompiler.SyntaxTree
{
    public partial class SyntaxVisitor
    {
        protected virtual void VisitList<T>(ListNode<T> list) where T : CrawlSyntaxNode
        {
            foreach (T item in list)
            {
                Visit(item);
            }
        }
    }

    public partial class SyntaxVisitor<T>
    {
        protected abstract T VisitList<TNode>(ListNode<TNode> list) where TNode : CrawlSyntaxNode;
    }

    public partial class SimpleSyntaxVisitor<T>
    {
        protected override T VisitList<TNode>(ListNode<TNode> list)
        {
            return Combine(list.Select(Visit).ToArray());
        }
    }

    /*public partial class SyntaxRewriter<T>
    {
        protected override CrawlSyntaxNode VisitList<TNode>(ListNode<TNode> list)
        {
            return list.Update(list.Select(Visit).ToArray());
        }
    }
    SimpleSyntaxVisitor
    SyntaxRewriter

*/
}