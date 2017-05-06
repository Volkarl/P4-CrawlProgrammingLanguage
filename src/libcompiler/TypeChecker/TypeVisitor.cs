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

            BinaryExpressionNode expressionNodeThatWeAreChanging = (BinaryExpressionNode) (base.VisitBinaryExpression(binaryExpression) );
            CrawlType leftOperand = expressionNodeThatWeAreChanging.LeftHandSide.ResultType;
            CrawlType rightOperand = expressionNodeThatWeAreChanging.RightHandSide.ResultType;
            ExpressionType oprator = expressionNodeThatWeAreChanging.ExpressionType;
            
            CrawlType theResultTypeThatWeWantOurExpressionNodeToHave = ExpressionEvaluator.EvaluateBinaryType(leftOperand, oprator, rightOperand);
            CrawlSyntaxNode result = expressionNodeThatWeAreChanging.WithResultType(theResultTypeThatWeWantOurExpressionNodeToHave);
            return result; 


        }

        #endregion

    }
}