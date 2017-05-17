using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using libcompiler.Datatypes;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.CompilerStage
{
    //Moves statements directly in the Translation Unit into a method. Might do more transformations in the future
    class MoveDeclerationsRewriter : SyntaxRewriter
    {
        protected override CrawlSyntaxNode VisitTranslationUnit(TranslationUnitNode translationUnit)
        {
            var imports = (ListNode<ImportNode>)(Visit(translationUnit.Imports));
            var @namespace = (NamespaceNode)(Visit(translationUnit.Namespace));
            var code = VisitMainCodeBlock(translationUnit.Code);
            return translationUnit.Update(translationUnit.Interval, translationUnit.ImportedNamespaces, imports, @namespace, code);
        }

        private BlockNode VisitMainCodeBlock(BlockNode block)
        {
            List<CrawlSyntaxNode> newcontents = new List<CrawlSyntaxNode>();
            List<CrawlSyntaxNode> implicitMain = new List<CrawlSyntaxNode>();

            foreach (CrawlSyntaxNode node in block)
            {
                switch (node.Type)
                {
                    case NodeType.MethodDecleration:
                    case NodeType.ClassTypeDecleration:
                        newcontents.Add(node);
                        break;

                    case NodeType.VariableDecleration:
                        CreateLocalInMain((VariableDeclerationNode) node, newcontents, implicitMain);
                        break;

                    case NodeType.Assignment:
                    case NodeType.While:
                    case NodeType.ForLoop:
                    case NodeType.If:
                    case NodeType.IfElse:
                    case NodeType.Call:
                        implicitMain.Add(node);
                        break;

                    default:
                        throw new NotImplementedException(node.Type.ToString());
                }
            }

            if (implicitMain.Count != 0)
            {
                newcontents.Add(CrawlSyntaxNode.MethodDecleration(
                    Interval.Invalid,
                    ProtectionLevel.Private,
                    null,
                    CrawlSyntaxNode.TypeNode(Interval.Invalid, "intet()", new CrawlMethodType(CrawlSimpleType.Intet, new CrawlType[0])),
                    new List<ParameterNode>(),
                    CrawlSyntaxNode.Block(Interval.Invalid, implicitMain),
                    CrawlSyntaxNode.Identifier(Interval.Invalid, "Main"),
                    new List<GenericParameterNode>()));
            }

            return block.Update(block.Interval, newcontents, block.Scope);
        }

        private void CreateLocalInMain(VariableDeclerationNode node, List<CrawlSyntaxNode> newcontents, List<CrawlSyntaxNode> implicitMain)
        {
            VariableDeclerationNode newDecleration = CrawlSyntaxNode.VariableDecleration(
                node.Interval,
                node.ProtectionLevel,
                node.DeclerationType,
                node.Declerations
                    .Select(n => CrawlSyntaxNode.SingleVariableDecleration(n.Interval, n.Identifier)));

            foreach (SingleVariableDeclerationNode decleration in node.Declerations)
            {
                if (decleration.DefaultValue != null)
                {
                    implicitMain.Add(CrawlSyntaxNode.Assignment(decleration.Interval, decleration.Identifier, decleration.DefaultValue));
                }
            }

            newcontents.Add(newDecleration);
        }
    }
}
