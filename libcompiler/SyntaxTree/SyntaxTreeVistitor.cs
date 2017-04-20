using System;
using System.Collections.Generic;
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
                    VisitWhile((SelectiveFlowNode) node);
                    break;
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
                case NodeType.MethodDecleration:
                    VisitMethodDecleration((MethodDeclerationNode) node);
                    break;
                case NodeType.Block:
                    VisitBlock((BlockNode) node);
                    break;
                case NodeType.Imports:
                    VisitImports((ListNode<ImportNode>) node);
                    break;
                case NodeType.Import:
                    VisitImport((ImportNode) node);
                    break;
                case NodeType.TranslationUnit:
                    VisitTranslationUnit((TranslationUnitNode) node);
                    break;
                case NodeType.Literal:
                    VisitLiteral((LiteralNode) node);
                    break;
                case NodeType.NodeList:
                    VisitList((IEnumerable<CrawlSyntaxNode>) node);
                    break;
                case NodeType.Type:
                    VisitType((TypeNode) node);
                    break;
                case NodeType.UnaryExpression:
                    VisitUnary((UnaryNode) node);
                    break;
                case NodeType.Reference:
                    VisitReference((ReferenceNode) node);
                    break;
                case NodeType.GenericUnpack:
                    VisitGenericUnpack((GenericsUnpackNode) node);
                    break;
                case NodeType.GenericParametersNode:
                    VisitGenericParameter((GenericParameterNode) node);
                    break;
                case NodeType.NameSpace:
                    VisitNameSpace((NameSpaceNode)node);
                    break;
                case NodeType.Identifier:
                    VisitIdentifierNode((IdentifierNode) node);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(node.ToString());
            }
        }

        private void VisitIdentifierNode(IdentifierNode node)
        {
        }

        private void VisitNameSpace(NameSpaceNode node)
        {
            
        }

        private void VisitReference(ReferenceNode node)
        {
            Visit(node.ParameterType);
        }

        protected virtual void VisitReference(ReferenceNode node)
        {
            Visit(node.Target);
        }

        protected virtual void VisitGenericParameter(GenericParameterNode node)
        {
        }

        protected virtual void VisitType(TypeNode node)
        {
        }

        protected virtual void VisitGenericUnpack(GenericsUnpackNode node)
        {
            Visit(node.Target);
            Visit(node.Generics);
        }

        protected virtual void VisitImports(ListNode<ImportNode> node)
        {
            foreach (ImportNode n in node)
            {
                Visit(n);
            }
        }

        private void VisitImport(ImportNode node)
        {

        }

        private void VisitUnary(UnaryNode node)
        {
            Visit(node.Target);
        }

        private void VisitList(IEnumerable<CrawlSyntaxNode> node) 
        {
            foreach (CrawlSyntaxNode val in node)
            {
                Visit(val);
            }
        }

        protected virtual void VisitWhile(SelectiveFlowNode node)
        {
            Visit(node.Check);
            Visit(node.Primary);
        }

        protected virtual  void VisitMulti(MultiChildExpressionNode node)
        {
            Visit(node.Arguments);
        }

        protected virtual void VisitReturnStatement(ReturnStatement node)
        {
            if(node.ReturnValue != null) Visit(node.ReturnValue);
        }

        protected virtual void VisitAssignment(AssignmentNode node)
        {
            Visit(node.Target);
            Visit(node.Value);
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

            Visit(node.Arguments);
        }

        protected virtual void VisitMethodDecleration(MethodDeclerationNode node)
        {
            Visit(node.MethodSignature);
            Visit(node.ParameterIdentifiers);
            Visit(node.Identfier);
            Visit(node.GenericParameters);
            Visit(node.Body);
        }

        protected virtual void VisitVariableDeclerationSingle(SingleVariableDecleration node)
        {
            Visit(node.Identifier);
            if(node.DefaultValue != null)
                Visit(node.DefaultValue);
        }

        protected virtual void VisitVariableDecleration(VariableDeclerationNode node)
        {
            Visit(node.Declerations);
        }

        protected virtual void VisitClassDecleration(ClassDeclerationNode node)
        {
            Visit(node.GenericParameters);
            Visit(node.BodyBlock);
        }

        protected virtual void VisitLiteral(LiteralNode node)
        {
            
        }

        protected virtual void VisitTranslationUnit(TranslationUnitNode node)
        {
            Visit(node.Imports);
            Visit(node.NameSpace);
            Visit(node.Code);
        }

        protected virtual void VisitBlock(BlockNode node)
        {
            foreach (CrawlSyntaxNode child in node)
            {
                Visit(child);
            }
        }
    }
}
