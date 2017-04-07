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
        public GreenTypeNode ReturnType { get; }
        public GreenVariableNode Identfier { get; }
        public GreenBlockNode Body { get; }


        public GreeenCallableDeclarationNode(
            Interval interval,
            ProtectionLevel protectionLevel,
            GreenTypeNode returnType,
            GreenVariableNode identfier,
            GreenBlockNode body,
            NodeType type
            
        )
            : base(interval, type, protectionLevel)
        {
            ReturnType = returnType;
            Identfier = identfier;
            Body = body;
            ChildCount = 3;
        }


        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                case 0:
                    return ReturnType;
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
