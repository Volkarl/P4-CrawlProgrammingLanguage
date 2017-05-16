using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using libcompiler.ExtensionMethods;
using libcompiler.Parser;
using libcompiler.TypeSystem;

namespace libcompiler.SyntaxTree.Parser
{
    public static class ExpressionParser
    {
        public static ExpressionNode ParseExpression(RuleContext rule)
        {
            //Literal value.
            if (rule.RuleIndex == CrawlParser.RULE_literal)
                return ParseLiteral(rule);

            if (rule.RuleIndex == CrawlParser.RULE_atom)
            {
                //Reference to variable by identifier.
                ITerminalNode tn = rule.GetChild(0) as ITerminalNode;
                if (tn != null && tn.Symbol.Type == CrawlLexer.IDENTIFIER)
                {
                    return CrawlSyntaxNode.Variable(tn.SourceInterval, CrawlType.UnspecifiedType,  tn.GetText());
                }
                //Expression in parentheses. Parse only content, throw out parentheses.
                else if (rule.ChildCount == 3)
                {
                    return ParseExpression((RuleContext) rule.GetChild(1));
                }
            }

            //If this is just an intermedite node, proceed to the next one.
            if(rule.ChildCount == 1)
                return ParseExpression((RuleContext) rule.GetChild(0));

            //And lastly handle operators.
            switch (rule.RuleIndex)
            {
                case CrawlParser.RULE_postfix_expression:
                    return ParsePostfix(rule);
                case CrawlParser.RULE_array_initialization_expression:
                    return ParseArrayInitialization(rule);
                case CrawlParser.RULE_comparison_expression:
                case CrawlParser.RULE_range_expression:
                    return ParseBinary(rule);
                case CrawlParser.RULE_additive_expression:
                case CrawlParser.RULE_multiplicative_expression:
                case CrawlParser.RULE_and_expression:
                case CrawlParser.RULE_or_expression:
                case CrawlParser.RULE_exponential_expression:
                    return ParseMultu(rule);
                case CrawlParser.RULE_cast_expression:
                    return ParseCast(rule);
                case CrawlParser.RULE_unary_expression:
                    return ParseUnary(rule);
                default:
                    throw new NotImplementedException("Some expression type is not handled");
                    
            }
        }

        private static ExpressionNode ParseUnary(RuleContext rule)
        {
            //( INVERT | MINUS )* postfix_expression
            RuleContext targetContext = (RuleContext) rule.LastChild();

            List<ExpressionType> unaries = new List<ExpressionType>();
            for (int i = rule.ChildCount-2; i >= 0; i--)
            {
                ITerminalNode symbol = (ITerminalNode) rule.GetChild(i);
                unaries.Add(ParseUnaryOp(symbol));
            }

            ExpressionNode result = ParseExpression(targetContext);

            foreach (ExpressionType unaryExpressionType in unaries)
            {
                result = CrawlSyntaxNode.UnaryExpression(rule.SourceInterval, unaryExpressionType, CrawlType.UnspecifiedType, result);
            }

            return result;
        }

        private static ExpressionNode ParseArrayInitialization(RuleContext rule)
        {
            // An array initialization is the call of a typeNode's constructor

            TypeNode type = ParseTreeParser.ParseType((CrawlParser.TypeContext)rule.GetChild(0));
            ArrayConstructorNode typeConstructor = CrawlSyntaxNode.ArrayConstructor(type.Interval,CrawlType.UnspecifiedType,  type);
            // TypeConstructorNode is an ExpressionNode that corresponds to a TypeNode

            IEnumerable<ArgumentNode> arguments = ParseCallTail((RuleContext)rule.GetChild(1)).Select(x => CrawlSyntaxNode.Argument(x.Interval, false, x));
            return CrawlSyntaxNode.Call(rule.SourceInterval, CrawlType.UnspecifiedType, typeConstructor, arguments);
        }

        private static ExpressionNode ParseMultu(RuleContext rule)
        {
            List<ExpressionNode> sources = new List<ExpressionNode>();
            sources.Add(ParseExpression((RuleContext)rule.GetChild(0)));
            ExpressionType type = ParseMultiOp((ITerminalNode) rule.GetChild(1));

            //Operations of same precedence may be chained together. Go through rest of the operator nodes.
            for (int i = 1; i < rule.ChildCount; i+=2)
            {
                ExpressionType newtype = ParseMultiOp((ITerminalNode) rule.GetChild(i));
                if (newtype == type)
                    sources.Add(ParseExpression((RuleContext) rule.GetChild(i + 1)));
                else
                {
                    //TODO: Actually calculate the interval
                    //The expression contained multiple types of operations
                    ExpressionNode newNode = CrawlSyntaxNode.MultiChildExpression(Interval.Invalid, type, CrawlType.UnspecifiedType, sources);
                    sources.Clear();
                    sources.Add(newNode);
                    type = newtype;
                    sources.Add(ParseExpression((RuleContext)rule.GetChild(i + 1)));
                }
            }

            return CrawlSyntaxNode.MultiChildExpression(rule.SourceInterval, type, CrawlType.UnspecifiedType, sources);
        }

        private static ExpressionNode ParseCast(RuleContext expression)
        {
            //( LPARENTHESIS type RPARENTHESIS ) * unary_expression
            var castExpression = (CrawlParser.Cast_expressionContext) expression;
            List<TypeNode> targetTypes = new List<TypeNode>();

            //Parse every part of the chain of casts. The for-loop works; Trust me.
            for (int i = castExpression.ChildCount -3; i > 2; i-=3)
            {
                targetTypes.Add(ParseTreeParser.ParseType((CrawlParser.TypeContext)castExpression.GetChild(i)));
            }

            //Parse the rest of the expression.
            ExpressionNode result = ParseExpression((RuleContext) castExpression.LastChild());

            //Wrap the casts around the rest of the expression one at a time.
            foreach (TypeNode type in targetTypes)
            {
                result = CrawlSyntaxNode.CastExpression(
                    Interval.Invalid, ExpressionType.Cast, CrawlType.UnspecifiedType,
                    type,
                    result
                );
            }

            return result;
        }

        private static ExpressionNode ParseBinary(RuleContext rule)
        {
            if (rule.ChildCount != 3) throw new CrawlImpossibleStateException("SHOULD NOT HAPPEN", rule.SourceInterval);

            RuleContext lhs = (RuleContext)rule.GetChild(0);

            IParseTree opChild = rule.GetChild(1);
            if (opChild is RuleContext)
            {
                opChild = opChild.GetChild(0);
            }
            ITerminalNode op = (ITerminalNode)opChild;
            RuleContext rhs = (RuleContext)rule.GetChild(2);

            ExpressionNode lhsNode = ParseExpression(lhs);
            ExpressionType type = ParseBinaryOp(op);
            ExpressionNode rhsNode = ParseExpression(rhs);
            
            return CrawlSyntaxNode.BinaryExpression(rule.SourceInterval, type, CrawlType.UnspecifiedType, lhsNode, rhsNode);
        }

        private static readonly Dictionary<string, ExpressionType> BinaryTypeMap = new Dictionary<string, ExpressionType>()
        {
            {">",  ExpressionType.Greater },
            {">=", ExpressionType.GreaterEqual },
            {"==", ExpressionType.Equal },
            {"!=", ExpressionType.NotEqual },
            {"<=", ExpressionType.LessEqual },
            {"<",  ExpressionType.Less },
            {"til", ExpressionType.Range }
        };

        private static ExpressionType ParseBinaryOp(ITerminalNode op)
        {
            string textop = op.GetText();
            ExpressionType et;
            if (BinaryTypeMap.TryGetValue(textop, out et))
                return et;

            throw new NotImplementedException($"There is no known binary {nameof(ExpressionType)} for {textop}");
        }

        private static readonly Dictionary<string, ExpressionType> MultiTypeMap = new Dictionary<string, ExpressionType>
        {
            {"-", ExpressionType.Subtract },
            {"+", ExpressionType.Add },
            {"*", ExpressionType.Multiply},
            {"/", ExpressionType.Divide},
            {"%", ExpressionType.Modulous},
            {"**", ExpressionType.Power },
            {"eller", ExpressionType.ShortCircuitOr },
            {"og", ExpressionType.ShortCircuitAnd}
        };

        private static ExpressionType ParseMultiOp(ITerminalNode op)
        {
            string textop = op.GetText();
            ExpressionType et;
            if (MultiTypeMap.TryGetValue(textop, out et))
                return et;

            throw new NotImplementedException($"There is no known multiop {nameof(ExpressionType)} for {textop}");
        }

        private static readonly Dictionary<string, ExpressionType> UnaryMap = new Dictionary<string, ExpressionType>
        {
            {"-", ExpressionType.Negate },
            {"ikke", ExpressionType.Not},
        };

        private static ExpressionType ParseUnaryOp(ITerminalNode op)
        {
            string textop = op.GetText();
            ExpressionType et;
            if (UnaryMap.TryGetValue(textop, out et))
                return et;


            throw new NotImplementedException($"There is no known unary {nameof(ExpressionType)} for {textop}");
        }

        private static ExpressionNode ParsePostfix(RuleContext rule)
        {
            ExpressionNode node = ParseExpression((RuleContext) rule.GetChild(0));

            for (int i = 1; i < rule.ChildCount; i++)
            {
                RuleContext post = (RuleContext)rule.GetChild(i);
                if (post.RuleIndex == CrawlParser.RULE_call_expression)
                {
                    node = CrawlSyntaxNode.Call(post.SourceInterval, CrawlType.UnspecifiedType, node, ParseArgumentList(post));
                }
                else if (post.RuleIndex == CrawlParser.RULE_index_expression)
                {
                    node = CrawlSyntaxNode.Index(post.SourceInterval, CrawlType.UnspecifiedType, node, ParseArgumentList(post));
                }
                else if(post.RuleIndex == CrawlParser.RULE_subfield_expression)
                {
                    IdentifierNode sub = CrawlSyntaxNode.Identifier(post.GetChild(1).SourceInterval, post.GetChild(1).GetText());
                    node = CrawlSyntaxNode.MemberAccess(post.SourceInterval, CrawlType.UnspecifiedType,  node, sub);
                }
                else if (post.RuleIndex == CrawlParser.RULE_generic_unpack_expression)
                {
                    List<TypeNode> generics = new List<TypeNode>();
                    for (int j = 1; j < post.ChildCount; j += 2)
                        generics.Add(ParseTreeParser.ParseType((CrawlParser.TypeContext) post.GetChild(j)));
                    node = CrawlSyntaxNode.GenericsUnpack(post.SourceInterval, CrawlType.UnspecifiedType, node, generics);

                }
                else throw new NotImplementedException("Strange postfix expression");

            }

            return node;
        }

        private static Dictionary<char, char> escapes = new Dictionary<char, char>()
        {
            {'b', '\b'},
            {'f', '\f'},
            {'n', '\n'},
            {'r', '\r'},
            {'t', '\t'},
            {'v', '\v'},
            {'\\', '\\'},
            {'\'', '\''},
            {'"', '"'}
        };
        private static string Unescape(string instring)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < instring.Length -1; i++)
            {
                if (instring[i] == '\\')
                {
                    i++;
                    sb.Append(escapes[instring[i]]);
                }
                else
                {
                    sb.Append(instring[i]);
                }
            }

            return sb.ToString();
        }

        private static ExpressionNode ParseLiteral(RuleContext rule)
        {
            RuleContext realLiteral = (RuleContext) rule.GetChild(0);

            switch (realLiteral.RuleIndex)
            {
                case CrawlParser.RULE_string_literal:
                    return CrawlSyntaxNode.StringLiteral(realLiteral.SourceInterval, CrawlType.UnspecifiedType, Unescape(realLiteral.GetText()));
                case CrawlParser.RULE_integer_literal:
                    return CrawlSyntaxNode.IntegerLiteral(realLiteral.SourceInterval, CrawlType.UnspecifiedType, int.Parse(realLiteral.GetText()));
                case CrawlParser.RULE_boolean_literal:
                    return CrawlSyntaxNode.BooleanLiteral(realLiteral.SourceInterval,CrawlType.UnspecifiedType,  (realLiteral.GetText()) == "true");
                case CrawlParser.RULE_real_literal:
                    return CrawlSyntaxNode.RealLiteral(realLiteral.SourceInterval,CrawlType.UnspecifiedType,  double.Parse(realLiteral.GetText()));
                case CrawlParser.RULE_null_literal:
                    return CrawlSyntaxNode.NullLiteral(realLiteral.SourceInterval, CrawlSimpleType.Tom);
                default:
                    throw new NotImplementedException("Strange literal type");
            }

        }

        public static ExpressionNode ParseSideEffectStatement(RuleContext rule)
        {
            ITerminalNode eos = (ITerminalNode) rule.GetChild(2);
            if(eos.Symbol.Type != CrawlLexer.END_OF_STATEMENT)
                throw new CrawlImpossibleStateException($"Method call not ending {nameof(CrawlLexer.END_OF_STATEMENT)}", rule.SourceInterval);

            RuleContext toCall = (RuleContext) rule.GetChild(0);
            RuleContext invocation = (RuleContext) rule.GetChild(1);

            List<ArgumentNode> args = ParseArgumentList(invocation).ToList();
            ExpressionNode target = ParseExpression(toCall);

            return CrawlSyntaxNode.Call(rule.SourceInterval, CrawlType.UnspecifiedType, target, args);
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

            throw new NotImplementedException("Not sure when this could happen....");
        }

        private static List<ExpressionNode> ParseExpressionList(RuleContext expList)
        {
            List<ExpressionNode> n = new List<ExpressionNode>(expList.ChildCount / 2);
            for (int i = 0; i < expList.ChildCount; i += 2)
            {

                n.Add(ParseExpression((RuleContext) expList.GetChild(i)));

                if (i + 1 != expList.ChildCount)
                {
                    ITerminalNode itemsep = (ITerminalNode) expList.GetChild(i+1);
                    if(itemsep.Symbol.Type != CrawlLexer.ITEM_SEPARATOR) throw new NotImplementedException("Strange stuff in expression list");
                }
            }
            return n;
        }

        public static IEnumerable<ArgumentNode> ParseArgumentList(RuleContext argList)
        {
            if(argList.ChildCount == 2) yield break;

            argList = (RuleContext)argList.GetChild(1);

            List<ExpressionNode> n = new List<ExpressionNode>(argList.ChildCount / 2);
            for (int i = 0; i < argList.ChildCount; i += 2)
            {
                int starti = i;
                bool reference = ((argList.GetChild(i) as ITerminalNode)?.Symbol?.Type == CrawlLexer.REFERENCE);

                if (reference) i++;

                Interval interval = argList.GetChild(starti).SourceInterval;
                interval = interval.Union(argList.GetChild(starti + (reference ? 1 : 0)).SourceInterval);
                yield return CrawlSyntaxNode.Argument(interval, reference,
                    ParseExpression((RuleContext) argList.GetChild(i)));

                if (i + 1 != argList.ChildCount)
                {
                    ITerminalNode itemsep = (ITerminalNode) argList.GetChild(i+1);
                    if(itemsep.Symbol.Type != CrawlLexer.ITEM_SEPARATOR) throw new NotImplementedException("Strange stuff in argument list");
                }
            }
            /*
            List<ArgumentNode> n = new List<ArgumentNode>();
            for (int i = 0; i < refExpList.ChildCount; i += 2)
            {
                var refTerminalNode = refExpList.GetChild(i) as ITerminalNode;
                if (refTerminalNode == null) // If there is no reference, then the child is an expression
                {
                    n.Add(ParseExpression((RuleContext) refExpList.GetChild(i)));
                }
                else if (refTerminalNode.Symbol.Type == CrawlLexer.REFERENCE)
                {
                    // If first child is a reference terminal, then the second one is an expression
                    i = i + 1;
                    ExpressionNode target = ParseExpression((RuleContext) refExpList.GetChild(i));
                    n.Add(CrawlSyntaxNode.ReferenceNode(target));
                }

                if (i != refExpList.ChildCount - 1) 
                {
                    // If there are any additional children, then the next one has to be an item separator
                    ITerminalNode itemsep = (ITerminalNode) refExpList.GetChild(i + 1);
                    if (itemsep.Symbol.Type != CrawlLexer.ITEM_SEPARATOR) throw new NotImplementedException("Strange stuff in reference expression list");
                }
            }
            return n; */
        }
    }
}