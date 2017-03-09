using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree.Nodes;

namespace libcompiler.SyntaxTree
{
    public class NodeFactory
    {
        private readonly CrawlSyntaxTree _owner;

        public NodeFactory(CrawlSyntaxTree owner)
        {
            _owner = owner;
        }
        
        public FlowNode If(Interval interval, ExpressionNode conditon, BlockNode trueBlock)
        {
            return new SelectiveFlowNode(SelectiveFlowNode.FlowType.If, conditon, trueBlock, null, interval, _owner);
        }

        public FlowNode IfElse(Interval interval, ExpressionNode conditon, BlockNode trueBlock, BlockNode falseBlock)
        {
            return new SelectiveFlowNode(SelectiveFlowNode.FlowType.IfElse, conditon, trueBlock, falseBlock, interval, _owner);
        }

        public FlowNode Forloop(Interval interval, CrawlType inducedVariableTyoe, string inducedVariableName, ExpressionNode iteratior, BlockNode block)
        {
            return new ForLoopNode(inducedVariableTyoe, inducedVariableName, iteratior, block, interval, _owner);
        }

        public DeclerationNode Function(Interval interval, ProtectionLevel protectionLevel, CrawlType functionType, string identifier, BlockNode block)
        {
            return new FunctionDeclerationNode(_owner, functionType, identifier, interval, block, protectionLevel);
        }

        public SingleVariableDecleration SingleVariable(Interval interval, string name)
        {
            return new SingleVariableDecleration(_owner, name,interval, null);
        }

        public SingleVariableDecleration SingleVariable(Interval interval, string name, ExpressionNode value)
        {
            return new SingleVariableDecleration(_owner, name, interval, value);
        }

        public DeclerationNode VariableDecleration(Interval interval, ProtectionLevel protectionLevel, CrawlType type, IEnumerable<SingleVariableDecleration> declerations)
        {
            return new VariableDeclerationNode(_owner, protectionLevel, type, declerations, interval);
        }

        public DeclerationNode ClassDecleration(Interval interval, ProtectionLevel protectionLevel, string name, BlockNode bodyBlock)
        {
            return new ClassDeclerationNode(_owner, protectionLevel, name, bodyBlock, interval);
        }

        public BlockNode Block(Interval interval, IEnumerable<CrawlSyntaxNode> contents)
        {
            return new BlockNode(_owner, interval, contents);
        }

        public CrawlSyntaxNode Return(Interval interval, ExpressionNode returnValue)
        {
            return  new ReturnStatement(_owner, interval, returnValue);
        }

        public CrawlSyntaxNode Return(Interval interval)
        {
            return new ReturnStatement(_owner, interval, null);
        }

        public VariableNode VariableAccess(Interval interval, string name)
        {
            return new VariableNode(_owner, name, interval);
        }

        public ExpressionNode MemberAccess(Interval interval, ExpressionNode target, VariableNode sub)
        {
            return new BinaryNode(_owner, interval, ExpressionType.SubfieldAccess, target, sub);
        }

        public ExpressionNode Index(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return new CallishNode(_owner, interval, target, arguments, ExpressionType.Index);
        }

        public ExpressionNode Call(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return new CallishNode(_owner, interval, target, arguments, ExpressionType.Invocation);
        }

        public CrawlSyntaxNode Assignment(Interval interval, ExpressionNode target, ExpressionNode value)
        {
            //This could be an BinaryNode, with one exception.
            //ExpressionNodes has a value, assignment is void type
            return new AssignmentNode(_owner, interval, target, value);
        }

        public CrawlSyntaxNode CompilationUnit(Interval interval, IEnumerable<ImportNode> importNodes, BlockNode rootCode)
        {
            return new CompiliationUnitNode(_owner, interval, rootCode, importNodes);
        }

        //TODO: Also expose this as individual methods
        public ExpressionNode MultiExpression(Interval interval, ExpressionType type, IEnumerable<ExpressionNode> sources)
        {
            return new MultiChildExpressionNode(_owner, interval, type, sources);
        }

        //TODO: Also expose this as individual methods
        //TODO: Automatically catch invalid ExpressionTypes for this and delegate to relevant target
        public ExpressionNode BinaryExpression(Interval interval, ExpressionType type, ExpressionNode leftHandSide, ExpressionNode rightHandSide)
        {
            return new BinaryNode(_owner, interval, type, leftHandSide, rightHandSide);
        }

        public ExpressionNode StringConstant(Interval interval, string textRepresentation)
        {
            return new LiteralNode(_owner, interval, textRepresentation, LiteralNode.LiteralType.String);
        }

        public ExpressionNode IntegerConstant(Interval interval, string textRepresentation)
        {
            return new LiteralNode(_owner, interval, textRepresentation, LiteralNode.LiteralType.Int);
        }

        public ExpressionNode BooleanConstant(Interval interval, string textRepresentation)
        {
            return new LiteralNode(_owner, interval, textRepresentation, LiteralNode.LiteralType.Boolean);
        }

        public ExpressionNode RealConstant(Interval interval, string textRepresentation)
        {
            return new LiteralNode(_owner, interval, textRepresentation, LiteralNode.LiteralType.Real);
        }
    }
}