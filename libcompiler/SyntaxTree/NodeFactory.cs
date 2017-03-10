using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes;
using _ = libcompiler.SyntaxTree.Nodes.Internal;

namespace libcompiler.SyntaxTree
{
    public class NodeFactory
    {
        private readonly CrawlSyntaxTree _owner;

        public NodeFactory(CrawlSyntaxTree owner)
        {
            _owner = owner;
        }

        private static CrawlSyntaxNode Wrap(_.GreenNode selectiveFlowNode)
        {
            return CrawlSyntaxTree.FromGreen(selectiveFlowNode, "<Unknown>").RootNode;
        }

        private _.BlockNode Extract(BlockNode n)
        {
            return (_.BlockNode) CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private _.TypeNode Extract(TypeNode n)
        {
            return (_.TypeNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private _.VariableNode Extract(VariableNode n)
        {
            return (_.VariableNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private _.ExpressionNode Extract(ExpressionNode n)
        {
            return (_.ExpressionNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private _.TokenNode Extract(TokenNode n)
        {
            return (_.TokenNode)CrawlSyntaxNode.ExtractGreenNode(n);
        }

        private _.ListNode<T> List<T>(IEnumerable<T> i) where T : CrawlSyntaxNode
        {
            //FIXME INTERVAL
            return new _.ListNode<T>(default(Interval), i.Select(CrawlSyntaxNode.ExtractGreenNode));
        }

        public FlowNode If(Interval interval, ExpressionNode conditon, BlockNode trueBlock)
        {
            return (FlowNode) Wrap(
                new _.SelectiveFlowNode(
                    interval, 
                    _.SelectiveFlowNode.FlowType.If,
                    (_.ExpressionNode) CrawlSyntaxNode.ExtractGreenNode(conditon),
                    (_.BlockNode) CrawlSyntaxNode.ExtractGreenNode(trueBlock),
                    null));
        }

        public FlowNode IfElse(Interval interval, ExpressionNode conditon, BlockNode trueBlock, BlockNode falseBlock)
        {
            return (FlowNode) Wrap(
                new _.SelectiveFlowNode(
                    interval, 
                    _.SelectiveFlowNode.FlowType.IfElse, 
                    Extract(conditon), 
                    Extract(trueBlock), 
                    Extract(falseBlock)));
                

        }

        

        public FlowNode Forloop(Interval interval, TypeNode inducedVariableType, VariableNode inducedVariableName, ExpressionNode iteratior, BlockNode block)
        {
            return (FlowNode) Wrap(
                new _.ForLoopNode(
                    interval,
                    Extract(inducedVariableType),
                    Extract(inducedVariableName), 
                    Extract(iteratior),
                    Extract(block)));
            
        }

        public FlowNode WhileLoop(Interval interval, ExpressionNode condition, BlockNode block)
        {
            return (FlowNode) Wrap(new _.SelectiveFlowNode(interval, _.SelectiveFlowNode.FlowType.While, Extract(condition), Extract(block), null));
        }

        public DeclerationNode Function(Interval interval, ProtectionLevel protectionLevel, TypeNode functionType, VariableNode identifier, BlockNode block)
        {
            return (DeclerationNode) Wrap(
                new _.FunctionDeclerationNode(
                    interval,
                    protectionLevel,
                    Extract(functionType),
                    Extract(identifier),
                    Extract(block)
                ));
        }

        public SingleVariableDecleration SingleVariable(Interval interval, VariableNode identifier)
        {
            return SingleVariable(interval, identifier, null);
        }

        public SingleVariableDecleration SingleVariable(Interval interval, VariableNode identifier, ExpressionNode value)
        {
            return (SingleVariableDecleration) Wrap(
                new _.SingleVariableDecleration(
                    interval,
                    Extract(identifier),
                    Extract(value))
            );
        }

        public DeclerationNode VariableDecleration(Interval interval, ProtectionLevel protectionLevel, TypeNode type, IEnumerable<SingleVariableDecleration> declerations)
        {
            return (DeclerationNode) Wrap(
                new _.VariableDeclerationNode(
                    interval,
                    protectionLevel,
                    Extract(type),
                    List(declerations))
            );
        }

        public DeclerationNode ClassDecleration(Interval interval, ProtectionLevel protectionLevel, TokenNode identifier, BlockNode bodyBlock)
        {
            return (DeclerationNode) Wrap(
                new _.ClassDeclerationNode(interval, protectionLevel, Extract(identifier), Extract(bodyBlock))
            );
        }

        public BlockNode Block(Interval interval, IEnumerable<CrawlSyntaxNode> contents)
        {
            return (BlockNode) Wrap(
                new _.BlockNode(interval, contents.Select(CrawlSyntaxNode.ExtractGreenNode)));
        }

        public CrawlSyntaxNode Return(Interval interval, ExpressionNode returnValue)
        {
            return Wrap(
                new _.ReturnStatement(interval, Extract(returnValue)));
        }

        public CrawlSyntaxNode Return(Interval interval)
        {
            return Return(interval, null);
        }

        public VariableNode VariableAccess(Interval interval, string name)
        {
            return (VariableNode) Wrap(
                new _.VariableNode(interval, name)
                );
        }

        public ExpressionNode MemberAccess(Interval interval, ExpressionNode target, VariableNode sub)
        {
            return (ExpressionNode) Wrap(
                new _.BinaryNode(interval, Extract(target), Extract(sub), ExpressionType.SubfieldAccess)
            );
        }

        public ExpressionNode Index(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return (ExpressionNode) Wrap(
                new _.CallishNode(interval, Extract(target), List(arguments), ExpressionType.Index));

        }

        public ExpressionNode Call(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return (ExpressionNode) Wrap(
                new _.CallishNode(interval, Extract(target), List(arguments), ExpressionType.Invocation));
        }

        public CrawlSyntaxNode Assignment(Interval interval, ExpressionNode target, ExpressionNode value)
        {
            return Wrap(
                new _.AssignmentNode(interval, Extract(target), Extract(target)));
        }

        public CrawlSyntaxNode CompilationUnit(Interval interval, IEnumerable<ImportNode> importNodes, BlockNode rootCode)
        {
            return (CompiliationUnitNode) Wrap(
                new _.CompiliationUnitNode(interval, List(importNodes), Extract(rootCode)));
        }

        //TODO: Also expose this as individual methods
        public ExpressionNode MultiExpression(Interval interval, ExpressionType type, IEnumerable<ExpressionNode> sources)
        {
            return (ExpressionNode) Wrap(
                new _.MultiChildExpressionNode(interval, type, List(sources)));
        }

        //TODO: Also expose this as individual methods
        //TODO: Automatically catch invalid ExpressionTypes for this and delegate to relevant target
        public ExpressionNode BinaryExpression(Interval interval, ExpressionType type, ExpressionNode leftHandSide, ExpressionNode rightHandSide)
        {
            return (ExpressionNode) Wrap(
                new _.BinaryNode(interval, Extract(leftHandSide), Extract(rightHandSide), type)
            );
        }


        private static ExpressionNode MkLiteral(Interval interval, string text, LiteralType type)
            => (ExpressionNode) Wrap(new _.LiteralNode(interval, text, type));
        public ExpressionNode StringConstant(Interval interval, string textRepresentation)
        {
            return MkLiteral(interval, textRepresentation, LiteralType.String);
        }

        public ExpressionNode IntegerConstant(Interval interval, string textRepresentation)
        {
            return MkLiteral(interval, textRepresentation, LiteralType.Int);
        }

        public ExpressionNode BooleanConstant(Interval interval, string textRepresentation)
        {
            return MkLiteral(interval, textRepresentation, LiteralType.Boolean);
        }

        public ExpressionNode RealConstant(Interval interval, string textRepresentation)
        {
            return MkLiteral(interval, textRepresentation, LiteralType.Real);
        }

        public VariableNode VariableNode(Interval interval, string name)
        {
            return (VariableNode) Wrap(new _.VariableNode(interval, name));
        }

        public TokenNode TokenNode(Interval interval, string name)
        {
            return (TokenNode) Wrap(new _.TokenNode(interval, name));
        }

        public TypeNode Type(Interval interval, CrawlType crawlType)
        {
            return (TypeNode) Wrap(new _.TypeNode(interval, crawlType));
        }
    }
}