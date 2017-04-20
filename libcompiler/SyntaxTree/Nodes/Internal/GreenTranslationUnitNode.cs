using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenTranslationUnitNode : GreenCrawlSyntaxNode
    {
        public GreenListNode<Nodes.ImportNode> Imports { get; }
        public GreenNameSpaceNode NameSpace { get; }
        public GreenBlockNode Body { get; }

        public GreenTranslationUnitNode(Interval interval, GreenListNode<Nodes.ImportNode> imports, GreenNameSpaceNode nameSpace,
            GreenBlockNode greenBlock) : base(NodeType.TranslationUnit, interval)
        {
            Imports = imports;
            NameSpace = nameSpace;
            Body = greenBlock;
            ChildCount = 3;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return Imports;
                case 1: return NameSpace;
                case 2: return Body;
                default:
                    return null;
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new Nodes.TranslationUnitNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            if(index == 0)
                return new GreenTranslationUnitNode(this.Interval, (GreenListNode<Nodes.ImportNode>)newChild, NameSpace, Body);
            else if(index == 1)
                return new GreenTranslationUnitNode(this.Interval, Imports, (GreenNameSpaceNode)newChild, Body);
            else if (index == 2)
                return new GreenTranslationUnitNode(this.Interval, Imports, NameSpace, (GreenBlockNode)newChild);

            throw new ArgumentOutOfRangeException();
        }
    }
}