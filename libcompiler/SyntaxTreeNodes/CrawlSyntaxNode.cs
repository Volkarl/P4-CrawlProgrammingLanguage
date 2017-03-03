using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using libcompiler.ExtensionMethods;
using libcompiler.Parser;

namespace libcompiler
{
    public abstract class CrawlSyntaxNode
    {
        public NodeType Type { get; }

        protected CrawlSyntaxNode(NodeType type)
        {
            Type = type;
        }

        public static CompiliationUnitNode Parse(CrawlParser.Translation_unitContext rootContext, CrawlSyntaxTree tree)
        {
            List<CrawlSyntaxNode> Contents = new List<CrawlSyntaxNode>();
            CrawlParser.StatementsContext statements = (CrawlParser.StatementsContext)rootContext.GetChild(1);
            for (int i = 0; i < statements.ChildCount; i++)
            {
                RuleContext child = (RuleContext)statements.GetChild(i);

                if (child is CrawlParser.DeclarationContext)
                {
                    Contents.AddRange(ParseDecleration((CrawlParser.DeclarationContext) child));
                }
                else 
                    Contents.Add(ParseSideEffect(child));

                Console.WriteLine(CrawlParser.ruleNames[child.RuleIndex]);
            }

            return new CompiliationUnitNode(Contents);
            
        }

        private static CrawlSyntaxNode ParseSideEffect(RuleContext child)
        {
            return ParsePostfixExpression(child.GetChild(0));
        }

        private static IEnumerable<DeclerationNode> ParseDecleration(CrawlParser.DeclarationContext child)
        {
            

            RuleContext lastChild = (RuleContext)child.GetChild(child.ChildCount - 1);
            CrawlParser.Function_or_variableContext fov = lastChild as CrawlParser.Function_or_variableContext;
            if (fov != null)
                return ParseFunctionOrVariable(fov);
            

            throw new NotImplementedException();
        }

        private static readonly Dictionary<string, ExpressionType> _expressionTypeMap = new Dictionary <string, ExpressionType>()
        {
            {"+", ExpressionType.Add},
            {"-", ExpressionType.Subtract},
            {"*", ExpressionType.Multiply },
            {"**", ExpressionType.Power}
        };

        private static IEnumerable<DeclerationNode> ParseFunctionOrVariable(CrawlParser.Function_or_variableContext fov)
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

        
    }

    public enum NodeType
    {
        Literal,
        CompilationUnit,
        Expression,
        MemberAccess,
        Index,
        Call,
        Decleration
    }

    public abstract class ValueNode : CrawlSyntaxNode
    {
        protected ValueNode(NodeType type) : base(type)
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

    public class CompiliationUnitNode : CrawlSyntaxNode
    {
        private List<CrawlSyntaxNode> list;

        //This should plausibly be 2 lists. 1 of All declarations (functions/classes/namespaces) and 1 of statements;
        //And maybe even a third, imports;
        public CompiliationUnitNode(List<CrawlSyntaxNode> children) : base(NodeType.CompilationUnit)
        {
            Children = children;
        }

        public List<CrawlSyntaxNode> Children { get; } = new List<CrawlSyntaxNode>();
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


    public class DeclerationNode : CrawlSyntaxNode
    {
        public DeclerationNode(string type, string identifier) : base(NodeType.Decleration)
        {
            Type = type;
            Identifier = identifier;
        }

        public string Type { get; }
        public string Identifier { get; }
    }

    public class VariableDefaultValueNode : DeclerationNode
    {
        //TODO: Needs to set NodeType or do something with this and DeclerationNode
        public VariableDefaultValueNode(ValueNode defaultValue, string type, string identifier) : base(type, identifier)
        {
            DefaultValue = defaultValue;
        }

        private ValueNode DefaultValue { get; }
    }
    /*
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