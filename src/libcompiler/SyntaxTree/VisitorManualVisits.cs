using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace libcompiler.SyntaxTree
{
    public partial class SyntaxVisitor
    {
        protected virtual void VisitList<T>(IEnumerable<T> list) where T : CrawlSyntaxNode
        {
            foreach (T item in list)
            {
                Visit(item);
            }
        }

        protected virtual void VisitBlock(BlockNode block)
        {
            foreach (CrawlSyntaxNode child in block)
            {
                Visit(child);
            }
        }
    }

    public partial class SyntaxVisitor<T>
    {
        protected abstract T VisitList<TNode>(IEnumerable<TNode> list) where TNode : CrawlSyntaxNode;

        protected abstract T VisitBlock(BlockNode block);
    }

    public partial class SimpleSyntaxVisitor<T>
    {
        protected override T VisitList<TNode>(IEnumerable<TNode> list)
        {
            return Combine(list.Select(Visit).ToArray());
        }

        protected override T VisitBlock(BlockNode block)
        {
            return Combine(block.Select(Visit).ToArray());
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