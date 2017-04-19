using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace CodeGeneratorDriver
{
    public class GreenNodeGenerator
    {
        public static SyntaxNode CreateGreenNode(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            List<SyntaxNode> classContents = new List<SyntaxNode>();

            foreach (Child child in node.Children)
            {
                classContents.Add(CreateChild(generator, child));
            }

            SharedGeneratorion.AddLineBreak(classContents);

            foreach (Property field in node.Properties)
            {
                classContents.Add(CreateProperty(generator, field));
            }


            classContents.Add(CreateConstructor(generator, node, options));
            classContents.Add(CreateGetChild(generator, node, options));
            if(!node.Abstract)
                classContents.Add(CreateCreateRed(generator, node, options));
            classContents.Add(CreateRepaceChild(generator, node, options));



            return generator.ClassDeclaration(
                SharedGeneratorion.GreenNodeName(node.Name),
                modifiers: DeclarationModifiers.None.WithIsAbstract(node.Abstract),
                accessibility: Accessibility.Internal,
                baseType: SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(node.BaseClass)),
                members: classContents
            );

        }

        public static SyntaxNode CreateConstructor(SyntaxGenerator generator, Node node,
            SyntaxGenerationOptions options)
        {


            List<SyntaxNode> baseArgs = null;

            if (node.BaseNode != null)
            {
                baseArgs = node.BaseNode.AllProperties()
                    .Select(x => generator.Argument(generator.IdentifierName(x.Name.AsParameter())))
                    .Concat(node.BaseNode.AllChildren()
                        .Select(x => generator.Argument(generator.IdentifierName(x.Name.AsParameter()))))
                    .ToList();
            }


            return generator.ConstructorDeclaration(
                null,
                node.AllProperties()
                    .Select(p =>
                        generator.ParameterDeclaration(p.Name.AsParameter(),SyntaxFactory.ParseTypeName(p.Type == "IEnumerable<CrawlSyntaxNode>" ? "IEnumerable<GreenCrawlSyntaxNode>" : p.Type))
                    )
                    .Concat(
                        node.AllChildren()
                            .Select(p => generator.ParameterDeclaration(p.Name.AsParameter(),
                                SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(p.Type))))
                    ),
                Accessibility.Internal,
                baseConstructorArguments: baseArgs,
                    statements: node.Properties.Select(x =>
                            generator.AssignmentStatement(generator.IdentifierName(x.Name),
                                generator.IdentifierName(x.Name.AsParameter())))
                        .Concat(node.Children.Select(
                            x => generator.AssignmentStatement(generator.IdentifierName(x.Name),
                                generator.IdentifierName(x.Name.AsParameter())))));



        }

        private static SyntaxNode CreateRepaceChild(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            return generator.MethodDeclaration(
                options.WithReplacedChild,
                new[]
                {
                    generator.ParameterDeclaration(options.NewChild, options.GreenBase()),
                    generator.ParameterDeclaration(options.Index, generator.TypeExpression(SpecialType.System_Int32))
                },
                null,
                options.GreenBase(),
                Accessibility.Internal,
                DeclarationModifiers.Override,
                new[]
                {
                    generator.SwitchStatement(generator.IdentifierName(options.Index),
                        node.AllChildren().Select((c, i) => generator.SwitchSection(
                                generator.LiteralExpression(i),
                                new[]
                                {
                                    generator.ReturnStatement(
                                        generator.ObjectCreationExpression(
                                            SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(node.Name)),
                                            node.AllProperties()
                                                .Select(x => generator.Argument(generator.IdentifierName(x.Name)))
                                                .Concat(node.AllChildren()
                                                    .Select((q, z) => generator.Argument(
                                                        z == i
                                                            ? generator.CastExpression(
                                                                SyntaxFactory.ParseTypeName(
                                                                    SharedGeneratorion.GreenNodeName(q.Type)),
                                                                generator.IdentifierName(options.NewChild))
                                                            : generator.IdentifierName(q.Name)


                                                    )))
                                        )
                                    )
                                }

                            ))
                            .Concat(
                                new[]
                                {
                                    generator.DefaultSwitchSection(new[]
                                    {
                                        generator.ThrowStatement(generator.ObjectCreationExpression(
                                            SyntaxFactory.ParseTypeName(options.InvalidChildReplace)))
                                    })
                                }))
                }
            );
        }

        private static SyntaxNode CreateCreateRed(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            return generator.MethodDeclaration(
                options.CreateRed,
                new[]
                {
                    generator.ParameterDeclaration(
                        options.Parent,
                        options.RedBase()),
                    generator.ParameterDeclaration(
                        options.IndexInParent,
                        generator.TypeExpression(SpecialType.System_Int32))
                },
                null,
                options.RedBase(),
                Accessibility.Internal,
                DeclarationModifiers.Override,
                new[]
                {
                    generator.ReturnStatement(generator.ObjectCreationExpression(
                        SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)),
                        generator.IdentifierName(options.Parent),
                        generator.ThisExpression(),
                        generator.IdentifierName(options.IndexInParent)
                    ))
                }
            );
        }

        private static SyntaxNode CreateGetChild(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            return generator.MethodDeclaration(
                options.GetChildAt,
                new[]
                    {generator.ParameterDeclaration(options.Index, generator.TypeExpression(SpecialType.System_Int32))},
                null,
                options.GreenBase(),
                Accessibility.Internal,
                DeclarationModifiers.Override,
                new[]
                {
                    generator.SwitchStatement(generator.IdentifierName(options.Index),
                        node.AllChildren()
                            .Select((c, i) => generator.SwitchSection(
                                generator.LiteralExpression(i),
                                new[]
                                {
                                    generator.ReturnStatement(
                                        generator.IdentifierName(c.Name))
                                }

                            ))
                            .Concat(
                                new[]
                                {
                                    generator.DefaultSwitchSection(new[]
                                    {
                                        generator.ReturnStatement(generator.DefaultExpression(
                                            options.GreenBase())
                                        )
                                    })
                                }))
                });
        }

        private static SyntaxNode CreateChild(SyntaxGenerator generator, Child child)
        {
            return SharedGeneratorion.GetOnlyAccessor(child.Name, SharedGeneratorion.GreenNodeName(child.Type));
        }

        private static SyntaxNode CreateProperty(SyntaxGenerator generator, Property property)
        {
            return SharedGeneratorion.GetOnlyAccessor(property.Name, property.Type);
        }
    }
}