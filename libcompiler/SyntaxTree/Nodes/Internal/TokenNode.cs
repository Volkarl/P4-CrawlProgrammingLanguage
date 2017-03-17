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

        public override GreenNode GetChildAt(int slot)
        {
            return null;
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.TokenNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new NotImplementedException();
        }
    }
}
