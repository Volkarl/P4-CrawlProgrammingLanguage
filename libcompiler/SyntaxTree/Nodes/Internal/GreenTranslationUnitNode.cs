using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class TranslationUnitNode : GreenNode
    {
        public ListNode<Nodes.ImportNode> Imports { get; }
        public BlockNode Body { get; }

        public TranslationUnitNode(Interval interval, ListNode<Nodes.ImportNode> imports,
            BlockNode block) : base(NodeType.TranslationUnit, interval)
        {
            Imports = imports;
            Body = block;
        }

        public override GreenNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return Imports;
                case 1: return Body;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.TranslationUnitNode(parent, this, indexInParent);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if(index == 0)
                return new TranslationUnitNode(this.Interval, (ListNode<Nodes.ImportNode>)newChild, Body);
            else if(index == 1)
                return new TranslationUnitNode(this.Interval, Imports, (BlockNode)newChild);

            throw new ArgumentOutOfRangeException();
        }
    }
}