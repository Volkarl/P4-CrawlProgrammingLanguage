using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace CodeGeneratorDriver
{
    class RedNodeGenerator
    {

        public static SyntaxNode CreateRedNode(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            List<SyntaxNode> members = new List<SyntaxNode>();

            members.AddRange(
                node.Children.Select(
                    x =>
                        generator.FieldDeclaration("_" + x.Name.AsParameter(),
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(x.Type)),
                            Accessibility.Private)));

            int parrentChildCount = node.BaseNode?.AllChildren()?.Count() ?? 0;
            members.AddRange(
                node.Children.Select(
                    (x, i) =>
                        generator.PropertyDeclaration(x.Name,
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(x.Type)), Accessibility.Public,
                            DeclarationModifiers.ReadOnly, CreateGetter(generator, x, i, parrentChildCount, options))));

            members.AddRange(node.Properties.Select(x => SharedGeneratorion.GetOnlyAccessor(x.Name, x.Type)));

            members.Add(CreateCtor(generator, node, options));

            members.Add(CreateGetChildAt(generator, node, options));

            return generator.ClassDeclaration(
                SharedGeneratorion.RedNodeName(node.Name),
                null,
                Accessibility.Public,
                DeclarationModifiers.Partial,
                SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.BaseClass)),
                null,
                members
            );
        }

        private static SyntaxNode CreateGetChildAt(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            return generator.MethodDeclaration(options.GetChildAt,
                new[]
                {
                    generator.ParameterDeclaration(options.Index, generator.TypeExpression(SpecialType.System_Int32))
                },
                null,
                options.RedBase(),
                Accessibility.Public,
                DeclarationModifiers.Override,
                new[]
                {
                    generator.SwitchStatement(generator.IdentifierName(options.Index),
                        node.AllChildren()
                            .Select(
                                (x, i) =>
                                    generator.SwitchSection(generator.LiteralExpression(i),
                                        new[] {generator.ReturnStatement(generator.IdentifierName(x.Name))}))),
                    generator.ReturnStatement(generator.DefaultExpression(options.RedBase()))
                });
        }

        private static SyntaxNode CreateCtor(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            return generator.ConstructorDeclaration(
                null,
                new[]
                {
                    generator.ParameterDeclaration(options.Parent, options.RedBase()),
                    generator.ParameterDeclaration(options.Self,
                        SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(node.Name))),
                    generator.ParameterDeclaration(options.IndexInParent,
                        generator.TypeExpression(SpecialType.System_Int32))
                },
                Accessibility.Public,
                DeclarationModifiers.None,
                new[]
                {
                    generator.Argument(generator.IdentifierName(options.Parent)),
                    generator.Argument(generator.IdentifierName(options.Self)),
                    generator.Argument(generator.IdentifierName(options.IndexInParent)),

                },
                node.Properties.Select(
                    x =>
                        generator.AssignmentStatement(generator.IdentifierName(x.Name),
                            generator.MemberAccessExpression(generator.IdentifierName(options.Self), x.Name)))

            );
        }

        private static IEnumerable<SyntaxNode> CreateGetter(SyntaxGenerator generator, Child child, int index, int parrentChildCount, SyntaxGenerationOptions options)
        {
            yield return
                generator.ReturnStatement(generator.InvocationExpression(generator.IdentifierName(options.GetRed),
                    generator.Argument(RefKind.Ref, generator.IdentifierName("_" + child.Name.AsParameter())),
                    generator.Argument(generator.LiteralExpression(index+parrentChildCount))));
        }
    }
}
