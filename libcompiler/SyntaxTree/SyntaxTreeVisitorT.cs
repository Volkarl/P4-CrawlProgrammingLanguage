using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.SyntaxTree
{
    public abstract class BaseSyntaxTreeVisitor<T>
    {
        public virtual T Visit(CrawlSyntaxNode node)
        {
            //  s/\.([^:]*):/.$1:\n\t\t\t\t\treturn Visit$1(($1Node) node);/
            switch (node.Type)
            {
                case NodeType.Forloop:
                    return VisitForLoop((ForLoopNode)node);
                case NodeType.If:
                case NodeType.IfElse:
                   return VisitIf((SelectiveFlowNode)node);
                    
                case NodeType.While:
                    return VisitWhile((SelectiveFlowNode)node);
                    
                case NodeType.Return:
                    return VisitReturnStatement((ReturnStatement)node);
                    
                case NodeType.Assignment:
                    return VisitAssignment((AssignmentNode)node);
                    
                case NodeType.Index:
                case NodeType.Call:
                    return VisitCall((CallishNode)node);
                    
                case NodeType.MultiExpression:
                    return VisitMulti((MultiChildExpressionNode)node);
                    
                case NodeType.BinaryExpression:
                    return VisitBinary((BinaryNode)node);
                    
                case NodeType.Variable:
                    return VisitVariableNode((VariableNode)node);
                    
                case NodeType.ClassDecleration:
                    return VisitClassDecleration((ClassDeclerationNode)node);
                    
                case NodeType.VariableDecleration:
                    return VisitVariableDecleration((VariableDeclerationNode)node);
                    
                case NodeType.VariableDeclerationSingle:
                    return VisitVariableDeclerationSingle((SingleVariableDecleration)node);
                    
                case NodeType.MethodDecleration:
                    return VisitFunctionDecleration((MethodDeclerationNode)node);
                    
                case NodeType.Block:
                    return VisitBlock((BlockNode)node);
                    
                case NodeType.Import:
                    throw new NotImplementedException();
                case NodeType.TranslationUnit:
                    return VisitCompiliationUnit((TranslationUnitNode)node);
                    
                case NodeType.Literal:
                    return VisitLiteral((LiteralNode)node);
                    
                case NodeType.NodeList:
                    return VisitList((IEnumerable<CrawlSyntaxNode>)node);
                case NodeType.UnaryExpression:
                    return VisitUnary((UnaryNode)node);
                case NodeType.Type:
                    return VisitType((TypeNode) node);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected T VisitType(TypeNode node)
        {
            return default(T);
        }

        protected T VisitUnary(UnaryNode node)
        {
            return Visit(node.Target);
        }

        //TODO: WAS PRIVATE? INVESTIGATE
        protected abstract T VisitList(IEnumerable<CrawlSyntaxNode> node);

        protected abstract T VisitWhile(SelectiveFlowNode node);

        protected virtual T VisitMulti(MultiChildExpressionNode node)
        {
            return Visit(node.Arguments);
        }

        protected virtual T VisitReturnStatement(ReturnStatement node)
        {
            if (node.ReturnValue != null) return Visit(node.ReturnValue);

            return default(T);
        }

        protected abstract T VisitAssignment(AssignmentNode node);

        protected abstract T VisitIf(SelectiveFlowNode node);

        protected abstract T VisitBinary(BinaryNode node);

        protected abstract T VisitForLoop(ForLoopNode node);

        protected virtual T VisitVariableNode(VariableNode node)
        {
            return default(T);
        }

        protected abstract T VisitCall(CallishNode node);

        protected virtual T VisitFunctionDecleration(MethodDeclerationNode node)
        {
            return Visit(node.Body);
        }

        protected virtual T VisitVariableDeclerationSingle(SingleVariableDecleration node)
        {
            if (node.DefaultValue != null)
                return Visit(node.DefaultValue);

            return default(T);
        }

        protected virtual T VisitVariableDecleration(VariableDeclerationNode node)
        {
            return Visit(node.Declerations);
        }

        protected virtual T VisitClassDecleration(ClassDeclerationNode node)
        {
            return Visit(node.BodyBlock);
        }

        protected virtual T VisitLiteral(LiteralNode node)
        {
            return default(T);
        }

        protected abstract T VisitCompiliationUnit(TranslationUnitNode node);

        protected abstract T VisitBlock(BlockNode node);

    }

    public abstract class SimpleSyntaxTreeVisitor<T> : BaseSyntaxTreeVisitor<T>
    {
        protected abstract T Combine(params T[] parts);

        protected override T VisitList(IEnumerable<CrawlSyntaxNode> node)
        {
            return Combine(node.Select(Visit).ToArray());
        }

        protected override T VisitWhile(SelectiveFlowNode node)
        {
            return Combine(Visit(node.Check), Visit(node.Primary));
        }

        protected override T VisitAssignment(AssignmentNode node)
        {
            return Combine(Visit(node.Target), Visit(node.Value));
        }

        protected override T VisitIf(SelectiveFlowNode node)
        {
           
            if (node.Alternative != null)
                return Combine(Visit(node.Check), Visit(node.Primary), Visit(node.Alternative));
            else
                return Combine(Visit(node.Check), Visit(node.Primary));
        }

        protected override T VisitBinary(BinaryNode node)
        {
            return Combine(Visit(node.LeftHandSide), Visit(node.RightHandSide));
        }

        protected override T VisitForLoop(ForLoopNode node)
        {
            return Combine(Visit(node.InducedFieldType), Visit(node.InducedFieldName), Visit(node.Iteratior), Visit(node.Block));
        }

        protected override T VisitCall(CallishNode node)
        {
            return Combine(Visit(node.Target), Visit(node.Arguments));
        }

        protected override T VisitCompiliationUnit(TranslationUnitNode node)
        {
            return Combine(Visit(node.Imports), Visit(node.Code));
        }

        protected override T VisitBlock(BlockNode node)
        {
            return Combine(node.Select(Visit).ToArray());
        }
    }
}
