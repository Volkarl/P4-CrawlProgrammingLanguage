using System.Runtime.Remoting.Messaging;
using libcompiler.SyntaxTree;

namespace libcompiler.ExtensionMethods
{
    public static class CrawlSyntaxNodeExtensions
    {
        public static NamespaceNode FindNameSpace(this CrawlSyntaxNode node)
        {
            CrawlSyntaxNode result = node;

            while (!(result is NamespaceNode) && !(result is TranslationUnitNode))
            {
                result = result.Parent;
            }

            return result as NamespaceNode;
        }
    }
}