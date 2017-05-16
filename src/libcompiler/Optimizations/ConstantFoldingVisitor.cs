using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using libcompiler.SyntaxTree;

namespace libcompiler.Optimizations
{
    /// <summary>
    /// Replaces expressionos that consists of only literal nodes with the result of those expressions.
    /// This visitor must be called more than once, until no more optimizations were made.
    /// That is 1+2*3==5 is eventually replaced with true.
    /// </summary>
    public partial class ConstantFoldingVisitor : SyntaxRewriter
    {
        //warning:
        //In this visitor, nodes may be replaced by other nodes of a different type.

        /// <summary>
        /// Whether the tree has been changed by this visitor.
        /// If changes were made, there may be more optimizations to do after it returns.
        /// </summary>
        public bool OptimizationsWereMade = false;



        protected override CrawlSyntaxNode VisitUnaryExpression(UnaryExpressionNode unaryExpression)
        {
            CrawlSyntaxNode expr = base.VisitUnaryExpression(unaryExpression);
            UnaryExpressionNode unaryExpr = expr as UnaryExpressionNode;
            if (unaryExpr == null)
                return expr;

            OptimizationsWereMade = true;

            ExpressionNode tar = unaryExpr.Target;

            return FoldUnary(tar, unaryExpression.ExpressionType);
        }

        protected override CrawlSyntaxNode VisitBinaryExpression(BinaryExpressionNode binaryExpression)
        {
            CrawlSyntaxNode expr = base.VisitBinaryExpression(binaryExpression);
            BinaryExpressionNode binaryExpr = expr as BinaryExpressionNode;
            if (binaryExpr == null)
                return expr;

            LiteralNode lhs = binaryExpr.LeftHandSide as LiteralNode;
            LiteralNode rhs = binaryExpr.RightHandSide as LiteralNode;
            if (lhs == null || rhs == null || binaryExpr.ExpressionType == ExpressionType.Range)
                return expr;

            OptimizationsWereMade = true;
            return FoldBinary(lhs, binaryExpr.ExpressionType, rhs);
        }

        protected override CrawlSyntaxNode VisitMultiChildExpression(MultiChildExpressionNode multiChildExpression)
        {
            CrawlSyntaxNode expr = base.VisitMultiChildExpression(multiChildExpression);
            MultiChildExpressionNode multuExpr = expr as MultiChildExpressionNode;
            if (multuExpr == null)
                return expr;

            if (multuExpr.Arguments.ChildCount == 1)
            {
                OptimizationsWereMade = true;
                return multuExpr.Arguments.First();
            }

            List<ExpressionNode> newArguments = new List<ExpressionNode>();
            ListNode<ExpressionNode> arguments = multuExpr.Arguments;


            //Algorithm ahead. Strap in.
            //Go through the node's arguments, building list of new arguments,
            //where any pair of neighboring literals have been folded into one.
            int i = 0;
            while (i < arguments.Count())
            {
                //If this is the last in the sequence, there is nothing left to fold.
                if (i == arguments.Count() - 1)
                {
                    newArguments.Add(arguments[i]);
                    break;
                }

                //Only literal nodes can be folded.
                LiteralNode current = arguments[i] as LiteralNode;
                if (current != null)
                {
                    //If both the current node and the next node is a literal, then
                    //fold them, add them to the new list,
                    //and jump to the argument after them.
                    LiteralNode next = arguments[i + 1] as LiteralNode;
                    if (next != null)
                    {
                        newArguments.Add(FoldMultuPair(current, multuExpr.ExpressionType, next));
                        OptimizationsWereMade = true;
                        i += 2;
                    }
                    //If the next argument is not a literal, it cannot be folded.
                    //move on to the argument after that.
                    else
                    {
                        newArguments.Add(arguments[i]);
                        newArguments.Add(arguments[i + 1]);
                        i += 2;
                    }
                }
                //If the current node isn't a literal,
                //move on to consider the next
                else
                {
                    newArguments.Add(arguments[i]);
                    i += 1;
                }
            }

            return CrawlSyntaxNode.MultiChildExpression(multuExpr.Interval, multuExpr.ExpressionType,
                multuExpr.ResultType, newArguments);
        }
    }
}