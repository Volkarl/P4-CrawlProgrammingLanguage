using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree.Nodes
{
    public class VariableDeclerationNode : DeclerationNode
    {
        private TypeNode _declType;
        private ListNode<SingleVariableDecleration> _decls;
        public TypeNode DeclerationType => GetRed(ref _declType, 0);
        public ListNode<SingleVariableDecleration> Declerations => GetRed(ref _decls, 1);

        public VariableDeclerationNode(CrawlSyntaxNode parrent, GreenNode self, int slot) : base(parrent, self, slot)
        {
            
        }

        public override CrawlSyntaxNode GetChild(int index)
        {
            throw new System.NotImplementedException();
        }
    }
}