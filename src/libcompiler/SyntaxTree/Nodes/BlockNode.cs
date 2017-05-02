using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Antlr4.Runtime.Misc;
using libcompiler.Scope;

namespace libcompiler.SyntaxTree
{
    /// <summary>
    /// Represents a block of code, surrounded by Indent .. Dedent
    /// </summary>
    public class BlockNode : ListNode<CrawlSyntaxNode>, IScope
    {
        public BlockScope Scope { get; }

        public BlockNode(CrawlSyntaxNode parent, GreenCrawlSyntaxNode self, int indexInParent)
            : base(parent, self, indexInParent)
        {
            Scope = ((GreenBlockNode) self).Scope;

        }
        //Checks if the symbol is in this block node scope or the nodes predecessor
        public TypeInformation[] FindSymbol(string symbol)
        {
            TypeInformation[] typeInformation = Scope?.FindSymbol(symbol);

            if (typeInformation == null)
            {
                IScope scope = Parent.FindFirstScope();
                return  scope?.FindSymbol(symbol);
            }

            return typeInformation;
        }

        public new BlockNode Update(Interval interval, IEnumerable<CrawlSyntaxNode> children, BlockScope scope = null)
        {
            List<CrawlSyntaxNode> newchildren = children.ToList();

            if (Interval.Equals(interval) && AreEqual(newchildren) && scope == Scope) return this;

            var green = new GreenBlockNode(NodeType.Block, interval, newchildren.Select(ExtractGreenNode), scope);

            return (BlockNode) Translplant(green.CreateRed(null, 0));

        }

        public CrawlSyntaxNode WithScope(BlockScope scope)
        {
            return Update(Interval, this, scope);
        }

        public override string ToString()
        {
            if (Scope == null) return $"Block {Interval}";

            return $"Block {Interval} -> {{{string.Join(", ", Scope.LocalSymbols())}}}";
        }

        public IEnumerable<string> LocalSymbols() => Scope.LocalSymbols();
    }
}