using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            BinaryExpressionNode newExpressoinNode = (BinaryExpressionNode) (base.VisitBinaryExpression(binaryExpression) );
            CrawlType leftOperand = newExpressoinNode.LeftHandSide.ResultType;
            CrawlType rightOperand = newExpressoinNode.RightHandSide.ResultType;
            ExpressionType oprator = newExpressoinNode.ExpressionType;
            
            CrawlType expressionTypeResult = ExpressionEvaluator.EvaluateBinaryType(leftOperand, oprator, rightOperand);
            CrawlSyntaxNode result = newExpressoinNode.WithResultType(expressionTypeResult);
            
            return result;
        }

        #endregion

        protected override CrawlSyntaxNode VisitCall(CallNode call)
        {
            CallNode result = (CallNode)base.VisitCall(call);
            CrawlType resultType;
            IEnumerable<CrawlType> givenParameters = call.Arguments.Select(x => x.Value.ResultType);

            CrawlMethodType methodSignature = call.Target.ResultType as CrawlMethodType;

            if (methodSignature == null)
                resultType = CrawlType.ErrorType;
            else
                resultType = ExpressionEvaluator.Call(methodSignature, givenParameters.ToArray());

            result = (CallNode)result.WithResultType(resultType);
            return result;
        }

        protected override CrawlSyntaxNode VisitCastExpression(CastExpressionNode castExpression)
        {
            var castExp = (CastExpressionNode) base.VisitCastExpression(castExpression);

            if (castExp.Target.ResultType.CastableTo(castExp.TypeToConvertTo.ActualType))
                return castExp.WithResultType(castExp.TypeToConvertTo.ActualType);
            else
                return castExp.WithResultType(CrawlType.ErrorType);
        }

        protected override CrawlSyntaxNode VisitMultiChildExpression(MultiChildExpressionNode multiChildExpression)
        {
            MultiChildExpressionNode newMultichildNode = (MultiChildExpressionNode)(base.VisitMultiChildExpression(multiChildExpression));
            List<CrawlType> operandsTypes = newMultichildNode.Arguments.Select(a => a.ResultType).ToList();

            CrawlType newType = ExpressionEvaluator.EvaluateMultiExpression(newMultichildNode.ExpressionType, operandsTypes);

            return newMultichildNode.WithResultType(newType); 
        }
    }
}