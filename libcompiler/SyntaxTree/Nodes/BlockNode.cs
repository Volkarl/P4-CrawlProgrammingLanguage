using libcompiler.SyntaxTree.Nodes.Internal;
using libcompiler.TypeChecker;

namespace libcompiler.SyntaxTree.Nodes
{
    /// <summary>
    /// Represents a block of code, surrounded by Indent .. Dedent
    /// </summary>
    public class BlockNode : ListNode<CrawlSyntaxNode>, IScope
    {
        private readonly BlockScope _scopeInfo;
        //TODO: This would be a good place to save scope information.
        
        public BlockNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent)
            : base(parent, self, indexInParent)
        {
            _scopeInfo = new BlockScope(this);
        }
        //Checks if the symbol is in this block node scope or the nodes predecessor
        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation[] typeInformation = _scopeInfo.FindSymbol(symbol);
            
            if (typeInformation == null)
            {
                IScope scope = Parent.FindFirstScope();
                return scope?.FindSymbol(symbol);
            }
            return typeInformation;
        }
    }
}