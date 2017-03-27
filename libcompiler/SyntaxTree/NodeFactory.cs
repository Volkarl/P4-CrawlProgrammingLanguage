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

        private static _.GreenBlockNode Extract(BlockNode n)
        {
            return (_.GreenBlockNode) CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.GreenTypeNode Extract(TypeNode n)
        {
            return (_.GreenTypeNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.GreenVariableNode Extract(VariableNode n)
        {
            return (_.GreenVariableNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.GreenExpressionNode Extract(ExpressionNode n)
        {
            return (_.GreenExpressionNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.GreenIdentifierNode Extract(IdentifierNode n)
        {
            return (_.GreenIdentifierNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        /// <summary>
        /// Create new Green GreenListNode from series of any kind of red nodes.
        /// </summary>
        /// <param name="i">Series of red nodes</param>
        /// <typeparam name="T">Type of red node.</typeparam>
        /// <returns>A new green GreenListNode</returns>
        private static _.GreenListNode<T> List<T>(IEnumerable<T> i) where T : CrawlSyntaxNode
        {
            //TODO correct interval
            return new _.GreenListNode<T>(default(Interval), i.Select(CrawlSyntaxNode.ExtractGreenNode));
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
                    Extract(conditon), 
                    Extract(trueBlock), 
                    Extract(falseBlock)));
        }

        

        public static FlowNode Forloop(Interval interval, TypeNode inducedVariableType, VariableNode inducedVariableName, ExpressionNode iteratior, BlockNode block)
        {
            return (FlowNode) Wrap(
                new _.GreenForLoopNode(
                    interval,
                    Extract(inducedVariableType),
                    Extract(inducedVariableName), 
                    Extract(iteratior),
                    Extract(block)));
            
        }

        public static FlowNode WhileLoop(Interval interval, ExpressionNode condition, BlockNode block)
        {
            return (FlowNode) Wrap(new _.GreenSelectiveFlowNode(interval, NodeType.While, Extract(condition), Extract(block), null));
        }

        public static MethodDeclerationNode Function(
            Interval interval,
            ProtectionLevel protectionLevel,
            TypeNode returnType,
            IEnumerable<GenericParameterNode> genericParameterNodes,
            VariableNode identifier,
            BlockNode block
        )
        {
            return (MethodDeclerationNode) Wrap(
                new _.GreenMethodDeclerationNode(
                    interval,
                    protectionLevel,
                    Extract(returnType),
                    List(genericParameterNodes),
                    Extract(identifier),
                    Extract(block)
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
                    Extract(identifier),
                    Extract(value))
            );
        }

        public static VariableDeclerationNode VariableDecleration(Interval interval, ProtectionLevel protectionLevel, TypeNode type, IEnumerable<SingleVariableDecleration> declerations)
        {
            return (VariableDeclerationNode) Wrap(
                new _.GreenVariableDeclerationNode(
                    interval,
                    protectionLevel,
                    Extract(type),
                    List(declerations))
            );
        }

        public static ClassDeclerationNode ClassDecleration(
            Interval interval,
            ProtectionLevel protectionLevel,
            IdentifierNode identifier,
            IEnumerable<GenericParameterNode> genericParameterNodes,
            BlockNode bodyBlock
        )
        {
            return (ClassDeclerationNode) Wrap(
                new _.GreenClassDeclerationNode(
                    interval,
                    protectionLevel,
                    Extract(identifier),
                    List(genericParameterNodes),
                    Extract(bodyBlock)
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
                new _.GreenReturnStatement(interval, Extract(returnValue)));
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
                new _.GreenBinaryNode(interval, Extract(target), Extract(sub), ExpressionType.SubfieldAccess)
            );
        }

        public static CallishNode Index(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return (CallishNode) Wrap(
                new _.GreenCallishNode(interval, Extract(target), List(arguments), ExpressionType.Index));

        }

        public static CallishNode Call(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return (CallishNode) Wrap(
                new _.GreenCallishNode(interval, Extract(target), List(arguments), ExpressionType.Invocation));
        }

        public static AssignmentNode Assignment(Interval interval, ExpressionNode target, ExpressionNode value)
        {
            return (AssignmentNode) Wrap(
                new _.GreenAssignmentNode(interval, Extract(target), Extract(value)));
        }

        public static TranslationUnitNode TranslationUnit(Interval interval, IEnumerable<ImportNode> importNodes, BlockNode rootCode)
        {
            return (TranslationUnitNode) Wrap(
                new _.GreenTranslationUnitNode(interval, List(importNodes), Extract(rootCode)));
        }

        //TODO: Also expose this as individual methods
        public static ExpressionNode MultiExpression(Interval interval, ExpressionType type, IEnumerable<ExpressionNode> sources)
        {
            return (ExpressionNode) Wrap(
                new _.GreenMultiChildExpressionNode(interval, type, List(sources)));
        }

        public static ExpressionNode UnaryExpression(Interval interval, ExpressionType type, ExpressionNode target)
        {
            return (ExpressionNode)Wrap(
                new _.GreenUnaryNode(interval, type, Extract(target))
                );
        }

        //TODO: Also expose this as individual methods
        //TODO: Automatically catch invalid ExpressionTypes for this and delegate to relevant target
        public static ExpressionNode BinaryExpression(Interval interval, ExpressionType type, ExpressionNode leftHandSide, ExpressionNode rightHandSide)
        {
            return (ExpressionNode) Wrap(
                new _.GreenBinaryNode(interval, Extract(leftHandSide), Extract(rightHandSide), type)
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

        public static TypeNode Type(Interval interval, CrawlType crawlType)
        {
            return (TypeNode) Wrap(new _.GreenTypeNode(interval, crawlType));
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
            return (ListNode<ImportNode>) Wrap(List(children));
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
            return (GenericsUnpackNode) Wrap(new _.GreenGenericUnpackNode(interval, Extract(target), List(generics)));
        }

        public static GenericParameterNode GenericsParameterNode(Interval interval, string value, string limitation)
        {
            return (GenericParameterNode) Wrap(new _.GreenGenericParameterNode(interval, value, limitation));
        }

        public static ReferenceNode ReferenceNode(ExpressionNode target)
        {
            return (ReferenceNode) Wrap(new _.GreenReferenceNode(Extract(target)));
        }
    }
}