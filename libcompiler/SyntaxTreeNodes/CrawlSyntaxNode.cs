using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using libcompiler.ExtensionMethods;
using libcompiler.Parser;

namespace libcompiler
{
    public enum NodeType
    {
        TODO
    }

    public class ExpressionParser
    {
        public static ExpressionNode ParseExpression(RuleContext rule)
        {
            if (rule.RuleIndex == CrawlParser.RULE_literal)
                return ParseLiteral(rule);

            if (rule.RuleIndex == CrawlParser.RULE_atom)
            {

                ITerminalNode tn = rule.GetChild(0) as ITerminalNode;
                if (tn != null && tn.Symbol.Type == CrawlLexer.IDENTIFIER)
                {
                    return new VariableNode(tn.GetText(), tn.SourceInterval);
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

        private static ExpressionNode ParseMultu(RuleContext rule)
        {
            List<ExpressionNode> sources = new List<ExpressionNode>();
            sources.Add(ParseExpression((RuleContext)rule.GetChild(0)));
            ExpressionType type = ParseMultiOp((ITerminalNode) rule.GetChild(1));

            for (int i = 1; i < rule.ChildCount; i+=2)
            {
                ExpressionType newtype = ParseMultiOp((ITerminalNode) rule.GetChild(i));
                if(newtype == type)
                    sources.Add(ParseExpression((RuleContext)rule.GetChild(i+1)));
                else throw new NotImplementedException();
            }
           
            return new MultiChildExpressionNode(type, sources);
        }

        private static ExpressionNode ParseBinary(RuleContext rule)
        {
            if (rule.ChildCount != 3) throw new NotImplementedException("SHOULD NOT HAPPEN");

            RuleContext lhs = (RuleContext)rule.GetChild(0);
            ITerminalNode op = (ITerminalNode)rule.GetChild(1);
            RuleContext rhs = (RuleContext)rule.GetChild(2);

            ExpressionNode lhsNode = ParseExpression(lhs);
            ExpressionType type = ParseBinaryOp(op);
            ExpressionNode rhsNode = ParseExpression(rhs);
            
            return new BinaryNode(type, lhsNode, rhsNode);
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

        private static ExpressionNode ParsePostfix(RuleContext rule)
        {
            ExpressionNode node = ParseExpression((RuleContext) rule.GetChild(0));

            for (int i = 1; i < rule.ChildCount; i++)
            {
                RuleContext post = (RuleContext)rule.GetChild(i);
                if (post.RuleIndex == CrawlParser.RULE_call_expression)
                {
                    node = new TodoRenameCall(node, ParseCallTail(post), ExpressionType.Invocation);
                }
                else if (post.RuleIndex == CrawlParser.RULE_index_expression)
                {
                    node = new TodoRenameCall(node, ParseCallTail(post), ExpressionType.Index);
                }
                else if(post.RuleIndex == CrawlParser.RULE_subfield_expression)
                {

                    VariableNode sub = new VariableNode(post.GetChild(1).GetText(), post.GetChild(1).SourceInterval);
                    node = new BinaryNode(ExpressionType.SubfieldAccess, node, sub);
                }
                else throw new NotImplementedException();

            }

            return node;
        }

        private static ExpressionNode ParseLiteral(RuleContext rule)
        {
            RuleContext realLiteral = (RuleContext) rule.GetChild(0);

            LiteralNode.LiteralType type;
            switch (realLiteral.RuleIndex)
            {
                case CrawlParser.RULE_string_literal:
                    type = LiteralNode.LiteralType.String;
                    break;
                case CrawlParser.RULE_integer_literal:
                    type = LiteralNode.LiteralType.Int;
                    break;
                case CrawlParser.RULE_boolean_literal:
                    type = LiteralNode.LiteralType.Boolean;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new LiteralNode(realLiteral.GetText(), type);

        }

        public static ExpressionNode ParseSideEffectStatement(RuleContext rule)
        {
            ITerminalNode eos = (ITerminalNode) rule.GetChild(2);
            if(eos.Symbol.Type != CrawlLexer.END_OF_STATEMENT) throw new NotImplementedException();

            RuleContext toCall = (RuleContext) rule.GetChild(0);
            RuleContext invocation = (RuleContext) rule.GetChild(1);

            List<ExpressionNode> args = ParseCallTail(invocation);
            ExpressionNode target = ParseExpression(toCall);


            return new TodoRenameCall(target, args, ExpressionType.Invocation);
        }

        public static List<ExpressionNode> ParseCallTail(RuleContext rule)
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

        private static List<ExpressionNode> ParseExpressionList(RuleContext expList)
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
        public static FlowNode ParseFlow(RuleContext rule)
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

        private static FlowNode ParseIf(RuleContext rule)
        {
            //IF expression INDENT statements DEDENT (ELSE INDENT statements DEDENT)?;
            ExpressionNode expression = ExpressionParser.ParseExpression((RuleContext)rule.GetChild(1));
            BlockNode trueBlock = ParseBlockNode((RuleContext) rule.GetChild(3));

            if (rule.ChildCount == 5)
            {
                return new SelectiveFlowNode(SelectiveFlowNode.FlowType.If, expression, trueBlock, null);
            }
            else if(rule.ChildCount == 9)
            {
                BlockNode falseBlock = ParseBlockNode((RuleContext) rule.GetChild(7));
                return new SelectiveFlowNode(SelectiveFlowNode.FlowType.IfElse, expression, trueBlock, falseBlock);
            }

            throw new NotImplementedException();
        }

        private static FlowNode ParseFor(RuleContext rule)
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

            return new ForLoopNode(type, identifierNode.GetText(), iteratior, block, rule.SourceInterval);
        }

        public static DeclerationNode ParseDeclerationNode(RuleContext rule)
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


        private static DeclerationNode ParseFunctionDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            CrawlType type = ParseType((CrawlParser.TypeContext) classPart.GetChild(0));
            ITerminalNode identifier = (ITerminalNode) classPart.GetChild(1);
            ITerminalNode assignment = (ITerminalNode) classPart.GetChild(2);
            RuleContext body = (RuleContext) classPart.GetChild(3).GetChild(1);

            if (identifier.Symbol.Type != CrawlLexer.IDENTIFIER) throw new NotImplementedException();
            if (assignment.Symbol.Type != CrawlLexer.ASSIGNMENT_SYMBOL) throw new NotImplementedException();

            return new FunctionDeclerationNode(type, identifier.GetText(), interval, ParseBlockNode(body));
        }

        private static DeclerationNode ParseVariableDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            ITerminalNode eos =  classPart.LastChild() as ITerminalNode;
            if(eos == null || eos.Symbol.Type != CrawlLexer.END_OF_STATEMENT) throw new NotImplementedException("Something strange happened");

            CrawlType type = ParseType((CrawlParser.TypeContext) classPart.GetChild(0));

            return new VariableDeclerationNode(
                protectionLevel, 
                type,
                classPart
                    .AsEdgeTrimmedIEnumerable()
                    .Cast<CrawlParser.Variable_declContext>()
                    .Select(ParseSingleVariable),
                interval);
            
        }

        private static SingleVariableDecleration ParseSingleVariable(CrawlParser.Variable_declContext variable)
        {
            ITerminalNode identifier = (ITerminalNode) variable.GetChild(0);
            if(identifier.Symbol.Type != CrawlLexer.IDENTIFIER) throw new NotImplementedException();

            if (variable.ChildCount == 1)
            {
                return new SingleVariableDecleration(identifier.GetText(), variable.GetChild(0).SourceInterval);
            }
            else if (variable.ChildCount == 3)
            {
                return new SingleVariableDecleration(identifier.GetText(), variable.GetChild(0).SourceInterval, ExpressionParser.ParseExpression((RuleContext) variable.GetChild(2)));
            }

            throw new NotImplementedException();
        }


        private static DeclerationNode ParseClassDecleration(RuleContext classPart, ProtectionLevel protectionLevel, Interval interval)
        {
            ITerminalNode tn1 = (ITerminalNode)classPart.GetChild(0);
            ITerminalNode tn2 = (ITerminalNode)classPart.GetChild(1);
            RuleContext body = (RuleContext) classPart.GetChild(2);

            if(tn1.Symbol.Type != CrawlLexer.CLASS) throw new NotImplementedException(); //TODO: WTFSYNTAXEXCEPTION with INTEVAL

            BlockNode bodyBlock = ParseBlockNode(body);
            string name = tn2.GetText();

            return new ClassDeclerationNode(protectionLevel, name, bodyBlock, interval);
        }

        #endregion

        private static CrawlType ParseType(CrawlParser.TypeContext type)
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

        public static BlockNode ParseBlockNode(RuleContext rule)
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

            List<CrawlSyntaxNode> contents = 
                meaningfullContent
                .Cast<RuleContext>()
                .Select(ParseStatement)
                .ToList();

            return new BlockNode();
        }

        public static ImportNode ParseImportNode(RuleContext rule)
        {
            throw new NotImplementedException();
        }

        public static CrawlSyntaxNode ParseStatement(RuleContext rule)
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

        private static CrawlSyntaxNode ParseReturn(RuleContext rule)
        {
            ExpressionNode retvalue = null;
            if (rule.ChildCount == 3)
            {
                retvalue = ExpressionParser.ParseExpression((RuleContext) rule.GetChild(1));
            }
            return new ReturnStatement(retvalue);
        }

        private static CrawlSyntaxNode ParseAssignment(RuleContext rule)
        {

            ExpressionNode target = ExpressionParser.ParseExpression((RuleContext)rule.GetChild(0));
            if(rule.GetChild(1) is CrawlParser.Subfield_expressionContext)
            {
                CrawlParser.Subfield_expressionContext subfield = (CrawlParser.Subfield_expressionContext) rule.GetChild(1);

                VariableNode sub = new VariableNode(subfield.GetChild(1).GetText(), subfield.GetChild(1).SourceInterval);
                target = new BinaryNode(ExpressionType.SubfieldAccess, target, sub);
            }
            else if(rule.GetChild(1) is CrawlParser.Index_expressionContext)
            {
                target = new TodoRenameCall(target, ExpressionParser.ParseCallTail((RuleContext)rule.GetChild(1)), ExpressionType.Index);
            }

            ExpressionNode value = ExpressionParser.ParseExpression((RuleContext) rule.GetChild(rule.ChildCount - 2));
            return new AssignmentNode(target, value);
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
        public NodeType Type { get; }

        protected CrawlSyntaxNode(NodeType type = NodeType.TODO)
        {
            Type = type;
        }

        public static CompiliationUnitNode Parse(CrawlParser.Translation_unitContext rootContext, CrawlSyntaxTree tree)
        {
            List<CrawlSyntaxNode> Contents = new List<CrawlSyntaxNode>();
            CrawlParser.Import_directivesContext imports =
                (CrawlParser.Import_directivesContext) rootContext.GetChild(0);

            CrawlParser.StatementsContext statements = 
                (CrawlParser.StatementsContext)rootContext.GetChild(1);


            BlockNode rootBlock = Foo.ParseBlockNode(statements);

            return new CompiliationUnitNode(Contents);
            
        }

        /*

        private static CrawlSyntaxNode ParseSideEffect(RuleContext child)
        {
            return ParsePostfixExpression(child.GetChild(0));
        }

        private static IEnumerable<OldDeclerationNode> ParseDecleration(CrawlParser.DeclarationContext child)
        {
            

            RuleContext lastChild = (RuleContext)child.GetChild(child.ChildCount - 1);
            CrawlParser.Function_or_variableContext fov = lastChild as CrawlParser.Function_or_variableContext;
            if (fov != null)
                return ParseFunctionOrVariable(fov);

            CrawlParser.Class_declarationContext classDeclaration = lastChild as CrawlParser.Class_declarationContext;
            if (classDeclaration != null)
                return ParseClass(classDeclaration);

            throw new NotImplementedException();
        }

        private static IEnumerable<OldDeclerationNode> ParseClass(CrawlParser.Class_declarationContext classDeclaration)
        {
            string name = classDeclaration.GetChild(1).GetText();

            if(classDeclaration.ChildCount != 3) throw new NotImplementedException("Inheritance");
            CrawlParser.Class_bodyContext body = (CrawlParser.Class_bodyContext)classDeclaration.LastChild();

            throw new NotImplementedException();
            
        }

        private static readonly Dictionary<string, ExpressionType> _expressionTypeMap = new Dictionary <string, ExpressionType>()
        {
            {"+", ExpressionType.Add},
            {"-", ExpressionType.Subtract},
            {"*", ExpressionType.Multiply },
            {"**", ExpressionType.Power}
        };

        private static IEnumerable<OldDeclerationNode> ParseFunctionOrVariable(CrawlParser.Function_or_variableContext fov)
        {
            
            if(fov.ChildCount == 2) //Just decl
            { throw new NotImplementedException();}
            else if (fov.GetChild(3) is CrawlParser.ExpressionContext)
            {
                if(fov.ChildCount != 5)
                { throw new NotImplementedException();} //Multiple children

                yield return
                    new VariableDefaultValueNode(
                        ParseExpression(fov.GetChild(3)),
                        fov.GetChild(0).ToString(), 
                        fov.GetChild(1).ToString());
            }
            else
            { throw new NotImplementedException(); }
        }

        private static ValueNode ParseExpression(IParseTree expression)
        {
            if (expression.ChildCount == 0)
            {
                ITerminalNode node = (ITerminalNode) expression;
                return new LiteralNode(node);
                
            }
            if (expression.ChildCount == 1)
                return ParseExpression(expression.GetChild(0));
            else if (expression.LastChild() is CrawlParser.Postfix_expressionContext)
            {
                return ParsePostfixExpression(expression);
            }
            else if (expression is CrawlParser.AtomContext)
            {
                return ParseAtom(expression);
            }
            else
            {
                string symbol = expression.GetChild(1).GetText();
                ExpressionNode node = new ExpressionNode(_expressionTypeMap[symbol]);
                for (int i = 0; i < expression.ChildCount; i+=2)
                {
                    node.Children.Add(ParseExpression(expression.GetChild(i)));
                }
                return node;
                
            }
        }

        private static ValueNode ParseAtom(IParseTree expression)
        {
            if (expression.ChildCount == 3)
            {
                return ParseExpression(expression.GetChild(1));
            }
            throw new NotImplementedException();
        }

        private static ValueNode ParsePostfixExpression(IParseTree expression, int doNotUse = 0)
        {
            RuleContext lastChild = (RuleContext)expression.GetChild(expression.ChildCount - (1 + doNotUse));
            ValueNode lhs;
            if (expression.ChildCount == 2)
                lhs = ParseExpression(expression.GetChild(0));
            else
                lhs = ParsePostfixExpression(expression, doNotUse + 1);

            if (lastChild.RuleIndex == CrawlParser.RULE_call_expression)
            {
                return new InvocationExpression(lhs, ParseExpressionList(lastChild.GetChild(1) as CrawlParser.Expression_listContext));
            }
            else if (lastChild.RuleIndex == CrawlParser.RULE_subfield_expression)
            {
                throw new NotImplementedException();
            }
            else if(lastChild.RuleIndex == CrawlParser.RULE_index_expression)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new Exception("No idea what happened");
            }
        }

        private static List<ValueNode> ParseExpressionList(CrawlParser.Expression_listContext list)
        {
            List<ValueNode> res = new List<ValueNode>();
            foreach (IParseTree child in list.children)
            {
                res.Add(ParseExpression(child));
            }
            return res;
        }
        */
    }

    public abstract class FlowNode : CrawlSyntaxNode
    { }

    internal class ForLoopNode : FlowNode
    {
        public ForLoopNode(CrawlType type, string inducedField, ExpressionNode iteratior, BlockNode block, Interval interval)
        {
            
        }
    }

    public class SelectiveFlowNode : FlowNode
    {
        public SelectiveFlowNode(FlowType type, ExpressionNode check, BlockNode primary, BlockNode alternative)
        { }

        public enum FlowType
        {
            If,
            IfElse,
            While,
        }
    }

    public class ReturnStatement : CrawlSyntaxNode
    {
        public ReturnStatement(ExpressionNode returnValue = null) { }
    }

    public class AssignmentNode : CrawlSyntaxNode
    {
        public AssignmentNode(ExpressionNode lhs, ExpressionNode rhs)
        { }
    }

    public abstract class ExpressionNode : CrawlSyntaxNode
    { }

    public class LiteralNode : ExpressionNode
    {
        public LiteralNode(string value, LiteralType type)
        {
            
        }

        public enum LiteralType
        {
            String,
            Int,
            Float,
            Boolean
        }
    }

    public class TodoRenameCall : ExpressionNode
    {
        public TodoRenameCall(ExpressionNode target, List<ExpressionNode> arguments, ExpressionType type)
        {
            
        }
    }

    public class MultiChildExpressionNode : ExpressionNode
    {
        public MultiChildExpressionNode(ExpressionType type, IEnumerable<ExpressionNode> children) { }
    }

    public class BinaryNode : ExpressionNode
    {
        public BinaryNode(ExpressionType type, ExpressionNode lhs, ExpressionNode rhs)
        { }
    }

    public class VariableNode : ExpressionNode
    {
        public VariableNode(string variableName, Interval interval)
        {

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
    { }

    public class ClassDeclerationNode : DeclerationNode
    {
        private BlockNode bodyBlock;
        private Interval interval;
        private string name;
        private ProtectionLevel protectionLevel;

        public ClassDeclerationNode(ProtectionLevel protectionLevel, string name, BlockNode bodyBlock, Interval interval)
        {
            this.protectionLevel = protectionLevel;
            this.name = name;
            this.bodyBlock = bodyBlock;
            this.interval = interval;
        }
    }

    public class VariableDeclerationNode : DeclerationNode
    {
        public List<SingleVariableDecleration> Declerations { get; }

        public VariableDeclerationNode(
            ProtectionLevel protectionLevel, 
            CrawlType type, 
            IEnumerable<SingleVariableDecleration> declerations, 
            Interval interval
        )
        {
            Declerations = declerations.ToList();
        }
    }

    public class SingleVariableDecleration : CrawlSyntaxNode
    {
        public string Name { get; }
        public Interval Interval { get; }
        public ExpressionNode DefaultValue { get; }

        public SingleVariableDecleration(string name, Interval interval, ExpressionNode defaultValue = null)
        {
            Name = name;
            Interval = interval;
            DefaultValue = defaultValue;
        }
    }

    public class FunctionDeclerationNode : DeclerationNode
    {
        public FunctionDeclerationNode(CrawlType type, string name, Interval interval, BlockNode block)
        {
            
        }
    }

    #endregion

    public class BlockNode : CrawlSyntaxNode
    {
        

    }

    public abstract class ImportNode : CrawlSyntaxNode
    { }

    

    public class CompiliationUnitNode : CrawlSyntaxNode
    {
        private List<CrawlSyntaxNode> list;

        //This should plausibly be 2 lists. 1 of All declarations (functions/classes/namespaces) and 1 of statements;
        //And maybe even a third, imports;
        public CompiliationUnitNode(List<CrawlSyntaxNode> children) : base(NodeType.TODO)
        {
            Children = children;
        }

        public List<CrawlSyntaxNode> Children { get; } = new List<CrawlSyntaxNode>();
    }



    /*

    public enum NodeType
    {
        Literal,
        CompilationUnit,
        Expression,
        MemberAccess,
        Index,
        Call,
        Decleration,
        ClassDecleration
    }
    
    public class BlockNode : CrawlSyntaxNode
    {
        public BlockNode(NodeType type, IEnumerable<CrawlSyntaxNode> children) : base(type)
        {
        }
    }

    public abstract class ValueNode : CrawlSyntaxNode
    {
        protected ValueNode(NodeType type) : base(type)
        {
        }
    }

    public class ClassDecleration : DeclerationNode
    {
        public ClassDecleration(string name, IEnumerable<DeclerationNode> contents) : base(NodeType.ClassDecleration)
        {
        }
    }

    public class LiteralNode : ValueNode
    {
        public override string ToString()
        {
            return Node.GetText();
        }

        public ITerminalNode Node { get; }

        public LiteralNode(ITerminalNode node) : base(NodeType.Literal)
        {
            Node = node;
        }
    }

    

    public class ExpressionNode : ValueNode
    {
        public ExpressionNode(ExpressionType expressionType) : base(NodeType.Expression)
        {
            ExpressionType = expressionType;
        }

        public ExpressionNode(ExpressionType expressionType, List<CrawlSyntaxNode> children) : this(expressionType)
        {
            Children = children;
        }

        public override string ToString()
        {
            return ExpressionType.ToString();
        }

        public List<CrawlSyntaxNode> Children { get; } = new List<CrawlSyntaxNode>();
        public ExpressionType ExpressionType { get; }
    }

    public class MemberAccessNode : ValueNode
    {
        public MemberAccessNode(ValueNode parent, string subfield) : base(NodeType.MemberAccess)
        {
            Subfield = subfield;
            Parent = parent;
        }

        public string Subfield { get; }
        public ValueNode Parent { get; }
    }

    public class IndexExpression : ValueNode
    {
        public IndexExpression(List<ValueNode> arguments) : base(NodeType.Index)
        {
            Arguments = arguments;
        }

        public List<ValueNode> Arguments { get; }
    }

    public class InvocationExpression : ValueNode  //Not sure if valueNode or ExpressionNode?
    {
        public InvocationExpression(ValueNode method, List<ValueNode> arguments) : base(NodeType.Call)
        {
            Method = method;
            Arguments = arguments;
        }

        public List<ValueNode> Arguments { get; }
        
        public ValueNode Method { get; }
    }

    public enum ExpressionType
    {
        Add,
        Subtract,
        Power,
        Multiply
    }

    public abstract class DeclerationNode : CrawlSyntaxNode
    {
        protected DeclerationNode(NodeType type) : base(type)
        {
        }
    }

    public class OldDeclerationNode : CrawlSyntaxNode
    {
        public OldDeclerationNode(string type, string identifier) : base(NodeType.Decleration)
        {
            Type = type;
            Identifier = identifier;
        }

        public string Type { get; }
        public string Identifier { get; }
    }

    public class VariableDefaultValueNode : OldDeclerationNode
    {
        //TODO: Needs to set NodeType or do something with this and DeclerationNode
        public VariableDefaultValueNode(ValueNode defaultValue, string type, string identifier) : base(type, identifier)
        {
            DefaultValue = defaultValue;
        }

        private ValueNode DefaultValue { get; }
    }
    
    public class CompiliationUnitNode : CrawlSyntaxNode
    { }

    public class CompiliationUnitNode : CrawlSyntaxNode
    { }

    public class CompiliationUnitNode : CrawlSyntaxNode
    { }

    public class CompiliationUnitNode : CrawlSyntaxNode
    { }

    public class CompiliationUnitNode : CrawlSyntaxNode
    { }

    */

}