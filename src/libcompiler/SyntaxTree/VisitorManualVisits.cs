using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Antlr4.Runtime.Tree.Xpath;

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

    public partial class SyntaxRewriter
    {
        protected override CrawlSyntaxNode VisitBlock(BlockNode block)
        {
            var newchildren = block.Select(Visit);

            return block.Update(block.Interval, newchildren);
        }

        protected override CrawlSyntaxNode VisitList<TNode>(IEnumerable<TNode> list)
        {
            /*
             * This is a rather nasty, but necessary hack to create updated ListNode<T>
             * The dilemma is that due the type rules for C#, it cannot just be cast to ListNode<CrawlSyntaxNode>
             *
             * As a workaround, reflection is used to examine the list and find the actual type of T
             * then using reflection to create a generic method matching said T and calling it
             */

            //Get the actual type contained in the list
            Type genericType = list.GetType().GenericTypeArguments[0];

            //Find the method (implemented below) that takes an arbitary list
            var visitRealListGeneral =
                typeof(SyntaxRewriter)
                    .GetMethod(nameof(InnerVisitList), BindingFlags.Instance | BindingFlags.NonPublic);

            //Create a specific version for T
            var visitRealListSpecefic = visitRealListGeneral.MakeGenericMethod(genericType);

            //Finally call it
            try
            {
                return (CrawlSyntaxNode) visitRealListSpecefic
                    .Invoke(this, new object[]
                    {
                        list
                    });
            }
            //If anything in the call throws an exception, .Invoke will catch it and throw a TargetInvocationException
            //This is rather useless as it tells us nothing. So we rethrow the inner exception that contains actual usefull data
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

                //Above line should never return, but C# compiler is not aware of this
                throw;
            }

        }

        protected virtual CrawlSyntaxNode InnerVisitList<TNode>(ListNode<TNode> list) where TNode : CrawlSyntaxNode
        {
            var newchildren = list.Select(Visit).Cast<TNode>();

            return list.Update(list.Interval, newchildren);
        }
    }


}