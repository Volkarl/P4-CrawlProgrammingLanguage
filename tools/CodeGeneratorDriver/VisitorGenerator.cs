using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace CodeGeneratorDriver
{
    abstract class VisitorGenerator
    {
        protected readonly SyntaxGenerator generator;
        protected readonly Model model;
        private readonly bool notVoid;
        protected readonly Options options;

        protected virtual bool noT => notVoid;
        protected SyntaxNode baseType = null;
        

        protected VisitorGenerator(SyntaxGenerator generator, Model model, bool T = false)
        {
            options = model.Options;
            this.generator = generator;
            this.model = model;
            notVoid = T;
        }

        public SyntaxNode CreateVisitor(string name)
        {
            SyntaxNode bailIfNull = generator.IfStatement(
                generator.ValueEqualsExpression(generator.IdentifierName(options.Node.AsParameter()),
                    generator.LiteralExpression(null)),
                new[]
                {
                    generator.ReturnStatement(notVoid ? generator.DefaultExpression(ReturnType()) : null)
                });


            SyntaxNode theBigVisitMethod = generator.MethodDeclaration(
                options.Visit,
                new[]
                {
                    generator.ParameterDeclaration(options.Node.AsParameter(), options.RedBase())
                },
                null,
                ReturnType(),
                Accessibility.Public,
                baseType == null ? DeclarationModifiers.Virtual : DeclarationModifiers.Override,
                new[]
                {
                    bailIfNull,
                    generator.SwitchStatement(
                        generator.MemberAccessExpression(generator.IdentifierName(options.Node.AsParameter()), "Type"),
                        model.Node.Where(x => x.Abstract == false).Select(SwitchSectionVoid)
                    ),
                    generator.ThrowStatement(
                        generator.ObjectCreationExpression(SyntaxFactory.ParseTypeName("ArgumentOutOfRangeException")))
                });


            List<SyntaxNode> theSmallVisitMethods = model.Node
                .Where(x => !x.Manual)
                .Where(x => x.Abstract == false)
                .Where(Filter)
                .Select(VisitMethod)
                .ToList();

            List<SyntaxNode> allMethods = new List<SyntaxNode>();
            allMethods.Add(theBigVisitMethod);
            allMethods.AddRange(ExtraMembers());
            allMethods.AddRange(theSmallVisitMethods);

            return generator.ClassDeclaration(name, noT ? new[] {"T"} : null, Accessibility.Public,
                DeclarationModifiers.Abstract.WithPartial(true), baseType, null,
                allMethods);
        }

        protected virtual TypeSyntax ReturnType()
        {
            return notVoid? SyntaxFactory.ParseTypeName("T"): null;
        }

        protected abstract bool Filter(Node arg);


        protected abstract SyntaxNode VisitMethod(Node node);
        

        protected virtual SyntaxNode SwitchSectionVoid(Node arg)
        {
            return generator.SwitchSection(SyntaxFactory.ParseExpression("NodeType." + arg.Name.NonGenericPart()),
                new[]
                {
                    generator.ReturnStatement(
                        generator.InvocationExpression(NameOfVisitMethod(arg),
                            GenerateCastExpression(arg)))
                });
        }

        protected SyntaxNode NameOfVisitMethod(Node arg)
        {
            return generator.IdentifierName(options.Visit+ arg.Name.NonGenericPart());
        }

        protected SyntaxNode GenerateCastExpression(Node arg)
        {
            return generator.CastExpression(
                arg.GetRepresentation(TypeClassContext.NotList | TypeClassContext.Red),
                generator.IdentifierName(options.Node.AsParameter()));
        }

        protected virtual IEnumerable<SyntaxNode> ExtraMembers()
        { yield break;}
    }

    class VoidVisitorGenerator : VisitorGenerator
    {
        public VoidVisitorGenerator(SyntaxGenerator generator, Model model) : base(generator, model)
        {
        }

        protected override bool Filter(Node arg)
        {
            return true;
        }

        protected override SyntaxNode VisitMethod(Node node)
        {
            return generator.MethodDeclaration(options.Visit + node.Name,
                new[]
                {
                    generator.ParameterDeclaration(options.Node.AsParameter(),
                        SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                },
                null, null,
                Accessibility.Protected, DeclarationModifiers.Virtual,
                node.AllChildren()
                    .Select(
                        x =>
                            generator.InvocationExpression(
                                generator.IdentifierName(options.Visit),
                                generator.MemberAccessExpression(
                                    generator.IdentifierName(options.Node.AsParameter()),
                                    x.Name))));
        }

        protected override SyntaxNode SwitchSectionVoid(Node arg)
        {
            return generator.SwitchSection(SyntaxFactory.ParseExpression("NodeType." + arg.Name.NonGenericPart()),
                new[]
                {
                    generator.InvocationExpression(NameOfVisitMethod(arg),
                        GenerateCastExpression(arg)),
                    generator.ReturnStatement()
                });
        }
    }

    class SimpleTVisitorGenerator : VisitorGenerator
    {
        public SimpleTVisitorGenerator(SyntaxGenerator generator, Model model) : base(generator, model, true)
        {
        }

        protected override bool Filter(Node arg)
        {
            return true;
        }

        protected override SyntaxNode VisitMethod(Node node)
        {
            int childCount = node.AllChildren().Count();

            if (childCount == 0)
            {
                return generator.MethodDeclaration(options.Visit + node.Name,
                    new[]
                    {
                        generator.ParameterDeclaration(options.Node.AsParameter(),
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                    },
                    null, ReturnType(),
                    Accessibility.Protected, DeclarationModifiers.Virtual,
                    new[]
                    {
                        generator.ReturnStatement(generator.DefaultExpression(SyntaxFactory.ParseTypeName("T")))
                    });
            }
            else if (childCount == 1)
            {
                return generator.MethodDeclaration(options.Visit + node.Name,
                    new[]
                    {
                        generator.ParameterDeclaration(options.Node.AsParameter(),
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                    },
                    null, ReturnType(),
                    Accessibility.Protected, DeclarationModifiers.Virtual, 
                    new []
                    {
                        generator.ReturnStatement(generator.InvocationExpression(
                                generator.IdentifierName(options.Visit),
                                generator.MemberAccessExpression(
                                    generator.IdentifierName(options.Node.AsParameter()),
                                    node.AllChildren().First().Name)))
                    });
            }
            else
            {
                return generator.MethodDeclaration(options.Visit + node.Name,
                    new[]
                    {
                        generator.ParameterDeclaration(options.Node.AsParameter(),
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                    },
                    null, ReturnType(),
                    Accessibility.Protected, DeclarationModifiers.Abstract, null);
            }
        }


    }

    class ComplexTVisitorGenerator : SimpleTVisitorGenerator
    {
        public ComplexTVisitorGenerator(SyntaxGenerator generator, Model model, SyntaxNode baseType) : base(generator, model)
        {
            this.baseType = baseType;
        }

        protected override bool Filter(Node arg)
        {
            return arg.AllChildren().Count() >= 2;
        }

        protected override SyntaxNode VisitMethod(Node node)
        {
            return generator.MethodDeclaration(options.Visit + node.Name,
                new[]
                {
                    generator.ParameterDeclaration(options.Node.AsParameter(),
                        SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                },
                null, ReturnType(),
                Accessibility.Protected, DeclarationModifiers.Override,
                new[]
                {
                    generator.ReturnStatement(generator.InvocationExpression(
                        generator.IdentifierName(options.Combine),
                        node.AllChildren()
                            .Select(
                                x =>
                                    generator.InvocationExpression(generator.IdentifierName(options.Visit),
                                        generator.MemberAccessExpression(
                                            generator.IdentifierName(options.Node.AsParameter()), x.Name)))

                    ))
                });
        }

        protected override IEnumerable<SyntaxNode> ExtraMembers()
        {

            yield return generator.MethodDeclaration("Combine",
                new[]
                {
                    SyntaxFactory.Parameter(SyntaxFactory.List<AttributeListSyntax>(),
                        SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.ParamsKeyword)),
                        (TypeSyntax) generator.ArrayTypeExpression(ReturnType()), SyntaxFactory.Identifier("parts"),
                        null)
                },
                null,
                ReturnType(),
                Accessibility.Protected,
                DeclarationModifiers.Abstract);

        }
    }

    class SyntaxRewriterGenerator : VisitorGenerator
    {
        protected override bool noT => false;

        public SyntaxRewriterGenerator(SyntaxGenerator generator, Model model, SyntaxNode baseType) : base(generator, model, true)
        {
            this.baseType = baseType;
        }

        protected override bool Filter(Node arg) => true;

        protected override SyntaxNode VisitMethod(Node node)
        {
            string parametername = node.Name.AsParameter();

            //Visits, in order
            List<SyntaxNode> statements = node.AllMembers.Where(member => member.IsNode)
                .Select(n =>
                    generator.LocalDeclarationStatement(n.ParameterName(),
                        generator.CastExpression(
                            n.GetRepresentation(TypeClassContext.Red),
                            generator.InvocationExpression(generator.IdentifierName(options.Visit),
                                generator.MemberAccessExpression(generator.IdentifierName(parametername),
                                    n.PropertyName()))))

                )
                .ToList();

            //Call to an "Update" method that returns a copy of a node with new children
            //Nodes get their value from visists, rest just pulled out
            statements.Add(
                generator.ReturnStatement(
                    generator.InvocationExpression(
                        generator.MemberAccessExpression(generator.IdentifierName(parametername), options.Update),
                        node
                            .AllMembers
                            .Where(member => !member.IsImplicitlyAssigned(node.Name))
                            .Select(member =>
                            member.IsNode
                                ? generator.IdentifierName(member.ParameterName())
                                : generator.MemberAccessExpression(
                                    generator.IdentifierName(parametername),
                                    member.PropertyName()
                                )
                        )
                    )
                )
            );

            return generator.MethodDeclaration(
                options.Visit + node.Name,
                new[]
                {
                    generator.ParameterDeclaration(parametername, node.GetRepresentation())
                },
                null,
                ReturnType(),
                Accessibility.Protected, DeclarationModifiers.Override,
                statements

            );
        }

        protected override TypeSyntax ReturnType()
        {
            return options.RedBase();
        }
    }
}
