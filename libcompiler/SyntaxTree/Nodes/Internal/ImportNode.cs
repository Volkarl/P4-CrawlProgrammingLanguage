using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class ImportNode : GreenNode
    {
        public string Module { get; }

        public ImportNode(Interval interval, string module)
            : base(NodeType.Import, interval)
        {
            Module = module;
            ChildCount = 0;
        }

        public override GreenNode GetChildAt(int slot)
        {
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            throw new System.NotImplementedException();
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}