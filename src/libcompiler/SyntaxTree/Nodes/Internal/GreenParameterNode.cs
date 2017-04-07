using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    class GreenParameterNode : GreenCrawlSyntaxNode
    {
        public bool Reference { get; }
        public GreenTypeNode ParameterType { get; }
        public string Identifier { get; }


        public GreenParameterNode(Interval interval, bool reference, GreenTypeNode parameterType, string identifier) : base(NodeType.Parameter, interval)
        {
            Reference = reference;
            ParameterType = parameterType;
            Identifier = identifier;
            ChildCount = 1;
        }

        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            if (slot == 0)
                return ParameterType;

            return null;
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new ParameterNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            if(index == 0)
                return new GreenParameterNode(Interval, Reference, (GreenTypeNode) newChild, Identifier);

            throw new ArgumentOutOfRangeException();
        }
    }
}
