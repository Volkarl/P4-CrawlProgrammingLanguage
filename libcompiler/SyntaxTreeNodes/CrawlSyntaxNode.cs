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

    public class Foo
    {
        public static ExpressionNode ParseExpression(RuleContext rule)
        {
            throw new NotImplementedException();
        }

        public static FlowNode ParseFlow(RuleContext rule)
        {
            throw new NotImplementedException();
        }

        public static DeclerationNode ParseDeclerationNode(RuleContext rule)
        {
            ProtectionLevel protectionLevel = ProtectionLevel.None;
            protectionLevel = ParseProtectionLevel((CrawlParser.Protection_levelContext) rule.GetChild(0));

            RuleContext declpart = (RuleContext)rule.GetChild(1);

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
                default:
                    throw new NotImplementedException();
            }
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

    public abstract class ExpressionNode : CrawlSyntaxNode
    { }

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