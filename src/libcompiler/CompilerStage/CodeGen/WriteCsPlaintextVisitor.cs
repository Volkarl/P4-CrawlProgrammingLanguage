using System;
using System.Linq;
using System.Text;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage.CodeGen
{
    public class WriteCsPlaintextVisitor : SimpleSyntaxVisitor<string>
    {
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
                sb.Append($"\n{child}\n");
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
            return node.ActualType.Identifier;
        }

        protected override string VisitSingleVariableDecleration(SingleVariableDeclerationNode node)
        {
            string identifier = Visit(node.Identifier);
            if (node.DefaultValue != null)
            {
                string defaultValue = Visit(node.DefaultValue);
                return $"{identifier} = {defaultValue},";
            }
            else
            {
                return $"{identifier},";
            }
            return base.VisitSingleVariableDecleration(node);
        }

        protected override string VisitVariable(VariableNode node)
        {
            return node.Name;
        }

        protected override string VisitIntegerLiteral(IntegerLiteralNode node)
        {
            if (node.Value == 1)
                return "1";
            else
                return "EXPRESSION_ERR ";
        }

        protected override string VisitMultiChildExpression(MultiChildExpressionNode node)
        {
            if(node.ExpressionType != ExpressionType.Add)
                return "OPERATOR_ERR ";

            StringBuilder sb = new StringBuilder();
            foreach (ExpressionNode expressionNode in node.Arguments)
            {
                string expr = Visit(expressionNode);
                sb.Append($"{expr}+");
            }
            sb.Append("0");

            return sb.ToString();
        }
    }
}