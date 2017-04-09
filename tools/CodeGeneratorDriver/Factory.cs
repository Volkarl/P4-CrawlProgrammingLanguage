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
    class Factory
    {
        internal static SyntaxNode[] CreateFactoryFor(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            var single = generator.MethodDeclaration(node.Name + "Node",
                node.AllProperties()
                    .Skip(1)
                    .Select(
                        x => generator.ParameterDeclaration(x.Name.AsParameter(), SyntaxFactory.ParseTypeName(x.Type)))
                    .Concat(node.AllChildren()
                        .Select(
                            x =>
                                generator.ParameterDeclaration(x.Name.AsParameter(),
                                    SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(x.Type))))),
                null,
                SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)),
                Accessibility.Public,
                DeclarationModifiers.Static, new[]
                {
                    generator.ReturnStatement(
                        generator.CastExpression(
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)),
                            generator.InvocationExpression(
                                generator.MemberAccessExpression(
                                    generator.ObjectCreationExpression(
                                        SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(node.Name)), new[]
                                            {
                                                SyntaxFactory.ParseExpression("NodeType." + node.Name)
                                            }.Concat(node.AllProperties()
                                                .Skip(1)
                                                .Select(x => generator.IdentifierName(x.Name.AsParameter())))
                                            .Concat(node.AllChildren()
                                                .Select(
                                                    x => generator.CastExpression(
                                                        SyntaxFactory.ParseTypeName(
                                                            SharedGeneratorion.GreenNodeName(x.Type)),
                                                        generator.MemberAccessExpression(
                                                            generator.IdentifierName(x.Name.AsParameter()),
                                                            "Green"))))),
                                    options.CreateRed),
                                generator.LiteralExpression(null), generator.LiteralExpression(0))))
                });

            return new[] {single};


        }
    }
}
