using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace CodeGeneratorDriver
{
    class RedNodeGenerator
    {

        public static SyntaxNode CreateRedNode(SyntaxGenerator generator, Node node, Options options)
        {
            List<SyntaxNode> members = new List<SyntaxNode>();

            if (!string.IsNullOrWhiteSpace(node.ExtraClassCode))
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText("class f{ " + node.ExtraClassCode + " }");
                var cu = (CompilationUnitSyntax)tree.GetRoot();
                var @class = (ClassDeclarationSyntax)cu.Members[0];
                members.AddRange(@class.Members);

            }

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

            members.AddRange(node.Properties.Select(x => SharedGeneratorion.GetOnlyAccessor(x.Name, SyntaxFactory.ParseTypeName(x.Type))));

            members.Add(CreateCtor(generator, node, options));

            if (!node.Abstract)
            {
                members.Add(CreateGetChildAt(generator, node, options));
                members.Add(CreateToString(generator, node, options));
                members.Add(CreateUpdate(generator, node, options));
            }


            return generator.ClassDeclaration(
                SharedGeneratorion.RedNodeName(node.Name),
                null,
                Accessibility.Public,
                DeclarationModifiers.Partial.WithIsAbstract(node.Abstract),
                SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.BaseClass)),
                null,
                members
            );
        }

        private static SyntaxNode CreateToString(SyntaxGenerator generator, Node node, Options options)
        {
            string formatstring = string.Join(" ", node.AllProperties().Select((n, i) => "{" + i + "}"));

            List<SyntaxNode> arguments = new List<SyntaxNode>()
            {
                generator.LiteralExpression(formatstring)
            };

            arguments.AddRange(node.AllProperties().Select(p => generator.IdentifierName(p.Name)));

            return generator.MethodDeclaration(
                "ToString",
                null,
                null,
                generator.TypeExpression(SpecialType.System_String),
                Accessibility.Public,
                DeclarationModifiers.Override,
                new []
                {
                    generator.ReturnStatement(generator.InvocationExpression(
                        generator.MemberAccessExpression(generator.TypeExpression(SpecialType.System_String), "Format"), arguments))
                }
                );
        }

        private static SyntaxNode CreateUpdate(SyntaxGenerator generator, Node node, Options options)
        {
            //Create the first comparison. This is the second member, as type will never change
            //This will be Interval, which don't support == so we need .Equals
            var first = generator.LogicalNotExpression(
                generator.InvocationExpression(
                    generator.MemberAccessExpression(
                        generator.IdentifierName(
                            node.AllMembers[1].PropertyName()
                        ),
                        "Equals"
                    ),
                    generator.IdentifierName(
                        node.AllMembers[1].ParameterName()
                    )
                )
            );

            //Create all the other comparisons. Those use ==
            var rest = node.AllMembers.Skip(2)
                .Where(m => !m.IsImplicitlyAssigned(node.Name))
                .Select(member => generator.ReferenceNotEqualsExpression(
                    generator.IdentifierName(member.PropertyName()),
                    generator.IdentifierName(member.ParameterName())
                ));

            //Reduce (Aggregate) all those comparisons (first + rest) into one big that checks if any are true
            var comparison = rest.Aggregate(first, generator.LogicalOrExpression);

            //.Select(n => n.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed))
            var changereturn = generator.ReturnStatement(
                generator.CastExpression(
                    node.GetRepresentation(TypeClassContext.Red),
                    generator.InvocationExpression(
                        generator.IdentifierName("Translplant"),
                        generator.InvocationExpression(
                            generator.MemberAccessExpression(
                                generator.IdentifierName("CrawlSyntaxNode"),
                                node.Name == "Type" ? "TypeNode" : node.Name
                            ),
                            node
                                .AllMembers
                                .Where(m => !m.IsImplicitlyAssigned(node.Name))
                                .Select(p =>
                                    generator.IdentifierName(p.ParameterName())
                                )
                        )
                    )
                )
            );

            SyntaxNode changeifstmt = generator.IfStatement(comparison
                , new []{changereturn});

            return generator.MethodDeclaration("Update",
                node
                    .AllMembers
                    .Where(m => !m.IsImplicitlyAssigned(node.Name))
                    .Select(m =>
                        generator.ParameterDeclaration(
                            m.ParameterName(),
                            m.GetRepresentation(TypeClassContext.Red))),
                null,
                SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)),
                Accessibility.Public,
                DeclarationModifiers.None,
                new []
                {
                    changeifstmt,
                    generator.ReturnStatement(generator.ThisExpression())
                });
        }

        private static SyntaxNode CreateComparison(SyntaxGenerator generator, string item)
        {
            return generator.ValueNotEqualsExpression(generator.IdentifierName(item),
                generator.IdentifierName(item.AsParameter()));
        }

        private static SyntaxNode CreateGetChildAt(SyntaxGenerator generator, Node node, Options options)
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

        private static SyntaxNode CreateCtor(SyntaxGenerator generator, Node node, Options options)
        {
            List<SyntaxNode> extraNodes = new List<SyntaxNode>();
            if (!string.IsNullOrWhiteSpace(node.ExtraConstructorCode))
            {
                SyntaxTree tree = CSharpSyntaxTree.ParseText("class f{ public f(){" + node.ExtraConstructorCode + "}}");
                var cu = (CompilationUnitSyntax)tree.GetRoot();
                var @class = (ClassDeclarationSyntax)cu.Members[0];
                var ctor = (ConstructorDeclarationSyntax)@class.Members[0];
                extraNodes.AddRange(ctor.Body.Statements);

            }
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
                Accessibility.Internal,
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
                            generator.MemberAccessExpression(generator.IdentifierName(options.Self), x.Name))).Concat(extraNodes)

            );
        }

        private static IEnumerable<SyntaxNode> CreateGetter(SyntaxGenerator generator, Child child, int index, int parrentChildCount, Options options)
        {
            yield return
                generator.ReturnStatement(generator.InvocationExpression(generator.IdentifierName(options.GetRed),
                    generator.Argument(RefKind.Ref, generator.IdentifierName("_" + child.Name.AsParameter())),
                    generator.Argument(generator.LiteralExpression(index+parrentChildCount))));
        }
    }
}
