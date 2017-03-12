using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes;
using _ = libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree
{
    public static class NodeFactory
    {
        private static CrawlSyntaxNode Wrap(_.GreenNode selectiveFlowNode)
        {
            return CrawlSyntaxTree.FromGreen(selectiveFlowNode, "<Unknown>").RootNode;
        }

        private static _.BlockNode Extract(BlockNode n)
        {
            return (_.BlockNode) CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.TypeNode Extract(TypeNode n)
        {
            return (_.TypeNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.VariableNode Extract(VariableNode n)
        {
            return (_.VariableNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.ExpressionNode Extract(ExpressionNode n)
        {
            return (_.ExpressionNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.TokenNode Extract(TokenNode n)
        {
            return (_.TokenNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private static _.ListNode<T> List<T>(IEnumerable<T> i) where T : CrawlSyntaxNode
        {
            //FIXME INTERVAL
            return new _.ListNode<T>(default(Interval), i.Select(CrawlSyntaxNode.ExtractGreenNode));
        }

        public static FlowNode If(Interval interval, ExpressionNode conditon, BlockNode trueBlock)
        {
            return (FlowNode) Wrap(
                new _.SelectiveFlowNode(
                    interval, 
                    _.SelectiveFlowNode.FlowType.If,
                    (_.ExpressionNode) CrawlSyntaxNode.ExtractGreenNode(conditon),
                    (_.BlockNode) CrawlSyntaxNode.ExtractGreenNode(trueBlock),
                    null));
        }

        public static FlowNode IfElse(Interval interval, ExpressionNode conditon, BlockNode trueBlock, BlockNode falseBlock)
        {
            return (FlowNode) Wrap(
                new _.SelectiveFlowNode(
                    interval, 
                    _.SelectiveFlowNode.FlowType.IfElse, 
                    Extract(conditon), 
                    Extract(trueBlock), 
                    Extract(falseBlock)));
                

        }

        

        public static FlowNode Forloop(Interval interval, TypeNode inducedVariableType, VariableNode inducedVariableName, ExpressionNode iteratior, BlockNode block)
        {
            return (FlowNode) Wrap(
                new _.ForLoopNode(
                    interval,
                    Extract(inducedVariableType),
                    Extract(inducedVariableName), 
                    Extract(iteratior),
                    Extract(block)));
            
        }

        public static FlowNode WhileLoop(Interval interval, ExpressionNode condition, BlockNode block)
        {
            return (FlowNode) Wrap(new _.SelectiveFlowNode(interval, _.SelectiveFlowNode.FlowType.While, Extract(condition), Extract(block), null));
        }

        public static FunctionDeclerationNode Function(Interval interval, ProtectionLevel protectionLevel, TypeNode functionType, VariableNode identifier, BlockNode block)
        {
            return (FunctionDeclerationNode) Wrap(
                new _.FunctionDeclerationNode(
                    interval,
                    protectionLevel,
                    Extract(functionType),
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
                new _.SingleVariableDecleration(
                    interval,
                    Extract(identifier),
                    Extract(value))
            );
        }

        public static VariableDeclerationNode VariableDecleration(Interval interval, ProtectionLevel protectionLevel, TypeNode type, IEnumerable<SingleVariableDecleration> declerations)
        {
            return (VariableDeclerationNode) Wrap(
                new _.VariableDeclerationNode(
                    interval,
                    protectionLevel,
                    Extract(type),
                    List(declerations))
            );
        }

        public static ClassDeclerationNode ClassDecleration(Interval interval, ProtectionLevel protectionLevel, TokenNode identifier, BlockNode bodyBlock)
        {
            return (ClassDeclerationNode) Wrap(
                new _.ClassDeclerationNode(interval, protectionLevel, Extract(identifier), Extract(bodyBlock))
            );
        }

        public static BlockNode Block(Interval interval, IEnumerable<CrawlSyntaxNode> contents)
        {
            return (BlockNode) Wrap(
                new _.BlockNode(interval, contents.Select(CrawlSyntaxNode.ExtractGreenNode)));
        }

        public static ReturnStatement Return(Interval interval, ExpressionNode returnValue)
        {
            return (ReturnStatement) Wrap(
                new _.ReturnStatement(interval, Extract(returnValue)));
        }

        public static ReturnStatement Return(Interval interval)
        {
            return Return(interval, null);
        }

        public static VariableNode VariableAccess(Interval interval, string name)
        {
            return (VariableNode) Wrap(
                new _.VariableNode(interval, name)
                );
        }

        public static BinaryNode MemberAccess(Interval interval, ExpressionNode target, VariableNode sub)
        {
            return (BinaryNode) Wrap(
                new _.BinaryNode(interval, Extract(target), Extract(sub), ExpressionType.SubfieldAccess)
            );
        }

        public static CallishNode Index(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return (CallishNode) Wrap(
                new _.CallishNode(interval, Extract(target), List(arguments), ExpressionType.Index));

        }

        public static CallishNode Call(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return (CallishNode) Wrap(
                new _.CallishNode(interval, Extract(target), List(arguments), ExpressionType.Invocation));
        }

        public static AssignmentNode Assignment(Interval interval, ExpressionNode target, ExpressionNode value)
        {
            return (AssignmentNode) Wrap(
                new _.AssignmentNode(interval, Extract(target), Extract(target)));
        }

        public static CrawlSyntaxNode CompilationUnit(Interval interval, IEnumerable<ImportNode> importNodes, BlockNode rootCode)
        {
            return (CompiliationUnitNode) Wrap(
                new _.CompiliationUnitNode(interval, List(importNodes), Extract(rootCode)));
        }

        //TODO: Also expose this as individual methods
        public static ExpressionNode MultiExpression(Interval interval, ExpressionType type, IEnumerable<ExpressionNode> sources)
        {
            return (ExpressionNode) Wrap(
                new _.MultiChildExpressionNode(interval, type, List(sources)));
        }

        //TODO: Also expose this as individual methods
        //TODO: Automatically catch invalid ExpressionTypes for this and delegate to relevant target
        public static ExpressionNode BinaryExpression(Interval interval, ExpressionType type, ExpressionNode leftHandSide, ExpressionNode rightHandSide)
        {
            return (ExpressionNode) Wrap(
                new _.BinaryNode(interval, Extract(leftHandSide), Extract(rightHandSide), type)
            );
        }


        private static ExpressionNode MkLiteral(Interval interval, string text, LiteralType type)
            => (ExpressionNode) Wrap(new _.LiteralNode(interval, text, type));
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
            return (VariableNode) Wrap(new _.VariableNode(interval, name));
        }

        public static TokenNode TokenNode(Interval interval, string name)
        {
            return (TokenNode) Wrap(new _.TokenNode(interval, name));
        }

        public static TypeNode Type(Interval interval, CrawlType crawlType)
        {
            return (TypeNode) Wrap(new _.TypeNode(interval, crawlType));
        }
    }
}