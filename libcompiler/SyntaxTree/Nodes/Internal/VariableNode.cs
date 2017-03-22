using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class VariableNode : ExpressionNode
    {
        public string Name { get; }

        public VariableNode(Interval interval, string name)
            : base(interval, NodeType.Variable, SyntaxTree.ExpressionType.Variable)
        {
            Name = name;
            ChildCount = 0;
        }

        public override string ToString()
        {
            return Name;
        }

        public override GreenNode GetChildAt(int slot)
        {
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.VariableNode(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}