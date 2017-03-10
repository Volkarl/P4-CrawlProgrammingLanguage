using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class TokenNode : GreenNode
    {
        public string Value { get; }

        public TokenNode(Interval interval, string value) : base(NodeType.Token, interval)
        {
            Value = value;
        }

        public override GreenNode GetSlot(int slot)
        {
            return null;
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.TokenNode(parrent, this, slot);
        }
    }
}
