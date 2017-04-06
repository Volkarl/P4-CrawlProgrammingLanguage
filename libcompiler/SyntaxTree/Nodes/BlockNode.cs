using libcompiler.SyntaxTree.Nodes.Internal;
using libcompiler.TypeChecker;

namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// Represents a block of code, surrounded by Indent .. Dedent
    /// </summary>
    public class BlockNode : ListNode<CrawlSyntaxNode>, IScope
    {
        private BlockScope scopeInfo;
        //TODO: This would be a good place to save scope information.
        
        public BlockNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent)
            : base(parent, self, indexInParent)
        {
            scopeInfo = new BlockScope(this);
        }
        //Checks if the identifier is in this block node scope or the nodes predecessor
        public TypeInformation[] GetScope(string identifier)
        {
            TypeInformation[] typeInformation = scopeInfo.GetScope(identifier);
            
            if (typeInformation == null)
            {
                IScope scope;
                scope = Parent.FindFirstScope();
                return scope?.GetScope(identifier);
            }
            return typeInformation;
        }
    }
}