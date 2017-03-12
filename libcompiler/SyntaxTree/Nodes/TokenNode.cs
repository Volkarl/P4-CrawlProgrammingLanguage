using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class TokenNode : CrawlSyntaxNode
    {
        public string Value { get; }
        public TokenNode(CrawlSyntaxNode parrent, Internal.TokenNode self, int slot) : base(parrent, self, slot)
        {
            Value = self.Value;
        }

        public override CrawlSyntaxNode GetChild(int index)
        {
            throw new NotImplementedException();
        }
    }
}
