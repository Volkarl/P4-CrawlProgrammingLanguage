using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.Isam.Esent.Interop;

namespace CodeGeneratorDriver
{
    public class GreenNodeGenerator
    {
        public static SyntaxNode CreateGreenNode(SyntaxGenerator generator, Node node, Options options)
        {
            List<SyntaxNode> classContents = new List<SyntaxNode>();

            foreach (Member type in node.Members)
            {
                classContents.Add(CreateMember(generator, type));
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
            Options options)
        {
            List<SyntaxNode> baseArgs = null;
            List<SyntaxNode> block = node.Members.Select(x =>
                    generator.AssignmentStatement(generator.IdentifierName(x.PropertyName()),
                        generator.IdentifierName(x.ParameterName())))
                .ToList();

            block.Add(generator.AssignmentStatement(generator.IdentifierName("ChildCount"),
                generator.LiteralExpression(node.AllChildren().Count())));

            if (node.BaseNode != null)
            {
                baseArgs = node.BaseNode.AllMembers.Select(x => generator.IdentifierName(x.ParameterName())).ToList();
            }

            return generator.ConstructorDeclaration(
                null,
                node.AllMembers.Select(m => generator.ParameterDeclaration(m.ParameterName(),
                    m.GetRepresentation(TypeClassContext.GreenIEnumerableParameter))),
                Accessibility.Internal,
                baseConstructorArguments: baseArgs,
                statements: block
            );
        }

        private static SyntaxNode CreateRepaceChild(SyntaxGenerator generator, Node node, Options options)
        {
            List<SyntaxNode> cases = new List<SyntaxNode>();

            cases.AddRange(node.AllChildren()
                .Select((c, i) => generator.SwitchSection(
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

                )));

            cases.Add(generator.DefaultSwitchSection(new[]
            {
                generator.ThrowStatement(generator.ObjectCreationExpression(
                    SyntaxFactory.ParseTypeName(options.InvalidChildReplace)))
            }));


            SyntaxNode switchStatement = generator.SwitchStatement(generator.IdentifierName(options.Index),
                cases);


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
                    switchStatement
                }
            );
        }

        private static SyntaxNode CreateCreateRed(SyntaxGenerator generator, Node node, Options options)
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

        private static SyntaxNode CreateGetChild(SyntaxGenerator generator, Node node, Options options)
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

        private static SyntaxNode CreateMember(SyntaxGenerator generator, Member type)
        {
            return SharedGeneratorion.GetOnlyAccessor(type.PropertyName(), type.GetRepresentation(TypeClassContext.None));
        }
    }
}