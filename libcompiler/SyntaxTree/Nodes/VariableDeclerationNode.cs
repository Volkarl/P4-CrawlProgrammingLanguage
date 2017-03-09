using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class VariableDeclerationNode : DeclerationNode
    {
        public CrawlType DeclerationType { get; }
        public List<SingleVariableDecleration> Declerations { get; }

        public VariableDeclerationNode(
            CrawlSyntaxTree owningTree,
            ProtectionLevel protectionLevel,
            CrawlType declerationType,
            IEnumerable<SingleVariableDecleration> declerations,
            Interval interval
        ) : base(owningTree, interval, NodeType.VariableDecleration, protectionLevel)
        {
            DeclerationType = declerationType;
            Declerations = declerations.ToList();
        }
    }
}