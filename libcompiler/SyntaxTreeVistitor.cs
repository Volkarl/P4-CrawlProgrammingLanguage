using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcompiler
{
    public abstract class SyntaxTreeVistitor
    {
        public virtual CrawlSyntaxNode Visit(CrawlSyntaxNode node)
        {
            //  s/\.([^:]*):/.$1:\n\t\t\t\t\treturn Visit$1(($1Node) node);/
            switch (node.Type)
            {
                case NodeType.Literal:
                    return VisitLiteral((LiteralNode) node);
                case NodeType.CompilationUnit:
                    return VisitCompilationUnit((CompiliationUnitNode) node);
                case NodeType.Expression:
				    return VisitExpression((ExpressionNode) node);
                case NodeType.MemberAccess:
				    return VisitMemberAccess((MemberAccessNode) node);
                case NodeType.Index:
				    return VisitIndex((IndexExpression) node);
                case NodeType.Call:
				    return VisitCall((InvocationExpression) node);
                case NodeType.Decleration:
				    return VisitDecleration((DeclerationNode) node);
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual CrawlSyntaxNode VisitDecleration(DeclerationNode node)
        {
            return node; //TODO: TYPE
        }

        protected virtual CrawlSyntaxNode VisitCall(InvocationExpression node)
        {
            return new InvocationExpression((ValueNode) Visit(node.Method), node.Arguments.Select(Visit).Cast<ValueNode>().ToList());
        }

        protected virtual CrawlSyntaxNode VisitIndex(IndexExpression node)
        {
            return new IndexExpression(node.Arguments.Select(Visit).Cast<ValueNode>().ToList());
        }

        protected virtual CrawlSyntaxNode VisitMemberAccess(MemberAccessNode node)
        {
            return new MemberAccessNode((ValueNode) Visit(node.Parent), node.Subfield);
        }

        protected virtual CrawlSyntaxNode VisitExpression(ExpressionNode node)
        {
            return new ExpressionNode(node.ExpressionType, node.Children.Select(Visit).ToList());
        }

        protected virtual CrawlSyntaxNode VisitCompilationUnit(CompiliationUnitNode node)
        {
            return new CompiliationUnitNode(node.Children.Select(Visit).ToList());
        }

        protected virtual CrawlSyntaxNode VisitLiteral(LiteralNode node)
        {
            return new LiteralNode(node.Node);
        }
    }
}
