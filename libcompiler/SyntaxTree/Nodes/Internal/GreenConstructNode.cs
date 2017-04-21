using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;

namespace libcompiler.SyntaxTree.Nodes.Internal
{
   internal class GreenConstructNode : GreeenCallableDeclarationNode
    {
        public new CrawlType ReturnType
        { get { throw new NotImplementedException(); } }

        public GreenConstructNode(Interval interval, ProtectionLevel protectionLevel, GreenTypeNode methodSignature, GreenBlockNode body) 
            : base(interval, protectionLevel, methodSignature,null, body,NodeType.ConstructorNode)
        {
            ChildCount = 3;
            
        }  

        public override CrawlSyntaxNode CreateRed(CrawlSyntaxNode parent, int indexInParent)
        {
            return new ConstructNode(parent, this, indexInParent);
        }

        internal override GreenCrawlSyntaxNode WithReplacedChild(GreenCrawlSyntaxNode newChild, int index)
        {
            switch (index)
            {
                //TODO: parameter skal udskiftes
                //TODO Return type. Hvis brugeren beder om return værdien, er der en exception i øjeblikket. 
                case 0:
                    return new GreenConstructNode(Interval, ProtectionLevel, null, Body);
                case 1:
                    return new GreenConstructNode(Interval, ProtectionLevel, null,(GreenBlockNode)newChild);
                default:
                    throw new ArgumentOutOfRangeException();
                


            }


        }

        
        public override GreenCrawlSyntaxNode GetChildAt(int slot)
        {
            switch (slot)
            {
                //TODO: Pramaeter skal faktisk retuneres.
                case 0:
                    throw new NotImplementedException();
                case 1:
                    throw new NotImplementedException();
                case 2:
                    return Body;
                default:
                    return default(GreenCrawlSyntaxNode);
            }
        }
    }



}
