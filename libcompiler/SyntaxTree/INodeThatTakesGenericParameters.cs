using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.SyntaxTree
{
    public interface INodeThatTakesGenericParameters
    {
        ListNode<GenericParameterNode> GenericParameters { get; }
    }
}