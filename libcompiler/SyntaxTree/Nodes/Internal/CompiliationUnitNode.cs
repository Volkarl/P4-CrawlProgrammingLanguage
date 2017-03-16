using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class CompiliationUnitNode : GreenNode
    {
        public ListNode<Nodes.ImportNode> Imports { get; }
        public BlockNode Body { get; }

        public CompiliationUnitNode(Interval interval, ListNode<Nodes.ImportNode> imports,
            BlockNode block) : base(NodeType.CompilationUnit, interval)
        {
            Imports = imports;
            Body = block;
        }

        public override GreenNode GetSlot(int slot)
        {
            switch (slot)
            {
                case 0: return Imports;
                case 1: return Body;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.CompiliationUnitNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            if(index == 0)
                return new CompiliationUnitNode(this.Interval, (ListNode<Nodes.ImportNode>)newChild, Body);
            else if(index == 1)
                return new CompiliationUnitNode(this.Interval, Imports, (BlockNode)newChild);

            throw new IndexOutOfRangeException();
        }
    }
}