using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace CodeGeneratorDriver
{
    //If you're just browsing, do have a look at RedNodeGenerator.cs, it has more comments.
    abstract class VisitorGenerator
    {
        protected readonly SyntaxGenerator Generator;
        protected readonly Model Model;
        private readonly bool _notVoid;
        protected readonly Options Options;

        protected virtual bool noT => _notVoid;
        protected SyntaxNode BaseType = null;
        

        protected VisitorGenerator(SyntaxGenerator generator, Model model, bool T = false)
        {
            Options = model.Options;
            this.Generator = generator;
            this.Model = model;
            _notVoid = T;
        }

        public SyntaxNode CreateVisitor(string name)
        {
            SyntaxNode bailIfNull = Generator.IfStatement(
                Generator.ValueEqualsExpression(Generator.IdentifierName(Options.Node.StartingWithLowercase()),
                    Generator.LiteralExpression(null)),
                new[]
                {
                    Generator.ReturnStatement(_notVoid ? Generator.DefaultExpression(ReturnType()) : null)
                });


            SyntaxNode theBigVisitMethod = Generator.MethodDeclaration(
                Options.Visit,
                new[]
                {
                    Generator.ParameterDeclaration(Options.Node.StartingWithLowercase(), Options.RedBase())
                },
                null,
                ReturnType(),
                Accessibility.Public,
                BaseType == null ? DeclarationModifiers.Virtual : DeclarationModifiers.Override,
                new[]
                {
                    bailIfNull,
                    Generator.SwitchStatement(
                        Generator.MemberAccessExpression(Generator.IdentifierName(Options.Node.StartingWithLowercase()), "Type"),
                        Model.Nodes.Where(x => x.Abstract == false).Select(SwitchSectionVoid)
                    ),
                    Generator.ThrowStatement(
                        Generator.ObjectCreationExpression(SyntaxFactory.ParseTypeName("ArgumentOutOfRangeException")))
                });


            List<SyntaxNode> theSmallVisitMethods = Model.Nodes
                .Where(x => !x.Manual)
                .Where(x => x.Abstract == false)
                .Where(Filter)
                .Select(VisitMethod)
                .ToList();

            List<SyntaxNode> allMethods = new List<SyntaxNode>();
            allMethods.Add(theBigVisitMethod);
            allMethods.AddRange(ExtraMembers());
            allMethods.AddRange(theSmallVisitMethods);

            return Generator.ClassDeclaration(name, noT ? new[] {"T"} : null, Accessibility.Public,
                DeclarationModifiers.Abstract.WithPartial(true), BaseType, null,
                allMethods);
        }

        protected virtual TypeSyntax ReturnType()
        {
            return _notVoid? SyntaxFactory.ParseTypeName("T"): null;
        }

        protected abstract bool Filter(Node arg);


        protected abstract SyntaxNode VisitMethod(Node node);
        

        protected virtual SyntaxNode SwitchSectionVoid(Node arg)
        {
            return Generator.SwitchSection(SyntaxFactory.ParseExpression("NodeType." + arg.Name.NonGenericPart()),
                new[]
                {
                    Generator.ReturnStatement(
                        Generator.InvocationExpression(NameOfVisitMethod(arg),
                            GenerateCastExpression(arg)))
                });
        }

        protected SyntaxNode NameOfVisitMethod(Node arg)
        {
            return Generator.IdentifierName(Options.Visit+ arg.Name.NonGenericPart());
        }

        protected SyntaxNode GenerateCastExpression(Node arg)
        {
            return Generator.CastExpression(
                arg.GetRepresentation(TypeClassContext.NotList | TypeClassContext.Red),
                Generator.IdentifierName(Options.Node.StartingWithLowercase()));
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
            return Generator.MethodDeclaration(Options.Visit + node.Name,
                new[]
                {
                    Generator.ParameterDeclaration(Options.Node.StartingWithLowercase(),
                        SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                },
                null, null,
                Accessibility.Protected, DeclarationModifiers.Virtual,
                node.AllChildren()
                    .Select(
                        x =>
                            Generator.InvocationExpression(
                                Generator.IdentifierName(Options.Visit),
                                Generator.MemberAccessExpression(
                                    Generator.IdentifierName(Options.Node.StartingWithLowercase()),
                                    x.Name))));
        }

        protected override SyntaxNode SwitchSectionVoid(Node arg)
        {
            return Generator.SwitchSection(SyntaxFactory.ParseExpression("NodeType." + arg.Name.NonGenericPart()),
                new[]
                {
                    Generator.InvocationExpression(NameOfVisitMethod(arg),
                        GenerateCastExpression(arg)),
                    Generator.ReturnStatement()
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
                return Generator.MethodDeclaration(Options.Visit + node.Name,
                    new[]
                    {
                        Generator.ParameterDeclaration(Options.Node.StartingWithLowercase(),
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                    },
                    null, ReturnType(),
                    Accessibility.Protected, DeclarationModifiers.Virtual,
                    new[]
                    {
                        Generator.ReturnStatement(Generator.DefaultExpression(SyntaxFactory.ParseTypeName("T")))
                    });
            }
            else if (childCount == 1)
            {
                return Generator.MethodDeclaration(Options.Visit + node.Name,
                    new[]
                    {
                        Generator.ParameterDeclaration(Options.Node.StartingWithLowercase(),
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                    },
                    null, ReturnType(),
                    Accessibility.Protected, DeclarationModifiers.Virtual, 
                    new []
                    {
                        Generator.ReturnStatement(Generator.InvocationExpression(
                                Generator.IdentifierName(Options.Visit),
                                Generator.MemberAccessExpression(
                                    Generator.IdentifierName(Options.Node.StartingWithLowercase()),
                                    node.AllChildren().First().Name)))
                    });
            }
            else
            {
                return Generator.MethodDeclaration(Options.Visit + node.Name,
                    new[]
                    {
                        Generator.ParameterDeclaration(Options.Node.StartingWithLowercase(),
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
            this.BaseType = baseType;
        }

        protected override bool Filter(Node arg)
        {
            return arg.AllChildren().Count() >= 2;
        }

        protected override SyntaxNode VisitMethod(Node node)
        {
            return Generator.MethodDeclaration(Options.Visit + node.Name,
                new[]
                {
                    Generator.ParameterDeclaration(Options.Node.StartingWithLowercase(),
                        SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
                },
                null, ReturnType(),
                Accessibility.Protected, DeclarationModifiers.Override,
                new[]
                {
                    Generator.ReturnStatement(Generator.InvocationExpression(
                        Generator.IdentifierName(Options.Combine),
                        node.AllChildren()
                            .Select(
                                x =>
                                    Generator.InvocationExpression(Generator.IdentifierName(Options.Visit),
                                        Generator.MemberAccessExpression(
                                            Generator.IdentifierName(Options.Node.StartingWithLowercase()), x.Name)))

                    ))
                });
        }

        protected override IEnumerable<SyntaxNode> ExtraMembers()
        {

            yield return Generator.MethodDeclaration("Combine",
                new[]
                {
                    SyntaxFactory.Parameter(SyntaxFactory.List<AttributeListSyntax>(),
                        SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.ParamsKeyword)),
                        (TypeSyntax) Generator.ArrayTypeExpression(ReturnType()), SyntaxFactory.Identifier("parts"),
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
            this.BaseType = baseType;
        }

        protected override bool Filter(Node arg) => true;

        protected override SyntaxNode VisitMethod(Node node)
        {
            string parametername = node.Name.StartingWithLowercase();

            //Visits, in order
            List<SyntaxNode> statements = node.AllMembers.Where(member => member.IsNode)
                .Select(n =>
                    Generator.LocalDeclarationStatement(n.ParameterName(),
                        Generator.CastExpression(
                            n.GetRepresentation(TypeClassContext.Red),
                            Generator.InvocationExpression(Generator.IdentifierName(Options.Visit),
                                Generator.MemberAccessExpression(Generator.IdentifierName(parametername),
                                    n.PropertyName()))))

                )
                .ToList();

            //Call to an "Update" method that returns a copy of a node with new children
            //Nodes get their value from visists, rest just pulled out
            statements.Add(
                Generator.ReturnStatement(
                    Generator.InvocationExpression(
                        Generator.MemberAccessExpression(Generator.IdentifierName(parametername), Options.Update),
                        node
                            .AllMembers
                            .Where(member => !member.IsImplicitlyAssigned(node.Name))
                            .Select(member =>
                            member.IsNode
                                ? Generator.IdentifierName(member.ParameterName())
                                : Generator.MemberAccessExpression(
                                    Generator.IdentifierName(parametername),
                                    member.PropertyName()
                                )
                        )
                    )
                )
            );

            return Generator.MethodDeclaration(
                Options.Visit + node.Name,
                new[]
                {
                    Generator.ParameterDeclaration(parametername, node.GetRepresentation())
                },
                null,
                ReturnType(),
                Accessibility.Protected, DeclarationModifiers.Override,
                statements

            );
        }

        protected override TypeSyntax ReturnType()
        {
            return Options.RedBase();
        }
    }
}
