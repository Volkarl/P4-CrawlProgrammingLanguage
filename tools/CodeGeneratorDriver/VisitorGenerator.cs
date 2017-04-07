using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace CodeGeneratorDriver
{
    abstract class VisitorGenerator
    {
        protected readonly SyntaxGenerator generator;
        protected readonly SyntaxGeneration syntaxGeneration;
        private readonly bool notVoid;
        protected readonly SyntaxGenerationOptions options;

        protected SyntaxNode baseType = null;
        

        protected VisitorGenerator(SyntaxGenerator generator, SyntaxGeneration syntaxGeneration, bool T = false)
        {
            options = syntaxGeneration.Options;
            this.generator = generator;
            this.syntaxGeneration = syntaxGeneration;
            notVoid = T;
        }

        public SyntaxNode CreateVisitor(string name)
        {
            
            SyntaxNode theBigVisitMethod = generator.MethodDeclaration(
                options.Visit,
                new[]
                {
                    generator.ParameterDeclaration(options.Node.AsParameter(), options.RedBase())
                },
                null,
                ReturnType(),
                Accessibility.Public,
                DeclarationModifiers.Virtual,
                new[]
                {
                    generator.SwitchStatement(
                        generator.MemberAccessExpression(generator.IdentifierName(options.Node.AsParameter()), "Type"),
                            syntaxGeneration.Node.Select(SwitchSectionVoid)
                        )
                });


            List<SyntaxNode> theSmallVisitMethods = syntaxGeneration.Node.Where(Filter).Select(VisitMethod).ToList();

            List<SyntaxNode> allMethods = new List<SyntaxNode>();
            allMethods.Add(theBigVisitMethod);
            allMethods.AddRange(theSmallVisitMethods);

            return generator.ClassDeclaration(name, notVoid ? new[] {"T"} : null, Accessibility.Public,
                DeclarationModifiers.None, baseType, null,
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
            return generator.SwitchSection(SyntaxFactory.ParseExpression("NodeType." + arg.Name),
                new[]
                {
                    generator.InvocationExpression(generator.IdentifierName(options.Visit + arg.Name),
                        generator.CastExpression(SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(arg.Name)),
                            generator.IdentifierName(options.Node.AsParameter()))),
                    generator.ReturnStatement()
                });
        }   
    }

    class VoidVisitorGenerator : VisitorGenerator
    {
        public VoidVisitorGenerator(SyntaxGenerator generator, SyntaxGeneration syntaxGeneration) : base(generator, syntaxGeneration)
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
    }

    class SimpleTVisitorGenerator : VisitorGenerator
    {
        public SimpleTVisitorGenerator(SyntaxGenerator generator, SyntaxGeneration syntaxGeneration) : base(generator, syntaxGeneration, true)
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
                        generator.ReturnStatement(generator.DefaultExpression(SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name))))
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

        protected override SyntaxNode SwitchSectionVoid(Node arg)
        {
            return generator.SwitchSection(SyntaxFactory.ParseExpression("NodeType." + arg.Name),
                new[]
                {
                    generator.ReturnStatement(
                        generator.InvocationExpression(generator.IdentifierName(options.Visit+ arg.Name),
                            generator.CastExpression(
                                SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(arg.Name)),
                                generator.IdentifierName(options.Node.AsParameter()))))
                });
        }
    }

    class ComplexTVisitorGenerator : SimpleTVisitorGenerator
    {
        public ComplexTVisitorGenerator(SyntaxGenerator generator, SyntaxGeneration syntaxGeneration, SyntaxNode baseType) : base(generator, syntaxGeneration)
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
                Accessibility.Protected, DeclarationModifiers.Virtual,
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
    }

    class SyntaxRewriterGenerator : VisitorGenerator
    {
        public SyntaxRewriterGenerator(SyntaxGenerator generator, SyntaxGeneration syntaxGeneration, SyntaxNode baseType) : base(generator, syntaxGeneration, false)
        {
            this.baseType = baseType;
        }

        protected override bool Filter(Node arg) => true;

        protected override SyntaxNode VisitMethod(Node node)
        {
            
            List<SyntaxNode> statements =
                node.AllChildren()
                    .Select(
                        x =>
                            generator.LocalDeclarationStatement(
                                SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(x.Type)),
                                x.Name.AsParameter(),
                                generator.CastExpression(SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(x.Type)),
                                    generator.InvocationExpression(generator.IdentifierName(options.Visit),
                                        generator.MemberAccessExpression(
                                            generator.IdentifierName(options.Node.AsParameter()), x.Name))))).ToList();

            statements.Add(
                generator.ReturnStatement(
                    generator.InvocationExpression(
                        generator.MemberAccessExpression(generator.IdentifierName(options.Node.AsParameter()),
                            options.Update),
                        node.AllChildren().Select(x => generator.IdentifierName(x.Name.AsParameter()))
                    )));

            return generator.MethodDeclaration(options.Visit + node.Name, new[]
            {
                generator.ParameterDeclaration(options.Node.AsParameter(),
                    SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)))
            }, null, ReturnType(), Accessibility.Protected, DeclarationModifiers.Virtual, statements);
        }

        protected override TypeSyntax ReturnType()
        {
            return options.RedBase();
        }
    }
}
