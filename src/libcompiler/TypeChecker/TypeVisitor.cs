using System;
using System.ComponentModel;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.TypeChecker
{
    public class TypeVisitor : SyntaxRewriter
    {
        #region Literals
        protected override CrawlSyntaxNode VisitIntegerLiteral(IntegerLiteralNode integerLiteral)
        {
            return integerLiteral.WithResultType(CrawlSimpleType.Tal);
        }

        protected override CrawlSyntaxNode VisitBooleanLiteral(BooleanLiteralNode booleanLiteral)
        {
            return booleanLiteral.WithResultType(CrawlSimpleType.Bool);
        }

        protected override CrawlSyntaxNode VisitStringLiteral(StringLiteralNode stringLiteral)
        {
            return stringLiteral.WithResultType(CrawlSimpleType.Tekst);
        }

        protected override CrawlSyntaxNode VisitRealLiteral(RealLiteralNode realLiteral)
        {
            return realLiteral.WithResultType(CrawlSimpleType.Kommatal);
        }
        #endregion

        #region Operators

        protected override CrawlSyntaxNode VisitUnaryExpression(UnaryExpressionNode expression)
        {
            CrawlSyntaxNode result;
            UnaryExpressionNode expr = (UnaryExpressionNode) base.VisitUnaryExpression(expression);

            switch (expr.ExpressionType)
            {
                case ExpressionType.Negate:
                    result = expr.WithResultType(
                        ExpressionEvaluator.UnaryNegate(expr.Target.ResultType)
                    );
                    break;

                case ExpressionType.Not:
                    result = expr.WithResultType(
                        ExpressionEvaluator.UnaryNot(expr.Target.ResultType)
                    );
                    break;

                default:
                    throw new InvalidEnumArgumentException("Invalid type of unary operator");
            }

            return result;
        }

        protected override CrawlSyntaxNode VisitBinaryExpression(BinaryExpressionNode binaryExpression)
        {
            return base.VisitBinaryExpression(binaryExpression); //Delete this line if you want it throwing exceptions left and right.
            CrawlSyntaxNode result;
            BinaryExpressionNode expr = (BinaryExpressionNode) base.VisitBinaryExpression(binaryExpression);

            switch (expr.ExpressionType)
            {                        case ExpressionType.Greater:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryGreater(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.GreaterEqual:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryGreaterEqual(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.Equal:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryEqual(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.NotEqual:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryNotEqual(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.LessEqual:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryLessEqual(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.Less:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryLess(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.Power:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryPower(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.Range:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryRange(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.ShortCircuitOr:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryShortCircuitOr(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                case ExpressionType.ShortCircuitAnd:
                    result = expr.WithResultType(
                        ExpressionEvaluator.BinaryShortCircuitAnd(expr.LeftHandSide.ResultType, expr.RightHandSide.ResultType)
                    );
                    break;

                default:
                    throw new InvalidEnumArgumentException("Invalid type of binary operator");
            }

            return result;
        }

        #endregion

    }
}