using System.CodeDom.Compiler;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public class GreenGenericParameterNode : GreenIdentifierNode
    {
        /// <summary>
        /// The class that must be implemented by values for this generic parameter.
        /// </summary>
        public string Limitation { get; }

        public GreenGenericParameterNode(Interval interval, string value, string limitation)
            : base(interval, value, NodeType.GenericParametersNode)
        {
            Limitation = limitation;
        }

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new GenericParameterNode(parent, this, indexInParent);
        }
    }
}