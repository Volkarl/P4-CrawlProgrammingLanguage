using System.Runtime.Remoting.Messaging;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.ExtensionMethods
{
    public static class CrawlSyntaxNodeExtensions
    {
        public static NameSpaceNode FindNameSpace(this CrawlSyntaxNode node)
        {
            CrawlSyntaxNode result = node;

            while (!(result is NameSpaceNode) && !(result is TranslationUnitNode))
            {
                result = result.Parent;
            }

            return result as NameSpaceNode;
        }
    }
}