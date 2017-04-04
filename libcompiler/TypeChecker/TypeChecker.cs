using System;
using System.Collections.Generic;
using System.Linq;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.TypeSystem;

namespace libcompiler.TypeChecker
{
    /* 
     * The main class of type checking. The basis principle of working is simple, but due the amount of details
     * it needs to keep track of, it gets a little bit big. (Or will grow)
     * 
     * The basic principle works by moving downwards, until it reaches the leaves. Here it either asks the scope 
     * checker for the type, or simply examines the literals.
     * 
     * From here on it moves up. Every VisitXXX method checks if there is a compatible operation between each
     * of its child types. Then it returns the type of the result.
     * 
     * It uses the 2 objects, CrawlType.Void and CrawlType.Error to represent the lack of any type and an error
     * in the type system respectively. Void can be expected sometimes, Error is supposed to propegate upwards.
     */
    public class TypeChecker : BaseSyntaxTreeVisitor<CrawlType>
    {
        //TODO: should be TypeErrorInformation : ErrorInformation
        public List<string> TypeErrors = new List<string>();

        protected override CrawlType VisitList(IEnumerable<CrawlSyntaxNode> node)
        {
            //Visits a list. Behavour is simple, check if all children has same type.
            //This won't be sufficent for real code, but it is a decent way of making it "mostly work"
            //TODO: make this throw. I don't think there are any _real_ case where this is correct behavour. Just about everything with lots of children needs special handling...
            CrawlType accomulator = null;
            foreach (CrawlSyntaxNode syntaxNode in node)
            {
                CrawlType partType = Visit(syntaxNode);
                if (accomulator == null)
                    accomulator = partType;
                else if(accomulator != partType)
                    accomulator = CrawlType.Error;
            }
            return accomulator ?? CrawlType.Void;
        }

        protected override CrawlType VisitWhile(SelectiveFlowNode node)
        {
            throw new System.NotImplementedException();
        }

        protected override CrawlType VisitAssignment(AssignmentNode node)
        {
            CrawlType destinationType = Visit(node.Target);
            CrawlType expressionValue = Visit(node.Value);

            if(expressionValue.IsAssignableTo(destinationType))
                return CrawlType.Void;

            return CrawlType.Error;
        }

        protected override CrawlType VisitIf(SelectiveFlowNode node)
        {
            throw new System.NotImplementedException();
        }

        protected override CrawlType VisitBinary(BinaryNode node)
        {
            switch (node.ExpressionType)
            {
                case ExpressionType.SubfieldAccess:
                    CrawlType lhs = Visit(node.LeftHandSide);
                    VariableNode sub = (VariableNode) node.RightHandSide;

                    TypeInformation[] inf = lhs.GetScope(sub.Name);
                    if (inf.Length == 0) return CrawlType.Error;
                    return inf[0].Type;

                    break;

            }
            throw new System.NotImplementedException();
        }

        protected override CrawlType VisitVariableNode(VariableNode node)
        {
            TypeInformation[] info = node.FindFirstScope().GetScope(node.Name);
            if(info.Length == 0) //TODO: EMIT NOT DEFINED //TODO: check for any symbol with equalish name (google edit lenght)
                return CrawlType.Error;
            return info[0].Type;
        }

        protected override CrawlType VisitForLoop(ForLoopNode node)
        {
            throw new System.NotImplementedException();
        }

        protected override CrawlType VisitCall(CallishNode node)
        {
            throw new System.NotImplementedException();
        }

        protected override CrawlType VisitCompiliationUnit(TranslationUnitNode node)
        {
            return Visit(node.Code);
        }

        protected override CrawlType VisitLiteral(LiteralNode node)
        {
            switch (node.LiteralType)
            {
                case LiteralType.String:
                    return CrawlType.String;
                case LiteralType.Int:
                    return CrawlType.Int;
                case LiteralType.Float:
                    break;
                case LiteralType.Boolean:
                    break;
                case LiteralType.Real:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new NotImplementedException();
        }

        protected override CrawlType VisitBlock(BlockNode node)
        {
            int errors = node.Count(x => Visit(x) == CrawlType.Error);
            if(errors > 0) return CrawlType.Error;
            return CrawlType.Void;
        }

        protected override CrawlType VisitVariableDecleration(VariableDeclerationNode node)
        {
            CrawlType type = node.DeclerationType.ExportedType;
            int errors = 0;
            foreach (SingleVariableDecleration decleration in node.Declerations)
            {
                if (decleration.DefaultValue != null)
                {
                    CrawlType assignment = Visit(decleration.DefaultValue);
                    if (!assignment.IsAssignableTo(type))
                        errors++; //TODO: LOG ERROR

                }
            }

            if(errors == 0)
                return CrawlType.Void;
            else
                return CrawlType.Error;
        }
    }
}