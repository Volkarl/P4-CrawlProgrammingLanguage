using System;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.SyntaxTree
{
    public abstract class SyntaxTreeVistitor
    {
        public virtual void Visit(CrawlSyntaxNode node)
        {
            //  s/\.([^:]*):/.$1:\n\t\t\t\t\treturn Visit$1(($1Node) node);/
            switch (node.Type)
            {
                case NodeType.Forloop:
                    VisitForLoop((ForLoopNode) node);
                    break;
                case NodeType.If:
                case NodeType.IfElse:
                    VisitIf((SelectiveFlowNode) node);
                    break;
                case NodeType.While:
                    throw new NotImplementedException(); ;
                case NodeType.Return:
                    VisitReturnStatement((ReturnStatement) node);
                    break;
                case NodeType.Assignment:
                    VisitAssignment((AssignmentNode) node);
                    break;
                case NodeType.Index:
                case NodeType.Call:
                    VisitCall((CallishNode) node);
                    break;
                case NodeType.MultiExpression:
                    VisitMulti((MultiChildExpressionNode) node);
                    break;
                case NodeType.BinaryExpression:
                    VisitBinary((BinaryNode) node);
                    break;
                case NodeType.Variable:
                    VisitVariableNode((VariableNode) node);
                    break;
                case NodeType.ClassDecleration:
                    VisitClassDecleration((ClassDeclerationNode) node);
                    break;
                case NodeType.VariableDecleration:
                    VisitVariableDecleration((VariableDeclerationNode) node);
                    break;
                case NodeType.VariableDeclerationSingle:
                    VisitVariableDeclerationSingle((SingleVariableDecleration) node);
                    break;
                case NodeType.FunctionDecleration:
                    VisitFunctionDecleration((FunctionDeclerationNode) node);
                    break;
                case NodeType.Block:
                    VisitBlock((BlockNode) node);
                    break;
                case NodeType.Import:
                    throw new NotImplementedException();
                case NodeType.CompilationUnit:
                    VisitCompiliationUnit((CompiliationUnitNode) node);
                    break;
                case NodeType.Literal:
                    VisitLiteral((LiteralNode) node);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual  void VisitMulti(MultiChildExpressionNode node)
        {
            foreach (ExpressionNode argument in node.Arguments)
            {
                Visit(argument);
            }
        }

        protected virtual void VisitReturnStatement(ReturnStatement node)
        {
            if(node.ReturnValue != null) Visit(node.ReturnValue);
        }

        protected virtual void VisitAssignment(AssignmentNode node)
        {
            Visit(node.LeftHandSide);
            Visit(node.RightHandSide);
        }

        protected virtual void VisitIf(SelectiveFlowNode node)
        {
            Visit(node.Check);
            Visit(node.Primary);
            if(node.Alternative !=  null) Visit(node.Alternative);
        }

        protected virtual void VisitBinary(BinaryNode node)
        {
            Visit(node.LeftHandSide);
            Visit(node.RightHandSide);
        }

        protected virtual void VisitForLoop(ForLoopNode node)
        {
            Visit(node.Iteratior);
            Visit(node.Block);
            {
                
            }
        }

        protected virtual void VisitVariableNode(VariableNode node)
        {
            
        }

        protected virtual void VisitCall(CallishNode node)
        {
            Visit(node.Target);

            foreach (ExpressionNode expressionNode in node.Arguments)
            {
                Visit(expressionNode);
            }
        }

        protected virtual void VisitFunctionDecleration(FunctionDeclerationNode node)
        {
            Visit(node.BodyBlock);
        }

        protected virtual void VisitVariableDeclerationSingle(SingleVariableDecleration node)
        {
            if(node.DefaultValue != null)
                Visit(node.DefaultValue);
        }

        protected virtual void VisitVariableDecleration(VariableDeclerationNode node)
        {
            foreach (SingleVariableDecleration decleration in node.Declerations)
            {
                Visit(decleration);
            }
        }

        protected virtual void VisitClassDecleration(ClassDeclerationNode node)
        {
            Visit(node.BodyBlock);
        }

        protected virtual void VisitLiteral(LiteralNode node)
        {
            
        }

        protected virtual void VisitCompiliationUnit(CompiliationUnitNode node)
        {
            foreach (ImportNode nodeImport in node.Imports)
            {
                Visit(nodeImport);
            }

            Visit(node.Code);
        }

        protected virtual void VisitBlock(BlockNode node)
        {
            foreach (CrawlSyntaxNode child in node.Children)
            {
                Visit(child);
            }
        }
    }

    public class FooVisitor : SyntaxTreeVistitor
    { }
    
}
