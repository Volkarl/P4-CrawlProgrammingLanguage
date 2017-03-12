using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class TypeNode : GreenNode
    {
        public CrawlType ExportedType { get; }

        public TypeNode(Interval interval, CrawlType expotedType) : base(NodeType.Type, interval)
        {
            ExportedType = expotedType;
        }

        public override GreenNode GetSlot(int slot)
        {
            return default(GreenNode);
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parrent, int slot)
        {
            return new Nodes.TypeNode(parrent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            throw new NotImplementedException();
        }
    }
}
