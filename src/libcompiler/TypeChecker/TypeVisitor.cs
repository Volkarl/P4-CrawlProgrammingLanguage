using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Antlr4.Runtime;
using libcompiler.CompilerStage;
using libcompiler.Scope;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;
using static libcompiler.TypeChecker.ExpressionEvaluator;

namespace libcompiler.TypeChecker
{
    class TypeVisitor : SyntaxRewriter
    {
        private ConcurrentBag<CompilationMessage> _messages;
        private AstData _data;


        public TypeVisitor(ConcurrentBag<CompilationMessage> messages, AstData data)
        {
            _messages = messages;
            _data = data;
        }

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
            if (expr.Target.ResultType is CrawlStatusType)
            {
                return expr.WithResultType(expr.Target.ResultType);
            }
            var results = expr.Target.ResultType.FindSymbol(expr.Member.Value);
            if (results == null || results.Length == 0)
                return expr.WithResultType(CrawlType.ErrorType);

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


        #region Call helpers
        private IEnumerable<CrawlMethodType> FindCallCandidates(ExpressionNode target)
        {
            //Three posibilities exist:


            //Method type is provided as method name in specific scope.
            MemberAccessNode asMem = target as MemberAccessNode;
            //Method type is provided as a method name. Find candidates and choose best fit.
            VariableNode asVar = target as VariableNode;

            TypeInformation[] tif;

            //If a signal, we cant do anything
            if (target.ResultType is CrawlStatusType)
            {
                yield break; //Nothing to do...
            }
            //If member or variable, find everything.
            else if (asVar != null)
            {
                IScope variableScope = asVar
                    .FindFirstScope();
                tif = variableScope.FindSymbol(asVar.Name);
            }
            else if (asMem != null)
            {
                tif = asMem.Target.ResultType.FindSymbol(asMem.Member.Value);
            }
            //Method type is provided by some other expression. Pass it on and exit
            else
            {
                CrawlMethodType methodSignature = target.ResultType as CrawlMethodType;

                if (methodSignature != null)
                {
                    yield return methodSignature;
                }

                yield break;
            }

            //Return everything that member or variable provided
            foreach (CrawlMethodType methodType in tif.SelectMany(GetMethodTypesFromTypeInformation))
            {
                yield return methodType;
            }
        }

        IEnumerable<CrawlMethodType> GetMethodTypesFromTypeInformation(TypeInformation tif)
        {
            if (tif.NeedsABetterNameType == NeedsABetterNameType.Class)
            {
                CrawlConstructedType ctype = (CrawlConstructedType) tif.Type;
                TypeInformation[] constructors = ctype.FindSymbol(".ctor");
                foreach (TypeInformation constructor in constructors)
                {
                    yield return (CrawlMethodType) constructor.Type;
                }
            }
            else
            {
                yield return (CrawlMethodType) tif.Type;
            }
        }


        #endregion



        protected override CrawlSyntaxNode VisitCall(CallNode call)
        {
            //Counter for how far we got before we got errors
            int stage = 0;
            CrawlType resultType;
            CallNode expr = (CallNode)base.VisitCall(call);

            List<CrawlType> actualParameters = expr.Arguments.Select(x => x.Value.ResultType).ToList();

            //Find every method that could fit in here.
            List<CrawlMethodType> candidates =
                FindCallCandidates(expr.Target).ToList();

            if (candidates.Count <= 0)
            {
                _messages.Add(CompilationMessage.Create(_data.TokenStream, expr.Target.Interval, MessageCode.NotMethod,
                    _data.Filename, "Could not finding anything looking like a method or constuctor"));
                return expr.WithResultType(CrawlType.ErrorType);
            }

            //Narrow down to fitting parameter counts
            candidates = candidates.Where(x => x.Parameters.Count == actualParameters.Count).ToList();
            if (candidates.Count <= 0)
            {
                _messages.Add(CompilationMessage.Create(_data.TokenStream, expr.Target.Interval, MessageCode.InvalidParameterCount,
                    _data.Filename, "There was no method signature matching the number of parameters"));
                return expr.WithResultType(CrawlType.ErrorType);
            }


            CrawlType result = BestParameterMatch(candidates, actualParameters);
            if (result is CrawlStatusType)
            {
                _messages.Add(CompilationMessage.Create(_data.TokenStream, expr.Target.Interval, MessageCode.InvalidParameterCount,
                    _data.Filename, "No version matches calling parameters"));
                return expr.WithResultType(result);
            }

            CrawlMethodType resultMethod = (CrawlMethodType) result;
            return expr.Update(expr.Interval, resultMethod.ReturnType, resultMethod, expr.Target, expr.Arguments);
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