using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Antlr4.Runtime;
using libcompiler.Scope;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;
using static libcompiler.TypeChecker.ExpressionEvaluator;

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

        #region variables
        protected override CrawlSyntaxNode VisitSingleVariableDecleration(SingleVariableDeclerationNode singleVariableDecleration)
        {
            var node = (SingleVariableDeclerationNode) base.VisitSingleVariableDecleration(singleVariableDecleration);
            CrawlType resultType = node.DefaultValue?.ResultType ?? node.Identifier.ResultType;
            VariableNode newIdentifier = (VariableNode) node.Identifier.WithResultType(resultType);
            return node.WithIdentifier(newIdentifier);
        }

        protected override CrawlSyntaxNode VisitVariableDecleration(VariableDeclerationNode variableDecleration)
        {
            var node = (VariableDeclerationNode) base.VisitVariableDecleration(variableDecleration);

            //The type of each of the SingleVariableDeclarationNodes.
            List<CrawlType> singleVarDclTypes = node.Declerations
                .Select(x => x.Identifier.ResultType).ToList();

            foreach (CrawlType type in singleVarDclTypes)
            {
                //If type given to variable doesn't match declaration type, return node with declaration type set to error intead.
                if (!type.Equals(node.DeclerationType.ActualType))
                {
                    TypeNode declarationError = node.DeclerationType.WithActualType(CrawlType.ErrorType);
                    return node.WithDeclerationType(declarationError);
                }
            }

            return node;
        }

        protected override CrawlSyntaxNode VisitVariable(VariableNode variable)
        {
            var expr = (VariableNode)base.VisitVariable(variable);
            CrawlType resultType = expr.FindFirstScope().FindSymbol(variable.Name)?.FirstOrDefault()?.Type;

            var result = expr.WithResultType(resultType ?? CrawlType.ErrorType);
            return result;
        }

        protected override CrawlSyntaxNode VisitMemberAccess(MemberAccessNode memberAccess)
        {
            var expr = (MemberAccessNode) base.VisitMemberAccess(memberAccess);
            CrawlType resultType = expr.Target.ResultType.FindSymbol(expr.Member.Value)[0].Type;
            return expr.WithResultType(resultType);
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
                        UnaryNegate(expr.Target.ResultType)
                    );
                    break;

                case ExpressionType.Not:
                    result = expr.WithResultType(
                        UnaryNot(expr.Target.ResultType)
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
            
            CrawlType expressionTypeResult = EvaluateBinaryType(leftOperand, oprator, rightOperand);
            CrawlSyntaxNode result = newExpressoinNode.WithResultType(expressionTypeResult);
            
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

        protected override CrawlSyntaxNode VisitCall(CallNode call)
        {
            CallNode expr = (CallNode)base.VisitCall(call);
            CrawlType resultType = CrawlType.ErrorType;
            List<CrawlType> actualParameters = expr.Arguments.Select(x => x.Value.ResultType).ToList();

            //Three posibilities exist:

            //Method type is provided as a method name. Find candidates and choose best fit.
            VariableNode asVar = expr.Target as VariableNode;
            if (asVar != null)
            {
                var foo = asVar
                    .FindFirstScope();
                var bar = foo
                    .FindSymbol(asVar.Name);
                List<CrawlMethodType> candidates = bar
                    .Select(x=>(CrawlMethodType)x.Type)
                    .ToList();

                resultType = BestParameterMatch(candidates, actualParameters);
            }

            //Method type is provided as method name in specific scope.
            MemberAccessNode asMem = expr.Target as MemberAccessNode;
            if (asMem != null)
            {
                List<CrawlMethodType> candidates = asMem
                    .Target.ResultType
                    .FindSymbol(asMem.Member.Value)
                    .Select(x=>(CrawlMethodType)x.Type)
                    .ToList();

                resultType = BestParameterMatch(candidates, actualParameters);
            }

            //Method type is provided by some other expression. In this case it either matches or does not.
            else
            {
                CrawlMethodType methodSignature = expr.Target.ResultType as CrawlMethodType;

                if (methodSignature != null)
                    resultType = Call(methodSignature, actualParameters);
            }

            return expr.WithResultType(resultType);
        }

        #endregion

    }
}