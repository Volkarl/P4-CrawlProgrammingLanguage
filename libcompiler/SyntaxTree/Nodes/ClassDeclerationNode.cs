using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class ClassDeclerationNode : DeclerationNode
    {
        //TODO: List of constructors? Probably as extension method to not calculate unless required
        public BlockNode BodyBlock { get; }
        public string Identifier { get; }

        public ClassDeclerationNode(CrawlSyntaxTree owningTree, ProtectionLevel protectionLevel, string name,
            BlockNode bodyBlock, Interval interval)
            : base(owningTree, interval, NodeType.ClassDecleration, protectionLevel)
        {
            Identifier = name;
            BodyBlock = bodyBlock;
        }
    }
}