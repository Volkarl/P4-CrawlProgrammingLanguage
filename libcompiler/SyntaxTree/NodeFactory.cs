using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes;
using _ = libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree
{
    public static class NodeFactory
    {
        /// <summary>
        /// Returns red counterpart of node.
        /// </summary>
        private static CrawlSyntaxNode Wrap(_.GreenCrawlSyntaxNode greenNode)
        {
            return greenNode.CreateRed(null, 0);
        }

        /// <summary>
        /// Returns green node from red node.
        /// </summary>
        /// <param name="n">The node extract green node from.</param>
        /// <typeparam name="T">The type of that node.</typeparam>
        /// <typeparam name="GreenT">The type of the green counterpart.</typeparam>
        /// <returns></returns>
        private static GreenT GetGreenNode<T, GreenT>(T n)
            where T:CrawlSyntaxNode
            where GreenT:_.GreenCrawlSyntaxNode
        {
            return (GreenT) CrawlSyntaxNode.ExtractGreenNode(n);
        }

        /// <summary>
        /// Create new Green GreenListNode from series of any kind of red nodes.
        /// </summary>
        /// <param name="i">Series of red nodes</param>
        /// <typeparam name="T">Type of red node.</typeparam>
        /// <returns>A new green GreenListNode</returns>
        private static _.GreenListNode<T> GetListOfGreenNodes<T>(IEnumerable<T> i) where T : CrawlSyntaxNode
        {
            //TODO correct interval
            return new _.GreenListNode<T>(default(Interval), i.Select(CrawlSyntaxNode.ExtractGreenNode));
        }

        internal static NameSpaceNode NameSpaceNode(Interval sourceInterval, string nameSpace)
        {
            var green = new _.GreenNameSpaceNode(sourceInterval, nameSpace);
            return (NameSpaceNode)Wrap( green );
        }

        public static FlowNode If(Interval interval, ExpressionNode conditon, BlockNode trueBlock)
        {
            return (FlowNode) Wrap(
                new _.GreenSelectiveFlowNode(
                    interval, 
                    NodeType.If,
                    (_.GreenExpressionNode) CrawlSyntaxNode.ExtractGreenNode(conditon),
                    (_.GreenBlockNode) CrawlSyntaxNode.ExtractGreenNode(trueBlock),
                    null));
        }

        public static FlowNode IfElse(Interval interval, ExpressionNode conditon, BlockNode trueBlock, BlockNode falseBlock)
        {
            return (FlowNode) Wrap(
                new _.GreenSelectiveFlowNode(
                    interval, 
                    NodeType.IfElse, 
                    GetGreenNode<ExpressionNode, _.GreenExpressionNode>(conditon),
                    GetGreenNode<BlockNode, _.GreenBlockNode>(trueBlock),
                    GetGreenNode<BlockNode, _.GreenBlockNode>(falseBlock)));
        }

        public static FlowNode Forloop(Interval interval, TypeNode inducedVariableType, VariableNode inducedVariableName, ExpressionNode iteratior, BlockNode block)
        {
            return (FlowNode) Wrap(
                new _.GreenForLoopNode(
                    interval,
                    GetGreenNode<TypeNode, _.GreenTypeNode> (inducedVariableType),
                    GetGreenNode<VariableNode, _.GreenVariableNode>(inducedVariableName),
                    GetGreenNode<ExpressionNode, _.GreenExpressionNode>(iteratior),
                    GetGreenNode<BlockNode, _.GreenBlockNode>(block)));
            
        }

        public static FlowNode WhileLoop(Interval interval, ExpressionNode condition, BlockNode block)
        {
            return (FlowNode) Wrap(new _.GreenSelectiveFlowNode(interval, NodeType.While, GetGreenNode<ExpressionNode, _.GreenExpressionNode>(condition), GetGreenNode<BlockNode, _.GreenBlockNode>(block), null));
        }

        public static ConstructNode Constructor (Interval interval, ProtectionLevel protectionlevel, BlockNode body)
        {
            return (ConstructNode)Wrap(new _.GreenConstructNode(interval, protectionlevel, null, GetGreenNode<BlockNode, _.GreenBlockNode>(body)));
        }


        public static MethodDeclerationNode Method(
            Interval interval,
            ProtectionLevel protectionLevel,
            TypeNode methodSignature,
            List<IdentifierNode> parameterIdentifiers,
            IEnumerable<GenericParameterNode> genericParameterNodes,
            IdentifierNode identifier,
            BlockNode block)
        {
            return (MethodDeclerationNode) Wrap(
                new _.GreenMethodDeclerationNode(
                    interval,
                    protectionLevel,
                    GetGreenNode<TypeNode, _.GreenTypeNode> (methodSignature),
                    GetListOfGreenNodes(parameterIdentifiers),
                    GetListOfGreenNodes(genericParameterNodes),
                    GetGreenNode<IdentifierNode, _.GreenIdentifierNode>(identifier),
                    GetGreenNode<BlockNode, _.GreenBlockNode>(block)
                ));
        }

        public static SingleVariableDecleration SingleVariable(Interval interval, VariableNode identifier)
        {
            return SingleVariable(interval, identifier, null);
        }

        public static SingleVariableDecleration SingleVariable(Interval interval, VariableNode identifier, ExpressionNode value)
        {
            return (SingleVariableDecleration) Wrap(
                new _.GreenSingleVariableDecleration(
                    interval,
                    GetGreenNode<VariableNode, _.GreenVariableNode>(identifier),
                    GetGreenNode<ExpressionNode, _.GreenExpressionNode>(value))
            );
        }

        public static VariableDeclerationNode VariableDecleration(Interval interval, ProtectionLevel protectionLevel, TypeNode type, IEnumerable<SingleVariableDecleration> declerations)
        {
            return (VariableDeclerationNode) Wrap(
                new _.GreenVariableDeclerationNode(
                    interval,
                    protectionLevel,
                    GetGreenNode<TypeNode, _.GreenTypeNode>(type),
                    GetListOfGreenNodes(declerations))
            );
        }

        public static ClassDeclerationNode ClassDecleration(
            Interval interval,
            ProtectionLevel protectionLevel,
            IdentifierNode identifier,
            IdentifierNode ancestor,
            IEnumerable<GenericParameterNode> genericParameterNodes,
            BlockNode bodyBlock
        )
        {
            return (ClassDeclerationNode) Wrap(
                new _.GreenClassDeclerationNode(
                    interval,
                    protectionLevel,
                    GetGreenNode<IdentifierNode, _.GreenIdentifierNode>(identifier),
                    GetGreenNode<IdentifierNode, _.GreenIdentifierNode>(ancestor),
                    GetListOfGreenNodes(genericParameterNodes),
                    GetGreenNode<BlockNode, _.GreenBlockNode>(bodyBlock)
                )
            );
        }

        public static BlockNode Block(Interval interval, IEnumerable<CrawlSyntaxNode> contents)
        {
            return (BlockNode) Wrap(
                new _.GreenBlockNode(interval, contents.Select(CrawlSyntaxNode.ExtractGreenNode)));
        }

        public static ReturnStatement Return(Interval interval, ExpressionNode returnValue)
        {
            return (ReturnStatement) Wrap(
                new _.GreenReturnStatement(interval, GetGreenNode<ExpressionNode, _.GreenExpressionNode>(returnValue)));
        }

        public static ReturnStatement Return(Interval interval)
        {
            return Return(interval, null);
        }

        public static VariableNode VariableAccess(Interval interval, string name)
        {
            return (VariableNode) Wrap(
                new _.GreenVariableNode(interval, name)
                );
        }

        public static BinaryNode MemberAccess(Interval interval, ExpressionNode target, VariableNode sub)
        {
            return (BinaryNode) Wrap(
                new _.GreenBinaryNode(
                    interval,
                    GetGreenNode<ExpressionNode, _.GreenExpressionNode>(target),
                    GetGreenNode<VariableNode, _.GreenVariableNode>(sub),
                    ExpressionType.SubfieldAccess
                )
            );
        }

        public static CallishNode Index(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return (CallishNode) Wrap(
                new _.GreenCallishNode(
                    interval,
                    GetGreenNode<ExpressionNode,
                        _.GreenExpressionNode>(target),
                    GetListOfGreenNodes(arguments),
                    ExpressionType.Index
                )
            );

        }

        public static CallishNode Call(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return (CallishNode) Wrap(
                new _.GreenCallishNode(interval,
                    GetGreenNode<ExpressionNode,
                        _.GreenExpressionNode>(target),
                    GetListOfGreenNodes(arguments),
                    ExpressionType.Invocation
                )
            );
        }

        public static AssignmentNode Assignment(Interval interval, ExpressionNode target, ExpressionNode value)
        {
            return (AssignmentNode) Wrap(
                new _.GreenAssignmentNode(interval,
                    GetGreenNode<ExpressionNode,
                        _.GreenExpressionNode>(target),
                    GetGreenNode<ExpressionNode,_.GreenExpressionNode>(value)));
        }

        //Denne skal skrives om senere, så den også gememer namespace noden.
        public static TranslationUnitNode TranslationUnit(Interval interval, IEnumerable<ImportNode> importNodes,NameSpaceNode nameSpace, BlockNode rootCode)
        {
            return (TranslationUnitNode) Wrap(
                new _.GreenTranslationUnitNode(
                    interval,
                    GetListOfGreenNodes(importNodes),
                    GetGreenNode<NameSpaceNode, _.GreenNameSpaceNode> (nameSpace),
                    GetGreenNode<BlockNode, _.GreenBlockNode>(rootCode)
                )
            );
        }

        //TODO: Also expose this as individual methods
        public static ExpressionNode MultiExpression(Interval interval, ExpressionType type, IEnumerable<ExpressionNode> sources)
        {
            return (ExpressionNode) Wrap(
                new _.GreenMultiChildExpressionNode(interval, type, GetListOfGreenNodes(sources)));
        }

        public static ExpressionNode UnaryExpression(Interval interval, ExpressionType type, ExpressionNode target)
        {
            return (ExpressionNode)Wrap(
                new _.GreenUnaryNode(interval, type, GetGreenNode<ExpressionNode, _.GreenExpressionNode>(target))
                );
        }

        //TODO: Also expose this as individual methods
        //TODO: Automatically catch invalid ExpressionTypes for this and delegate to relevant target
        public static ExpressionNode BinaryExpression(Interval interval, ExpressionType type, ExpressionNode leftHandSide, ExpressionNode rightHandSide)
        {
            return (ExpressionNode) Wrap(
                new _.GreenBinaryNode(
                    interval,
                    GetGreenNode<ExpressionNode, _.GreenExpressionNode>(leftHandSide),
                    GetGreenNode<ExpressionNode, _.GreenExpressionNode>(rightHandSide),
                    type
                )
            );
        }


        private static ExpressionNode MkLiteral(Interval interval, string text, LiteralType type)
            => (ExpressionNode) Wrap(new _.GreenLiteralNode(interval, text, type));
        public static ExpressionNode StringConstant(Interval interval, string textRepresentation)
        {
            return MkLiteral(interval, textRepresentation, LiteralType.String);
        }

        public static ExpressionNode IntegerConstant(Interval interval, string textRepresentation)
        {
            return MkLiteral(interval, textRepresentation, LiteralType.Int);
        }

        public static ExpressionNode BooleanConstant(Interval interval, string textRepresentation)
        {
            return MkLiteral(interval, textRepresentation, LiteralType.Boolean);
        }

        public static ExpressionNode RealConstant(Interval interval, string textRepresentation)
        {
            return MkLiteral(interval, textRepresentation, LiteralType.Real);
        }

        public static VariableNode VariableNode(Interval interval, string name)
        {
            return (VariableNode) Wrap(new _.GreenVariableNode(interval, name));
        }

        public static IdentifierNode TokenNode(Interval interval, string name)
        {
            return (IdentifierNode) Wrap(new _.GreenIdentifierNode(interval, name));
        }

        public static TypeNode Type(Interval interval, CrawlType crawlType, bool isReference)
        {
            return (TypeNode) Wrap(new _.GreenTypeNode(interval, crawlType, isReference));
        }

        /// <summary>
        /// Makes GreenListNode representing a series of import directives.
        /// </summary>
        /// <param name="interval">Positions of start- and end-character in original source file. For debugging.</param>
        /// <param name="children">Import directives.</param>
        /// <returns>What was made. </returns>
        // Green node created, red representation returned.
        public static ListNode<ImportNode> ImportsNode(Interval interval, IEnumerable<ImportNode> children)
        {
            return (ListNode<ImportNode>) Wrap(GetListOfGreenNodes(children));
        }

        /// <summary>
        /// Makes GreenImportNode representing a single import directive.
        /// </summary>
        /// <param name="interval">Positions of start- and end-character in original source file. For debugging.</param>
        /// <param name="modulePath">A series of period-separated identifiers denoting the path to the module to be imported.</param>
        /// <returns>What was made.</returns>
        public static ImportNode ImportNode(Interval interval, string modulePath)
        {
            return (ImportNode) Wrap(new _.GreenImportNode(interval, modulePath));
        }

        public static GenericsUnpackNode GenericsUnpackNode(Interval interval, ExpressionNode target, IEnumerable<TypeNode> generics)
        {
            return (GenericsUnpackNode) Wrap(new _.GreenGenericUnpackNode(interval, GetGreenNode<ExpressionNode,_.GreenExpressionNode>(target), GetListOfGreenNodes(generics)));
        }

        public static GenericParameterNode GenericsParameterNode(Interval interval, string value, string limitation)
        {
            return (GenericParameterNode) Wrap(new _.GreenGenericParameterNode(interval, value, limitation));
        }

        public static ReferenceNode ReferenceNode(ExpressionNode target)
        {
            return (ReferenceNode) Wrap(new _.GreenReferenceNode(GetGreenNode<ExpressionNode, _.GreenExpressionNode>(target)));
        }

        public static ParameterNode Parameter(Interval interval, bool isReference, TypeNode type, string identifier)
        {
            return (ParameterNode) Wrap(new _.GreenParameterNode(interval, isReference, GetGreenNode<TypeNode, _.GreenTypeNode>(type), identifier));
        }
    }
}