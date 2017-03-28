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
        //Checks if the symbol is in this block node scope or the nodes predecessor
        public TypeInformation[] GetScope(string symbol)
        {
            TypeInformation[] typeInformations = scopeInfo.GetScope(symbol);
            
            if (typeInformations == null)
            {
                IScope scope;
                scope = FindScopeParent(this);
                return scope?.GetScope(symbol);
            }
            return scopeInfo.GetScope(symbol);
        }
        // checks if the parent inherites from IScope, if it inherities it return the node as an IScope,
        // if the node do not inherites from IScope we check the parents parent.
        public IScope FindScopeParent(CrawlSyntaxNode node)
        {
            if (node.Parent is IScope)
            {
                return node.Parent as IScope;
            }
            else if (node.Parent == null)
            {
                return null;
            }
            else
            {
                return FindScopeParent(node.Parent);
            }
        }
    }
}