using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.TypeChecker;

namespace libcompiler.SyntaxTree
{
    public static class CrawlSyntaxNodeExtensions
    {
        // checks if the parent inherites from IScope, if it inherities it return the node as an IScope,
        // if the node do not inherites from IScope we check the parents parent.
        /// <summary>
        /// Finds the first scope among this node and its ancestors
        /// </summary>
        /// <param name="node">A <see cref="CrawlSyntaxNode"/> to examine for scope information</param>
        /// <returns></returns>
        public static IScope FindFirstScope(this CrawlSyntaxNode node)
        {
            if (node == null) return null;

            IScope scope = node as IScope;
            if (scope != null) return scope;

            return FindFirstScope(node.Parent);

        }
    }
}
