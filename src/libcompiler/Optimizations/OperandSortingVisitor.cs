using System.Collections.Generic;
using System.Linq;
using libcompiler.SyntaxTree;

namespace libcompiler.Optimizations
{
    /// <summary>
    /// Sorts the operands of commotative multichildexpressions, so that literals come first.
    /// </summary>
    public class OperandSortingVisitor :SyntaxRewriter
    {
        protected override CrawlSyntaxNode VisitMultiChildExpression(MultiChildExpressionNode multiChildExpression)
        {
            var expr = (MultiChildExpressionNode)base.VisitMultiChildExpression(multiChildExpression);

            if (!(expr.ExpressionType == ExpressionType.Add ||
                  expr.ExpressionType == ExpressionType.Multiply ||
                  expr.ExpressionType == ExpressionType.ShortCircuitAnd ||
                  expr.ExpressionType == ExpressionType.ShortCircuitOr))
                return expr;

            List<ExpressionNode> literals = new List<ExpressionNode>();
            List<ExpressionNode> restOfIt = new List<ExpressionNode>();

            foreach (ExpressionNode expression in expr.Arguments)
            {
                var literal = expression as LiteralNode;

                if (literal != null)
                    literals.Add(literal);
                else
                    restOfIt.Add(expression);
            }

            List<ExpressionNode> newArguments = literals.Concat(restOfIt).ToList();

            return CrawlSyntaxNode.MultiChildExpression(expr.Interval, expr.ExpressionType, expr.ResultType,
                newArguments);
        }
    }
}