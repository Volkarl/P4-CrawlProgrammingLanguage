using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Remoting.Channels;
using Antlr4.Runtime.Misc;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.Optimizations
{
    public partial class ConstantFoldingVisitor
    {
        //Lot of swithces here. You want to have an editor that can collapse stuff.

        private static double _getNumber(LiteralNode node)
        {

            switch (node.Type)
            {
                case NodeType.IntegerLiteral:
                    return ((IntegerLiteralNode) node).Value;
                case NodeType.RealLiteral:
                    return ((RealLiteralNode) node).Value;
                default:
                    throw new Exception("Type error where no type error should be.");
            }
        }


        private LiteralNode _foldUnary(ExpressionNode target, ExpressionType expressionType)
        {
            if (expressionType == ExpressionType.Negate)
            {
                var asReal = target as RealLiteralNode;
                if (asReal != null)
                {
                    double resultValue = - asReal.Value;
                    return CrawlSyntaxNode.RealLiteral(target.Interval, CrawlSimpleType.Kommatal, resultValue);
                }
                var asInt = target as IntegerLiteralNode;
                if (asInt != null)
                {
                    int resultValue = -asInt.Value;
                    return CrawlSyntaxNode.IntegerLiteral(target.Interval, CrawlSimpleType.Tal, resultValue);
                }
            }
            //else if (expressionType == ExpressionType.Not)
            {
                var asBool = (BooleanLiteralNode)target;
                bool resultValue = !asBool.Value;
                return CrawlSyntaxNode.BooleanLiteral(target.Interval, CrawlSimpleType.Bool, resultValue);
            }
        }


        private static LiteralNode _foldBinary(LiteralNode op1, ExpressionType expressionType, LiteralNode op2)
        {
            var val1 = _getNumber(op1);
            var val2 = _getNumber(op2);

            bool result;
            switch (expressionType)
            {
                case ExpressionType.Greater:
                    result = val1 > val2;
                    break;
                case ExpressionType.GreaterEqual:
                    result = val1 >= val2;
                    break;
                case ExpressionType.Less:
                    result = val1 < val2;
                    break;
                case ExpressionType.LessEqual:
                    result = val1 <= val2;
                    break;
                case ExpressionType.Equal:
                    result = Math.Abs(val1 - val2) < Double.Epsilon;
                    break;
                case ExpressionType.NotEqual:
                    result = Math.Abs(val1 - val2) > Double.Epsilon;
                    break;
                default:
                    throw new Exception("Wait this isn't a binary operator!");
            }

            Interval interval = new Interval(op1.Interval.a, op2.Interval.b);

            return CrawlSyntaxNode.BooleanLiteral(interval, CrawlSimpleType.Bool, result);
        }

        private static LiteralNode _foldMultuPair(LiteralNode op1, ExpressionType expressionType, LiteralNode op2)
        {
            if (expressionType == ExpressionType.Add)
                return _foldTheDreadedAddition(op1, op2);

            Interval interval = new Interval(op1.Interval.a, op2.Interval.b);

            IntegerLiteralNode int1 = op1 as IntegerLiteralNode;
            IntegerLiteralNode int2 = op2 as IntegerLiteralNode;
            if (int1 != null && int2 != null)
            {
                int result;
                switch (expressionType)
                {
                    case ExpressionType.Divide:
                        result = int1.Value / int2.Value;
                        break;
                    case ExpressionType.Modulous:
                        result = int1.Value % int2.Value;
                        break;
                    case ExpressionType.Multiply:
                        result = int1.Value * int2.Value;
                        break;
                    case ExpressionType.Power:
                        result = (int)Math.Pow(int1.Value, int2.Value);
                        break;
                    case ExpressionType.Range:
                        throw new NotImplementedException();
                    default:
                        throw new Exception("Operator and/or type entirely unexpected!");
                }
                return CrawlSyntaxNode.IntegerLiteral(interval, CrawlSimpleType.Tal, result);
            }
            BooleanLiteralNode boo1 = op1 as BooleanLiteralNode;
            BooleanLiteralNode boo2 = op2 as BooleanLiteralNode;
            if (boo1 != null && boo2 != null)
            {
                bool result;
                switch (expressionType)
                {
                    case ExpressionType.ShortCircuitAnd:
                        result = boo1.Value && boo2.Value;
                        break;
                    case ExpressionType.ShortCircuitOr:
                        result = boo1.Value || boo2.Value;
                        break;
                    default:
                        throw new Exception("Operator and/or type entirely unexpected!");
                }

                return CrawlSyntaxNode.BooleanLiteral(interval, CrawlSimpleType.Bool, result);
            }

            //Else one or both operands are kommatal, so both are casted to that.
            double rea1 = _getNumber(op1);
            double rea2 = _getNumber(op2);
            double res;
            switch (expressionType)
            {
                case ExpressionType.Divide:
                    res = rea1 / rea2;
                    break;
                case ExpressionType.Modulous:
                    res = rea1 % rea2;
                    break;
                case ExpressionType.Multiply:
                    res = rea1 * rea2;
                    break;
                case ExpressionType.Power:
                    res = Math.Pow(rea1, rea2);
                    break;
                default:
                    throw new Exception("Operator and/or type entirely unexpected!");
            }
            return CrawlSyntaxNode.RealLiteral(interval, CrawlSimpleType.Tal, res);

        }


        private static LiteralNode _foldTheDreadedAddition(LiteralNode op1, LiteralNode op2)
        {
            Interval interval = new Interval(op1.Interval.a, op2.Interval.b);

            //Aye.
            IntegerLiteralNode int1 = op1 as IntegerLiteralNode;
            IntegerLiteralNode int2 = op2 as IntegerLiteralNode;
            RealLiteralNode rea1 = op1 as RealLiteralNode;
            RealLiteralNode rea2 = op2 as RealLiteralNode;
            StringLiteralNode str1 = op1 as StringLiteralNode;
            StringLiteralNode str2 = op2 as StringLiteralNode;

            if (int1 != null && int2 != null)
            {
                int result = int1.Value + int2.Value;
                return CrawlSyntaxNode.IntegerLiteral(interval, CrawlSimpleType.Tal, result);
            }
            if (str1 != null && str2 != null)
            {
                string result = str1.Value + str2.Value;
                return CrawlSyntaxNode.StringLiteral(interval, CrawlSimpleType.Tekst, result);
            }
            if (rea1 != null && rea2 != null)
            {
                double result = rea1.Value + rea2.Value;
                return CrawlSyntaxNode.RealLiteral(interval, CrawlSimpleType.Kommatal, result);
            }
            //Hvis de er forskellige, se om a kan tildeles til b, hvis ja,  konverter a til b's type
            //Hvis stadig ikke se om b kan tildeles til a, hvis ja, konverter b til a's type
            if (str1 != null)
            {
                string result = str1.Value + (int2?.Value.ToString() ?? rea2?.Value.ToString(CultureInfo.GetCultureInfo("en-GB")));
                return CrawlSyntaxNode.StringLiteral(interval, CrawlSimpleType.Tekst, result);
            }
            if (str2 != null)
            {
                string result = (int1?.Value.ToString() ?? rea1?.Value.ToString(CultureInfo.GetCultureInfo("en-GB"))) + str2.Value;
                return CrawlSyntaxNode.StringLiteral(interval, CrawlSimpleType.Tekst, result);
            }
            if (rea1 != null)
            {
                double result = rea1.Value + int2.Value;
                return CrawlSyntaxNode.RealLiteral(interval, CrawlSimpleType.Kommatal, result);
            }
            if (rea2 != null)
            {
                double result = int1.Value + rea2.Value;
                return CrawlSyntaxNode.RealLiteral(interval, CrawlSimpleType.Kommatal, result);
            }
            throw new NullReferenceException();
        }

    }
}