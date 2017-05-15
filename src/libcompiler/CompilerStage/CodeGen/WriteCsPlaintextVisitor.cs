using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using libcompiler.ExtensionMethods;
using libcompiler.Parser;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.CompilerStage.CodeGen
{
    public class WriteCsPlaintextVisitor : SimpleSyntaxVisitor<string>
    {
        protected override string VisitTranslationUnit(TranslationUnitNode node)
        {
            bool hasNamespace = !string.IsNullOrWhiteSpace(node.Namespace?.Module);
            StringBuilder sb = new StringBuilder();

            foreach (ImportNode import in node.Imports)
            {
                //Don't generate import for the implicit module
                if(string.IsNullOrWhiteSpace(import.Module)) continue;
                
                sb.Append("using ");
                sb.Append(import.Module);
                sb.Append(";\n");
            }

            if (hasNamespace)
            {
                sb.Append("namespace ");
                sb.Append(node.Namespace.Module);
                sb.Append("\n{");
            }


            sb.Append(Visit(node.Code));

            if (hasNamespace)
                sb.Append("}");

            return sb.ToString();
        }

        protected override string VisitClassTypeDecleration(ClassTypeDeclerationNode node)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(node.ProtectionLevel.AsCSharpString());
            sb.Append(" class ");
            sb.Append(node.Identifier.Value);

            sb.Append(" ");
            sb.Append(node.ClassType.FullName);

            sb.Append("{");
            sb.Append(Visit(node.Body));
            sb.Append("}");
            
            return sb.ToString();
        }

        protected override string VisitMethodDecleration(MethodDeclerationNode node)
        {
            StringBuilder sb = new StringBuilder();


            sb.Append(node.ProtectionLevel.AsCSharpString());
            sb.Append(" ");

            CrawlMethodType signature = node.MethodSignature.ActualType as CrawlMethodType;
            if (signature == null) throw new Exception();

            if (signature.ReturnType.Equals(CrawlSimpleType.Intet))
                sb.Append("void");
            else
                sb.Append(signature.ReturnType.FullName);
            sb.Append(" ");

            sb.Append(node.Identifier.Value);
            sb.Append(" (");

            sb.Append(string.Join(", ",
                node.Parameters.Select(
                    x => $"{(x.Reference ? "ref" : "")}{x.ParameterType.ActualType.FullName} {x.Identifier.Value}")));

            sb.Append(")\n{\n");

            sb.Append(Visit(node.Body));

            sb.Append("}\n");

            return sb.ToString();
        }



        string VisitAndAddDelimiters<T>(ListNode<T> arguments, string delimiter) where T : CrawlSyntaxNode
        {
            var sb = new StringBuilder();
            for (int i = 0; i < arguments.Count(); i++)
            {
                if (i != 0) sb.Append(delimiter);
                sb.Append(Visit(arguments[i]));
            }
            return sb.ToString();
        }

        protected override string Combine(params string[] parts)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string part in parts)
            {
                sb.Append(part);
            }
            return sb.ToString();
        }

        protected override string VisitBlock(BlockNode block)
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (string child in block.Select(Visit))
            {
                sb.Append($"{child}\n");
            }
            
            return sb.ToString();
        }

        protected override string VisitVariableDecleration(VariableDeclerationNode node)
        {
            string type = Visit(node.DeclerationType);
            string decls = Visit(node.Declerations);
            StringBuilder sb = new StringBuilder();

            foreach (string decl in decls.Split(','))
            {
                if(decl.Length>0)
                    sb.Append($"{type} {decl};\n\n");
            }

            return sb.ToString();
        }

        protected override string VisitType(TypeNode node)
        {
            return "@" + node.ActualType.Identifier;
        }

        protected override string VisitIdentifier(IdentifierNode node)
        {
            return "@" +  node.Value;
        }

        protected override string VisitBooleanLiteral(BooleanLiteralNode node)
        {
            return node.Value.ToString();
        }

        protected override string VisitStringLiteral(StringLiteralNode node)
        {
            return node.Value;
        }

        protected override string VisitRealLiteral(RealLiteralNode node)
        {
            return node.Value.ToString(CultureInfo.GetCultureInfo("en-GB"));
        }

        protected override string VisitSingleVariableDecleration(SingleVariableDeclerationNode node)
        {
            string identifier = Visit(node.Identifier);
            if (node.DefaultValue != null)
            {
                string defaultValue = Visit(node.DefaultValue);
                return $"{identifier} = {defaultValue},";
            }
            return $"{identifier},";
        }

        protected override string VisitVariable(VariableNode node)
        {
            return node.Name;
        }

        protected override string VisitIntegerLiteral(IntegerLiteralNode node)
        {
            return node.Value.ToString();
        }

        protected override string VisitMultiChildExpression(MultiChildExpressionNode node)
        {
            string delimiter;
            switch (node.ExpressionType)
            {
                case ExpressionType.Add:
                    delimiter = " + ";
                    break;
                case ExpressionType.Subtract:
                    delimiter = " - ";
                    break;
                case ExpressionType.Divide:
                    delimiter = " / ";
                    break;
                case ExpressionType.Multiply:
                    delimiter = " * ";
                    break;
                case ExpressionType.ShortCircuitOr:
                    delimiter = " || ";
                    break;
                case ExpressionType.ShortCircuitAnd:
                    delimiter = " && ";
                    break;
                
                case ExpressionType.Power:
                    return WritePowerExpression(node.Arguments.ToList());
                default: throw new ArgumentException("MultiChildExpression expressionType " + node.ExpressionType + " not supported");
            }
            return VisitAndAddDelimiters(node.Arguments, delimiter);
        }

        private string WritePowerExpression(List<ExpressionNode> arguments /*, int recursion = 0*/)
        {
            return InsertVisitsRecursivelyUntilTwoRemain(arguments, "System.Math.Pow(", ", ", ")");

            #region DeprecatedCode
            //if (arguments.Count - recursion == 2)
            //{
            //    // If there are only two left, then the recursion is done
            //    return $"System.Math.Pow({Visit(arguments[recursion])}, {Visit(arguments[recursion + 1])})";
            //}
            //return $"System.Math.Pow({Visit(arguments[recursion])}, {WritePowerExpression(arguments, recursion + 1)})";
            #endregion
        }

        private string InsertVisitsRecursivelyUntilTwoRemain(List<ExpressionNode> arguments,
            string front = "", string middle = "", string end = "", bool rightRecursion = true, int currentRecursion = 0)
        {
            // A bit tought to understand, so take a look at the deprecated code in the WritePowerExpression method in stead:
            // This is simply a more widely applicable version of the same concept.

            if (arguments.Count - currentRecursion == 2)
            {
                // If there are only two arguments left, then the recursion is done
                return front + Visit(arguments[currentRecursion]) + middle + Visit(arguments[currentRecursion + 1]) + end;
            }
            if(rightRecursion)
                return front + Visit(arguments[currentRecursion]) + middle + 
                    InsertVisitsRecursivelyUntilTwoRemain(arguments, front, middle, end, rightRecursion, currentRecursion + 1) + end;
            // Otherwise its a left recursion, so we reverse the order
            return front + InsertVisitsRecursivelyUntilTwoRemain(arguments, front, middle, end, rightRecursion, currentRecursion + 1)
                + middle + Visit(arguments[currentRecursion]) + end;
        }

        protected override string VisitCall(CallNode node)
        {
            string target = Visit(node.Target);
            string arg = VisitAndAddDelimiters(node.Arguments, ", ");
            return $"{target}({arg})";
        }

        protected override string VisitCastExpression(CastExpressionNode node)
        {
            return $"({Visit(node.TypeToConvertTo)}) {Visit(node.Target)}";
        }

        protected override string VisitArrayConstructor(ArrayConstructorNode node)
        {
            // Gets the type and writes all arguments into the array, for example:
            // Array tal[,,][][,] and arguments (1,2,3,4,5,6) becomes new tal[1,2,3][4][5,6];
            char lBracket = '[';
            char rBracket = ']';
            char comma = ',';

            string type = Visit(node.Target);
            string identifier = type.TrimEnd(lBracket, rBracket, comma);
            int arrayIndex = identifier.Length; // Index where array definition starts

            int dimensions = type.Substring(arrayIndex).Count(c => c == lBracket || c == comma);
            if (dimensions != node.Arguments.Count())
                throw new ArgumentException($"Argument count {node.Arguments.Count()} does not match array definition {type.Substring(arrayIndex)}, which requires {dimensions} arguments.");

            for (int argumentNr = 0; argumentNr < node.Arguments.Count();)
            {
                if (type[arrayIndex] != lBracket)
                    throw new ArgumentException("Expected: " + lBracket);
                string argLBracket = Visit(node.Arguments[argumentNr++]);
                type = type.Insert(++arrayIndex, argLBracket);
                arrayIndex += argLBracket.Length;

                while (type[arrayIndex] == comma)
                {
                    string argComma = Visit(node.Arguments[argumentNr++]);
                    type = type.Insert(++arrayIndex, argComma);
                    arrayIndex += argComma.Length;
                }

                if (type[arrayIndex++] != rBracket)
                    throw new ArgumentException($"Expected {rBracket}, found {type[arrayIndex]}");
            }

            if (arrayIndex != type.Length)
                throw new ArgumentException($"Argument count {node.Arguments.Count()} does not match array definition {type.Substring(arrayIndex)}");

            // This method may be quite brittle, because it appears possible to get indexOutOfRangeExceptions everywhere. I'm leaving it 
            // as it is though, because it gets so much harder to read otherwise, and I'm not sure its even possible to invoke those errors.

            return "new " + type;
        }

        protected override string VisitArgument(ArgumentNode node)
        {
            string sref = node.Refence ? "ref" : String.Empty;
            return $"{sref} {Visit(node.Value)}";
        }

        protected override string VisitAssignment(AssignmentNode node)
        {
            string target = Visit(node.Target);
            string val = Visit(node.Value);
            return $"{target} = {val};";
        }

        protected override string VisitMemberAccess(MemberAccessNode node)
        {
            return $"{ Visit(node.Target)}.@{node.Member.Value}";
        }


        protected override string VisitUnaryExpression(UnaryExpressionNode node)
        {
            switch (node.ExpressionType)
            {
                case ExpressionType.Not: return "!" + Visit(node.Target);
                case ExpressionType.Negate: return "-" + Visit(node.Target);
                default: throw new ArgumentException("Weird unary expression type: " + node.ExpressionType);
            }
        }

        protected override string VisitBinaryExpression(BinaryExpressionNode node)
        {
            string delimiter;
            switch (node.ExpressionType)
            {
                case ExpressionType.Range:
                    return $"System.Enumerable.Range({Visit(node.LeftHandSide)},{Visit(node.RightHandSide)})";

                case ExpressionType.Greater:
                    delimiter = ">";
                    break;
                case ExpressionType.GreaterEqual:
                    delimiter = ">=";
                    break;
                case ExpressionType.Less:
                    delimiter = "<";
                    break;
                case ExpressionType.LessEqual:
                    delimiter = "<=";
                    break;
                case ExpressionType.Equal:
                    delimiter = "==";
                    break;
                case ExpressionType.NotEqual:
                    delimiter = "!=";
                    break;
                case ExpressionType.Add: // Can these two ever show up? They are in the BinaryExpressionDict, so I've included them here too.
                    delimiter = "+";
                    break;
                case ExpressionType.Subtract:
                    delimiter = "-";
                    break;
                default: throw new ArgumentException("BinaryExpression expressionType " + node.ExpressionType + " not supported");
            }
            return Visit(node.LeftHandSide) + delimiter + Visit(node.RightHandSide);
        }

        protected override string VisitWhile(WhileNode node)
        {
            return $"while({Visit(node.Condition)})" + Visit(node.Statementes).Indent().SurroundWithBrackets();
        }

        protected override string VisitForLoop(ForLoopNode node)
        {
            return $"foreach({Visit(node.Loopvariable)} {Visit(node.LoopVariable)} in {Visit(node.Iterator)})" + Visit(node.Body).Indent().SurroundWithBrackets();
        }
    }
}