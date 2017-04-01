using System;
using System.Collections.Generic;
using System.Linq;
using libcompiler.SyntaxTree;
using libcompiler.SyntaxTree.Nodes;
using libcompiler.TypeSystem;

namespace libcompiler.TypeChecker
{
    public class TypeChecker : BaseSyntaxTreeVisitor<CrawlType>
    {
        //TODO: should be TypeErrorInformation : ErrorInformation
        public List<string> TypeErrors = new List<string>();

        protected override CrawlType VisitList(IEnumerable<CrawlSyntaxNode> node)
        {
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