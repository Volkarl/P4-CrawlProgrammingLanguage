using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenVariableNode : GreenExpressionNode
    {
        public string Name { get; }

        public GreenVariableNode(Interval interval, string name)
            : base(interval, NodeType.Variable, SyntaxTree.ExpressionType.Variable)
        {
            Name = name;
            ChildCount = 0;
        }

        public override string ToString()
        {
            return Name;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            return default(GreenCrawlSyntaxNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.VariableNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}