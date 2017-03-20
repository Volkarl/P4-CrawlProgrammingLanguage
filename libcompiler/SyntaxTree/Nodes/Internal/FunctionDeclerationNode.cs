using System;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class FunctionDeclerationNode : DeclerationNode
    {
        //TODO: do something about parameters
        public TypeNode FunctionType { get; }
        public VariableNode Identfier { get; }
        public BlockNode BodyBlock { get; }

        public FunctionDeclerationNode(Interval interval, ProtectionLevel protectionLevel, TypeNode functionType, VariableNode identfier, BlockNode bodyBlock)
            : base(interval, NodeType.FunctionDecleration, protectionLevel)
        {
            FunctionType = functionType;
            Identfier = identfier;
            BodyBlock = bodyBlock;
            ChildCount = 3;
        }

        public override string ToString()
        {
            return $"decl {FunctionType.ExportedType.Textdef} {Identfier} =";
        }

        public override GreenNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0: return FunctionType;
                case 1: return Identfier;
                case 2: return BodyBlock;
                default:
                    return default(GreenNode);
            }
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int slot)
        {
            return new Nodes.FunctionDeclerationNode(parent, this, slot);
        }

        internal override GreenNode WithReplacedChild(GreenNode newChild, int index)
        {
            switch (index)
            {
                case 0: return new FunctionDeclerationNode(Interval, ProtectionLevel, (TypeNode) newChild, Identfier, BodyBlock);
                case 1:return new FunctionDeclerationNode(Interval, ProtectionLevel, FunctionType, (VariableNode) newChild, BodyBlock);
                case 2:return new FunctionDeclerationNode(Interval, ProtectionLevel, FunctionType, Identfier, (BlockNode) newChild);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}