using libcompiler.SyntaxTree.Nodes;
using libcompiler.TypeChecker;

namespace libcompiler.ExtensionMethods
{
    public static class CrawlSyntaxNodeExtensions
    {
        /// <summary>
        /// Finds the first scope among this node and its ancestors
        /// </summary>
        /// <param name="node">A <see cref="CrawlSyntaxNode"/> to examine for scope information</param>
        /// <returns></returns>
        public static IScope FindFirstScope(this CrawlSyntaxNode node)
        {
            // Check if the parent inherits from IScope, if does, return the node as an IScope.
            if (node == null) return null;

            IScope scope = node as IScope;
            if (scope != null) return scope;

            // If the node doesn't inherit from IScope, we check the parent's parent.
            return FindFirstScope(node.Parent);

        }
    }
}
