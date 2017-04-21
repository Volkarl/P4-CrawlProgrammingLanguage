using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
    public abstract class GreeenCallableDeclarationNode: GreenDeclerationNode
    {

        //TODO: do something about parameters
        /// <summary>
        /// This is the type of the entire thing. return type and parameter types
        /// </summary>
        public GreenTypeNode MethodSignature { get; }
        public GreenIdentifierNode Identfier { get; }
        public GreenBlockNode Body { get; }


        protected GreeenCallableDeclarationNode(
            Interval interval,
            ProtectionLevel protectionLevel,
            GreenTypeNode methodSignature,
            GreenIdentifierNode identfier,
            GreenBlockNode body,
            NodeType type
            
        )
            : base(interval, type, protectionLevel)
        {
            MethodSignature = methodSignature;
            Identfier = identfier;
            Body = body;
            ChildCount = 3;
        }


        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0:
                    return MethodSignature;
                case 1:
                    return Identfier;
                case 2:
                    return Body;
                default:
                    return default(GreenCrawlSyntaxNode);
            }
        }



    }
}
