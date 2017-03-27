using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler
{
    /// <summary>
    /// Very very simple syntehesis typechecker. When creating the actual typechecker, start from SyntaxTreeVisitor[CrawlType] not this.
    /// It is merly intended to showcase how the concept works.
    /// Lots of stuff it dosen't understand, incl calls, variables and casting, soo...
    /// </summary>
    class POCTypechecker : SimpleSyntaxTreeVisitor<POCType>
    {
        //The very simple "add types together" function.
        //Cut it all out. Way too many things it won't catch, and no way to check context
        protected override POCType Combine(params POCType[] parts)
        {
            if(parts.Length == 0) return POCType.Notype;
            
            POCType part = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                if(parts[i] != part && parts[i] != POCType.Notype)
                    return POCType.Error;
            }

            return part;
        }

        protected override POCType VisitLiteral(LiteralNode node)
        {
            switch (node.LiteralType)
            {
                case LiteralType.String:
                    return POCType.String;
                case LiteralType.Int:
                    return POCType.Int;
                case LiteralType.Float:
                    return POCType.Float;
                case LiteralType.Boolean:
                    return POCType.Bool;
                case LiteralType.Real:
                    return POCType.Float;;
            }

            return POCType.Error;
        }

        protected override POCType VisitVariableNode(VariableNode node)
        {
            //Every variable is int. Yes its true
            return POCType.Int;
        }
    }

    internal enum POCType
    {
        Notype,
        Error,
        Int,
        Float,
        String,
        Bool
    }
}
