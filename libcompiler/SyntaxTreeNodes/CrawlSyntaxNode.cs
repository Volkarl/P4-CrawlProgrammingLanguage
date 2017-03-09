using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using libcompiler.ExtensionMethods;
using libcompiler.Parser;

namespace libcompiler.SyntaxTreeNodes
{
    public enum NodeType
    {
        TODO,
        Forloop,
        If,
        IfElse,
        While,
        Return,
        Assignment,
        Call,
        Index,
        MultiExpression,
        BinaryExpression,
        Variable,
        ClassDecleration,
        VariableDecleration,
        VariableDeclerationSingle,
        FunctionDecleration,
        Block,
        Import,
        CompilationUnit,
        Literal
    }

    public class ExpressionParser
    {
        private readonly NodeFactory NodeFactory;

        public ExpressionParser(NodeFactory nodeFactory)
        {
            NodeFactory = nodeFactory;
        }

        public ExpressionNode ParseExpression(RuleContext rule)
        {
            if (rule.RuleIndex == CrawlParser.RULE_literal)
                return ParseLiteral(rule);

            if (rule.RuleIndex == CrawlParser.RULE_atom)
            {

                ITerminalNode tn = rule.GetChild(0) as ITerminalNode;
                if (tn != null && tn.Symbol.Type == CrawlLexer.IDENTIFIER)
                {
                    return NodeFactory.VariableAccess(tn.SourceInterval, tn.GetText());
                }
                else if (rule.ChildCount == 3)
                {
                    return ParseExpression((RuleContext) rule.GetChild(1));
                }
            }

            if(rule.ChildCount == 1)
                return ParseExpression((RuleContext) rule.GetChild(0));


            switch (rule.RuleIndex)
            {
                case CrawlParser.RULE_postfix_expression:
                    return ParsePostfix(rule);
                case CrawlParser.RULE_comparison_expression:
                    return ParseBinary(rule);
                case CrawlParser.RULE_additive_expression:
                case CrawlParser.RULE_multiplicative_expression:
                case CrawlParser.RULE_and_expression:
                case CrawlParser.RULE_or_expression:
                case CrawlParser.RULE_exponential_expression:
                    return ParseMultu(rule);
                default:
                    throw new NotImplementedException();
                    
            }
        }

        private ExpressionNode ParseMultu(RuleContext rule)
        {
            List<ExpressionNode> sources = new List<ExpressionNode>();
            sources.Add(ParseExpression((RuleContext)rule.GetChild(0)));
            ExpressionType type = ParseMultiOp((ITerminalNode) rule.GetChild(1));

            for (int i = 1; i < rule.ChildCount; i+=2)
            {
                ExpressionType newtype = ParseMultiOp((ITerminalNode) rule.GetChild(i));
                if (newtype == type)
                    sources.Add(ParseExpression((RuleContext) rule.GetChild(i + 1)));
                else
                {
                    //TODO: Actually calculate the interval
                    //The expression contained multiple types of operations
                    ExpressionNode newNode = NodeFactory.MultiExpression(Interval.Invalid, type, sources);
                    sources.Clear();
                    sources.Add(newNode);
                    type = newtype;
                    sources.Add(ParseExpression((RuleContext)rule.GetChild(i + 1)));
                }
            }

            return NodeFactory.MultiExpression(rule.SourceInterval, type, sources);
        }

        private ExpressionNode ParseBinary(RuleContext rule)
        {
            if (rule.ChildCount != 3) throw new NotImplementedException("SHOULD NOT HAPPEN");

            RuleContext lhs = (RuleContext)rule.GetChild(0);
            ITerminalNode op = (ITerminalNode)rule.GetChild(1);
            RuleContext rhs = (RuleContext)rule.GetChild(2);

            ExpressionNode lhsNode = ParseExpression(lhs);
            ExpressionType type = ParseBinaryOp(op);
            ExpressionNode rhsNode = ParseExpression(rhs);
            
            return NodeFactory.BinaryExpression(rule.SourceInterval, type, lhsNode, rhsNode);
        }

        private static readonly Dictionary<string, ExpressionType> BinaryTypeMap = new Dictionary<string, ExpressionType>()
        {
            {">", ExpressionType.Greater },
            {">=", ExpressionType.GreaterEqual },
            {"==", ExpressionType.Equal },
            {"!=", ExpressionType.NotEqual },
            {"<=", ExpressionType.LessEqual },
            {"<", ExpressionType.Less },
        };

        private static ExpressionType ParseBinaryOp(ITerminalNode op)
        {
            ExpressionType et;
            if (BinaryTypeMap.TryGetValue(op.GetText(), out et))
                return et;

            throw new NotImplementedException();
        }

        private static readonly Dictionary<string, ExpressionType> MultiTypeMap = new Dictionary<string, ExpressionType>
        {
            {"-", ExpressionType.Subtract },
            {"+", ExpressionType.Add },
            {"*", ExpressionType.Multiply},
            {"**", ExpressionType.Power }
        };
        

        private static ExpressionType ParseMultiOp(ITerminalNode op)
        {
            string textop = op.GetText();
            ExpressionType et;
            if (MultiTypeMap.TryGetValue(op.GetText(), out et))
                return et;

            throw new NotImplementedException();
        }

        private ExpressionNode ParsePostfix(RuleContext rule)
        {
            ExpressionNode node = ParseExpression((RuleContext) rule.GetChild(0));

            for (int i = 1; i < rule.ChildCount; i++)
            {
                RuleContext post = (RuleContext)rule.GetChild(i);
                if (post.RuleIndex == CrawlParser.RULE_call_expression)
                {
                    node = NodeFactory.Call(post.SourceInterval, node, ParseCallTail(post));
                }
                else if (post.RuleIndex == CrawlParser.RULE_index_expression)
                {
                    node = NodeFactory.Index(post.SourceInterval, node, ParseCallTail(post));
                }
                else if(post.RuleIndex == CrawlParser.RULE_subfield_expression)
                {
                    VariableNode sub = NodeFactory.VariableAccess(post.GetChild(1).SourceInterval, post.GetChild(1).GetText());
                    node = NodeFactory.MemberAccess(post.SourceInterval, node, sub);
                }
                else throw new NotImplementedException();

            }

            return node;
        }

        private ExpressionNode ParseLiteral(RuleContext rule)
        {
            RuleContext realLiteral = (RuleContext) rule.GetChild(0);

            switch (realLiteral.RuleIndex)
            {
                case CrawlParser.RULE_string_literal:
                    return NodeFactory.StringConstant(realLiteral.SourceInterval, realLiteral.GetText());
                case CrawlParser.RULE_integer_literal:
                    return NodeFactory.IntegerConstant(realLiteral.SourceInterval, realLiteral.GetText());
                case CrawlParser.RULE_boolean_literal:
                    return NodeFactory.BooleanConstant(realLiteral.SourceInterval, realLiteral.GetText());
                case CrawlParser.RULE_real_literal:
                    return NodeFactory.RealConstant(realLiteral.SourceInterval, realLiteral.GetText());
                default:
                    throw new NotImplementedException();
            }

        }

        public ExpressionNode ParseSideEffectStatement(RuleContext rule)
        {
            ITerminalNode eos = (ITerminalNode) rule.GetChild(2);
            if(eos.Symbol.Type != CrawlLexer.END_OF_STATEMENT) throw new NotImplementedException();

            RuleContext toCall = (RuleContext) rule.GetChild(0);
            RuleContext invocation = (RuleContext) rule.GetChild(1);

            List<ExpressionNode> args = ParseCallTail(invocation);
            ExpressionNode target = ParseExpression(toCall);

            return NodeFactory.Call(rule.SourceInterval, target, args);
        }

        public List<ExpressionNode> ParseCallTail(RuleContext rule)
        {
            if (rule.ChildCount == 2)
            {
                return new List<ExpressionNode>();
            }
            else if(rule.ChildCount == 3)
            {
                RuleContext expList = (RuleContext) rule.GetChild(1);
                return ParseExpressionList(expList);
            }

            throw new NotImplementedException();
        }

        private List<ExpressionNode> ParseExpressionList(RuleContext expList)
        {
            List<ExpressionNode> n = new List<ExpressionNode>(expList.ChildCount / 2);
            for (int i = 0; i < expList.ChildCount; i += 2)
            {
                n.Add(ParseExpression((RuleContext) expList.GetChild(i)));

                if (i + 1 != expList.ChildCount)
                {
                    ITerminalNode itemsep = (ITerminalNode) expList.GetChild(i);
                    if(itemsep.Symbol.Type != CrawlLexer.ITEM_SEPARATOR) throw new NotImplementedException();
                }
            }

            return n;
        }
    }

    public class Foo
    {
        private readonly NodeFactory NodeFactory;
        private readonly ExpressionParser ExpressionParser;

        public Foo(CrawlSyntaxTree tree)
        {
            NodeFactory = new NodeFactory(tree);
            ExpressionParser = new ExpressionParser(NodeFactory);
        }

        public FlowNode ParseFlow(RuleContext rule)
        {
            if (rule.RuleIndex == CrawlParser.RULE_for_loop)
            {
                return ParseFor(rule);
            }
            else if (rule.RuleIndex == CrawlParser.RULE_if_selection)
            {
                return ParseIf(rule);
            }
            throw new NotImplementedException();
        }

        private FlowNode ParseIf(RuleContext rule)
        {
            //IF expression INDENT statements DEDENT (ELSE INDENT statements DEDENT)?;
            ExpressionNode expression = ExpressionParser.ParseExpression((RuleContext)rule.GetChild(1));
            BlockNode trueBlock = ParseBlockNode((RuleContext) rule.GetChild(3));

            if (rule.ChildCount == 5)
            {
                return NodeFactory.If(rule.SourceInterval, expression, trueBlock);
            }
            else if(rule.ChildCount == 9)
            {
                BlockNode falseBlock = ParseBlockNode((RuleContext) rule.GetChild(7));
                return NodeFactory.IfElse(rule.SourceInterval, expression, trueBlock, falseBlock);
            }

            throw new NotImplementedException();
        }

        private FlowNode ParseFor(RuleContext rule)
        {
            //FOR type IDENTIFIER FOR_LOOP_SEPERATOR expression INDENT statements DEDENT
            ITerminalNode forNode = (ITerminalNode)rule.GetChild(0);
            CrawlParser.TypeContext typeNode = (CrawlParser.TypeContext) rule.GetChild(1);
            ITerminalNode identifierNode = (ITerminalNode)rule.GetChild(2);
            ITerminalNode serperatorNode = (ITerminalNode)rule.GetChild(3);
            RuleContext expression = (RuleContext)rule.GetChild(4);
            ITerminalNode indent = (ITerminalNode)rule.GetChild(5);
            RuleContext blockCtx = (RuleContext)rule.GetChild(6);
            ITerminalNode dedent = (ITerminalNode)rule.GetChild(7);

            CrawlType type = ParseType(typeNode);
            ExpressionNode iteratior = ExpressionParser.ParseExpression(expression);
            BlockNode block = ParseBlockNode(blockCtx);

            return NodeFactory.Forloop(rule.SourceInterval, type, identifierNode.GetText(), iteratior, block);
        }

        public DeclerationNode ParseDeclerationNode(RuleContext rule)
        {
            ProtectionLevel protectionLevel = ProtectionLevel.None;
            RuleContext declpart;
            if (rule.GetChild(0) is CrawlParser.Protection_levelContext)
            {
                protectionLevel = ParseProtectionLevel((CrawlParser.Protection_levelContext) rule.GetChild(0));

                declpart = (RuleContext)rule.GetChild(1);
            }
            else
            {
                declpart = (RuleContext)rule.GetChild(0);
            }


            if (declpart.RuleIndex == CrawlParser.RULE_class_declaration)
            {
                return ParseClassDecleration(declpart, protectionLevel, rule.SourceInterval);
            }
            if (declpart.RuleIndex == CrawlParser.RULE_function_decleration)
            {
                return ParseFunctionDecleration(declpart, protectionLevel, rule.SourceInterval);
            }
            else if (declpart.RuleIndex == CrawlParser.RULE_variable_declerations)
            {
                return ParseVariableDecleration(declpart, protectionLevel, rule.SourceInterval);
            }
            
            throw new NotImplementedException();
        }

        #region DeclerationSubs


        private DeclerationNode ParseFunctionDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            CrawlType type = ParseType((CrawlParser.TypeContext) classPart.GetChild(0));
            ITerminalNode identifier = (ITerminalNode) classPart.GetChild(1);
            ITerminalNode assignment = (ITerminalNode) classPart.GetChild(2);
            RuleContext body = (RuleContext) classPart.GetChild(3).GetChild(1);

            if (identifier.Symbol.Type != CrawlLexer.IDENTIFIER) throw new NotImplementedException();
            if (assignment.Symbol.Type != CrawlLexer.ASSIGNMENT_SYMBOL) throw new NotImplementedException();

            return NodeFactory.Function(interval, protectionLevel, type, identifier.GetText(), ParseBlockNode(body));
        }

        private DeclerationNode ParseVariableDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            ITerminalNode eos =  classPart.LastChild() as ITerminalNode;
            if(eos == null || eos.Symbol.Type != CrawlLexer.END_OF_STATEMENT) throw new NotImplementedException("Something strange happened");

            CrawlType type = ParseType((CrawlParser.TypeContext) classPart.GetChild(0));

            return NodeFactory.VariableDecleration(
                interval,
                protectionLevel,
                type,
                classPart
                    .AsEdgeTrimmedIEnumerable()
                    .OfType<CrawlParser.Variable_declContext>()
                    .Select(ParseSingleVariable));
        }

        private SingleVariableDecleration ParseSingleVariable(CrawlParser.Variable_declContext variable)
        {
            ITerminalNode identifier = (ITerminalNode) variable.GetChild(0);
            if(identifier.Symbol.Type != CrawlLexer.IDENTIFIER) throw new NotImplementedException();

            if (variable.ChildCount == 1)
            {
                return NodeFactory.SingleVariable(variable.SourceInterval, identifier.GetText());
            }
            else if (variable.ChildCount == 3)
            {
                return NodeFactory.SingleVariable(variable.SourceInterval, identifier.GetText(),
                    ExpressionParser.ParseExpression((RuleContext) variable.GetChild(2)));
            }

            throw new NotImplementedException();
        }


        private DeclerationNode ParseClassDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            ITerminalNode tn1 = (ITerminalNode)classPart.GetChild(0);
            ITerminalNode tn2 = (ITerminalNode)classPart.GetChild(1);
            RuleContext body = (RuleContext) classPart.GetChild(2);

            if(tn1.Symbol.Type != CrawlLexer.CLASS) throw new NotImplementedException(); //TODO: WTFSYNTAXEXCEPTION with INTEVAL

            BlockNode bodyBlock = ParseBlockNode(body);
            string name = tn2.GetText();

            return NodeFactory.ClassDecleration(interval, protectionLevel, name, bodyBlock);
        }

        #endregion

        private CrawlType ParseType(CrawlParser.TypeContext type)
        {
            return new CrawlType(type.GetText());
        }

        private static ProtectionLevel ParseProtectionLevel(CrawlParser.Protection_levelContext protectionLevel)
        {
            TerminalNodeImpl tnode = (TerminalNodeImpl) protectionLevel.GetChild(0);

            switch (tnode.Payload.Type)
            {
                case CrawlLexer.PUBLIC:
                    return ProtectionLevel.Public;
                case CrawlLexer.PRIVATE:
                    return ProtectionLevel.Private;
                case CrawlLexer.PROTECTED:
                    return ProtectionLevel.Protected;
                case CrawlLexer.INTERNAL:
                    return ProtectionLevel.Internal;
                case CrawlLexer.PROTECTED_INTERNAL:
                    return ProtectionLevel.ProtectedInternal;
                default:
                    throw new NotImplementedException("This should never happen");
            }   
        }

        public BlockNode ParseBlockNode(RuleContext rule)
        {
            System.Collections.IEnumerable meaningfullContent;

            if (rule.RuleIndex == CrawlParser.RULE_statements)
            {
                meaningfullContent = rule.AsIEnumerable();
            }
            else if (rule.RuleIndex == CrawlParser.RULE_class_body)
            {
                meaningfullContent = rule.AsEdgeTrimmedIEnumerable();
            }
            else throw new NotImplementedException("Probably broken");

            IEnumerable<CrawlSyntaxNode> contents =
                meaningfullContent
                    .Cast<RuleContext>()
                    .Select(ParseStatement);

            return NodeFactory.Block(rule.SourceInterval, contents);
        }

        public ImportNode ParseImportNode(RuleContext rule)
        {
            throw new NotImplementedException();
        }

        public CrawlSyntaxNode ParseStatement(RuleContext rule)
        {
            switch (rule.RuleIndex)
            {
                case CrawlParser.RULE_declaration:
                    return ParseDeclerationNode(rule);
                case CrawlParser.RULE_side_effect_stmt:
                    return ExpressionParser.ParseSideEffectStatement(rule);
                case CrawlParser.RULE_for_loop:
                case CrawlParser.RULE_if_selection:
                    return ParseFlow(rule);
                case CrawlParser.RULE_assignment:
                    return ParseAssignment(rule);
                case CrawlParser.RULE_return_statement:
                    return ParseReturn(rule);
                default:
                    throw new NotImplementedException();
            }
        }

        private CrawlSyntaxNode ParseReturn(RuleContext rule)
        {
            
            if (rule.ChildCount == 3)
            {
                ExpressionNode retvalue = ExpressionParser.ParseExpression((RuleContext) rule.GetChild(1));
                return NodeFactory.Return(rule.SourceInterval, retvalue);
            }
            else
            {
                return NodeFactory.Return(rule.SourceInterval);
            }
        }

        private CrawlSyntaxNode ParseAssignment(RuleContext rule)
        {

            ExpressionNode target = ExpressionParser.ParseExpression((RuleContext)rule.GetChild(0));
            if(rule.GetChild(1) is CrawlParser.Subfield_expressionContext)
            {
                CrawlParser.Subfield_expressionContext subfield = (CrawlParser.Subfield_expressionContext) rule.GetChild(1);

                VariableNode sub = NodeFactory.VariableAccess(subfield.GetChild(1).SourceInterval, subfield.GetChild(1).GetText());
                target = NodeFactory.MemberAccess(subfield.SourceInterval, target, sub);
            }
            else if(rule.GetChild(1) is CrawlParser.Index_expressionContext)
            {
                RuleContext idx = (RuleContext) rule.GetChild(1);
                target = NodeFactory.Index(idx.SourceInterval, target, ExpressionParser.ParseCallTail(idx));
            }

            ExpressionNode value = ExpressionParser.ParseExpression((RuleContext) rule.GetChild(rule.ChildCount - 2));
            return NodeFactory.Assignment(rule.SourceInterval, target, value);
        }
    }

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
            return new TodoRenameCall(_owner, interval, target, arguments, ExpressionType.Index);
        }

        public ExpressionNode Call(Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments)
        {
            return new TodoRenameCall(_owner, interval, target, arguments, ExpressionType.Invocation);
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

    public enum ProtectionLevel
    {
        None,
        Public,
        Internal,
        Protected,
        ProtectedInternal,
        Private,
    }

    public class CrawlType
    {
        public string Textdef { get; }

        public CrawlType(string textdef)
        {
            Textdef = textdef;
        }
    }

    public abstract class CrawlSyntaxNode
    {
        public CrawlSyntaxTree OwningTree { get; }
        public NodeType Type { get; }
        public Interval CodeInterval { get; }

        protected CrawlSyntaxNode(CrawlSyntaxTree owningTree, NodeType type, Interval codeInterval)
        {
            OwningTree = owningTree;
            Type = type;
            CodeInterval = codeInterval;
        }
    }

    public abstract class FlowNode : CrawlSyntaxNode
    {
        protected FlowNode(CrawlSyntaxTree owningTree, NodeType type, Interval interval) : base(owningTree, type, interval)
        {
        }
    }

    public class ForLoopNode : FlowNode
    {
        public CrawlType InducedFieldType { get; }
        public string InducedFieldName { get; }
        public ExpressionNode Iteratior { get; }
        public BlockNode Block { get; }

        public ForLoopNode(
            CrawlType type, 
            string inducedField, 
            ExpressionNode iteratior, 
            BlockNode block, 
            Interval interval, 
            CrawlSyntaxTree owningTree) 
        : base(
            owningTree, 
            NodeType.Forloop, 
            interval)
        {
            InducedFieldType = type;
            InducedFieldName = inducedField;
            Iteratior = iteratior;
            Block = block;
        }
    }

    public class SelectiveFlowNode : FlowNode
    {
        public ExpressionNode Check { get; }
        public BlockNode Primary { get; }
        public BlockNode Alternative { get; }

        public SelectiveFlowNode(FlowType type, ExpressionNode check, BlockNode primary, BlockNode alternative, Interval interval, CrawlSyntaxTree owningTree) : base(owningTree, MakeNodeType(type), interval)
        {
            Check = check;
            Primary = primary;
            Alternative = alternative;
        }

        private static NodeType MakeNodeType(FlowType type)
        {
            switch (type)
            {
                case FlowType.If:
                    return NodeType.If;
                case FlowType.IfElse:
                    return NodeType.IfElse;
                case FlowType.While:
                    return NodeType.While;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public enum FlowType
        {
            If,
            IfElse,
            While,
        }
    }

    public class ReturnStatement : CrawlSyntaxNode
    {
        public ExpressionNode ReturnValue { get; }
        public ReturnStatement(CrawlSyntaxTree owningTree, Interval interval, ExpressionNode returnValue = null) : base(owningTree, NodeType.Return, interval)
        {
            ReturnValue = returnValue;
        }
    }

    public class AssignmentNode : CrawlSyntaxNode
    {
        public ExpressionNode LeftHandSide { get; }
        public ExpressionNode RightHandSide { get; }

        public AssignmentNode(CrawlSyntaxTree owningTree, Interval interval, ExpressionNode leftHandSide, ExpressionNode rightHandSide) : base(owningTree, NodeType.Assignment, interval)
        {
            LeftHandSide = leftHandSide;
            RightHandSide = rightHandSide;
        }
    }

    public abstract class ExpressionNode : CrawlSyntaxNode
    {
        protected ExpressionNode(CrawlSyntaxTree owningTree, Interval interval, NodeType type) : base(owningTree, type, interval) { }
    }

    public class LiteralNode : ExpressionNode
    {
        public string Value { get; }
        public LiteralType Type { get; }

        public LiteralNode(CrawlSyntaxTree owningTree, Interval interval, string value, LiteralType type) : base(owningTree, interval, NodeType.Literal)
        {
            Value = value;
            Type = type;
        }

        public enum LiteralType
        {
            String,
            Int,
            Float,
            Boolean,
            Real
        }

        public override string ToString()
        {
            return Value;
        }
    }

    public class TodoRenameCall : ExpressionNode
    {
        public ExpressionNode Target { get; }
        public IReadOnlyCollection<ExpressionNode> Arguments { get; }

        public TodoRenameCall(CrawlSyntaxTree owningTree, Interval interval, ExpressionNode target, IEnumerable<ExpressionNode> arguments, ExpressionType type) : base(owningTree, interval, MakeNodeType(type))
        {
            Target = target;
            Arguments = arguments.ToList().AsReadOnly();
        }

        private static NodeType MakeNodeType(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Index:
                    return NodeType.Index;
                case ExpressionType.Invocation:
                    return NodeType.Call;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public class MultiChildExpressionNode : ExpressionNode
    {
        public ExpressionType ExpressionType { get; }
        public IReadOnlyCollection<ExpressionNode> Arguments { get; }
        public MultiChildExpressionNode(CrawlSyntaxTree owningTree, Interval interval, ExpressionType type, IEnumerable<ExpressionNode> children): base(owningTree, interval, NodeType.MultiExpression)
        {
            ExpressionType = type;
            Arguments = children.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }
    }

    public class BinaryNode : ExpressionNode
    {
        public ExpressionNode LeftHandSide { get; }
        public ExpressionNode RightHandSide { get; }
        public ExpressionType ExpressionType { get; }

        public BinaryNode(CrawlSyntaxTree owningTree, Interval interval, ExpressionType type, ExpressionNode lhs, ExpressionNode rhs) : base(owningTree, interval, NodeType.BinaryExpression)
        {
            ExpressionType = type;
            LeftHandSide = lhs;
            RightHandSide = rhs;
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }
    }

    public class VariableNode : ExpressionNode
    {
        public string Name { get; }

        public VariableNode(CrawlSyntaxTree owningTree, string name, Interval interval) : base(owningTree, interval, NodeType.Variable)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public enum ExpressionType
    {
        Invocation,
        SubfieldAccess,
        Greater,
        GreaterEqual,
        Equal,
        NotEqual,
        LessEqual,
        Less,
        Index,
        Subtract,
        Power,
        Add,
        Multiply
    }

    #region Declerations

    public abstract class DeclerationNode : CrawlSyntaxNode
    {
        public ProtectionLevel ProtectionLevel { get; }

        protected DeclerationNode(CrawlSyntaxTree owningTree, Interval interval, NodeType type,
            ProtectionLevel protectionLevel) : base(owningTree, type, interval)
        {
            ProtectionLevel = protectionLevel;
        }

    }

    public class ClassDeclerationNode : DeclerationNode
    {
        //TODO: List of constructors? Probably as extension method to not calculate unless required
        public BlockNode BodyBlock { get; }
        public string Identifier { get; }

        public ClassDeclerationNode(CrawlSyntaxTree owningTree, ProtectionLevel protectionLevel, string name, BlockNode bodyBlock, Interval interval) : base(owningTree, interval, NodeType.ClassDecleration, protectionLevel)
        {
            Identifier = name;
            BodyBlock = bodyBlock;
        }
    }

    public class VariableDeclerationNode : DeclerationNode
    {
        public CrawlType DeclerationType { get; }
        public List<SingleVariableDecleration> Declerations { get; }

        public VariableDeclerationNode(
            CrawlSyntaxTree owningTree, 
            ProtectionLevel protectionLevel, 
            CrawlType declerationType, 
            IEnumerable<SingleVariableDecleration> declerations, 
            Interval interval
        ) : base(owningTree, interval, NodeType.VariableDecleration, protectionLevel)
        {
            DeclerationType = declerationType;
            Declerations = declerations.ToList();
        }
    }

    public class SingleVariableDecleration : CrawlSyntaxNode
    {
        public string Identifier { get; }
        public Interval Interval { get; }
        public ExpressionNode DefaultValue { get; }
         
        public SingleVariableDecleration(CrawlSyntaxTree owningTree, string name, Interval interval, ExpressionNode defaultValue = null) : base(owningTree, NodeType.VariableDeclerationSingle, interval)
        {
            Identifier = name;
            Interval = interval;
            DefaultValue = defaultValue;
        }
    }

    public class FunctionDeclerationNode : DeclerationNode
    {
        //TODO: do something about parameters
        public CrawlType FunctionType { get; }
        public string Identfier { get; }
        public BlockNode BodyBlock { get; }

        public FunctionDeclerationNode(CrawlSyntaxTree owningTree, CrawlType functionType, string name, Interval interval, BlockNode block, ProtectionLevel protectionLevel) : base(owningTree, interval, NodeType.FunctionDecleration, protectionLevel)
        {
            FunctionType = functionType;
            Identfier = name;
            BodyBlock = block;
        }
    }

    #endregion

    public class BlockNode : CrawlSyntaxNode
    {
        //TODO: Probably some kind of (generated) Scope information here
        public IReadOnlyCollection<CrawlSyntaxNode> Children { get; }

        public BlockNode(CrawlSyntaxTree owningTree, Interval interval, IEnumerable<CrawlSyntaxNode> children)
            : base(owningTree, NodeType.Block, interval)
        {
            Children = children.ToList().AsReadOnly();
        }
    }

    public  class ImportNode : CrawlSyntaxNode
    {
        public string Module { get; }

        public ImportNode(CrawlSyntaxTree owningTree, Interval interval, string module) : base(owningTree, NodeType.Import, interval)
        {
            Module = module;
        }
    }

    public class CompiliationUnitNode : CrawlSyntaxNode
    {
        //This should plausibly be 2 lists. 1 of All declarations (functions/classes/namespaces) and 1 of statements;
        //And maybe even a third, imports;
        public CompiliationUnitNode(CrawlSyntaxTree owningTree, Interval interval, BlockNode codeChildren, IEnumerable<ImportNode> imports ) : base(owningTree, NodeType.CompilationUnit, interval)
        {
            Code = codeChildren;
            Imports = imports.ToList().AsReadOnly();
        }

        public IReadOnlyCollection<ImportNode> Imports;
        public BlockNode Code { get; } 
    }
}