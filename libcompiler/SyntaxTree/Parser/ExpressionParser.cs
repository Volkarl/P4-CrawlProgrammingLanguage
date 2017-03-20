using System;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using libcompiler.Parser;
using libcompiler.SyntaxTree.Nodes;

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
                    return NodeFactory.VariableAccess(tn.SourceInterval, tn.GetText());
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
                case CrawlParser.RULE_comparison_expression:
                case CrawlParser.RULE_range_expression:
                    return ParseBinary(rule);
                case CrawlParser.RULE_additive_expression:
                case CrawlParser.RULE_multiplicative_expression:
                case CrawlParser.RULE_and_expression:
                case CrawlParser.RULE_or_expression:
                case CrawlParser.RULE_exponential_expression:
                    return ParseMultu(rule);
                case CrawlParser.RULE_unary_expression:
                    return ParseUnary(rule);
                default:
                    throw new NotImplementedException("Some expression type is not handled");
                    
            }
        }

        private static ExpressionNode ParseUnary(RuleContext rule)
        {
            RuleContext tar = (RuleContext) rule.GetChild(1);
            ExpressionNode target = ParseExpression(tar);

            ITerminalNode symbol = (ITerminalNode) rule.GetChild(0).GetChild(0);
            ExpressionType type = ParseUnaryOp(symbol);

            return NodeFactory.UnaryExpression(rule.SourceInterval, type, target);
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
                    ExpressionNode newNode = NodeFactory.MultiExpression(Interval.Invalid, type, sources);
                    sources.Clear();
                    sources.Add(newNode);
                    type = newtype;
                    sources.Add(ParseExpression((RuleContext)rule.GetChild(i + 1)));
                }
            }

            return NodeFactory.MultiExpression(rule.SourceInterval, type, sources);
        }

        private static ExpressionNode ParseBinary(RuleContext rule)
        {
            if (rule.ChildCount != 3) throw new CrawlImpossibleStateException("SHOULD NOT HAPPEN", rule.SourceInterval);

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
                else throw new NotImplementedException("Strange postfix expression");

            }

            return node;
        }

        private static ExpressionNode ParseLiteral(RuleContext rule)
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

            List<ExpressionNode> args = ParseCallTail(invocation);
            ExpressionNode target = ParseExpression(toCall);

            return NodeFactory.Call(rule.SourceInterval, target, args);
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
    }
}