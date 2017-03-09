using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes
{
    public class FunctionDeclerationNode : DeclerationNode
    {
        //TODO: do something about parameters
        public CrawlType FunctionType { get; }
        public string Identfier { get; }
        public BlockNode BodyBlock { get; }

        public FunctionDeclerationNode(CrawlSyntaxTree owningTree, CrawlType functionType, string name,
            Interval interval, BlockNode block, ProtectionLevel protectionLevel)
            : base(owningTree, interval, NodeType.FunctionDecleration, protectionLevel)
        {
            FunctionType = functionType;
            Identfier = name;
            BodyBlock = block;
        }
    }
}