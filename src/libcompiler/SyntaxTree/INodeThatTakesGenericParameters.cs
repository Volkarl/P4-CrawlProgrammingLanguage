using libcompiler.SyntaxTree;

namespace libcompiler.SyntaxTree
{
    public interface INodeThatTakesGenericParameters
    {
        ListNode<GenericParameterNode> GenericParameters { get; }
    }
}