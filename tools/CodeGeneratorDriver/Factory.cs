using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.Isam.Esent.Interop;

namespace CodeGeneratorDriver
{
    class Factory
    {
        internal static SyntaxNode[] CreateFactoryFor(SyntaxGenerator generator, Node node, Options options)
        {

            List<List<ParameterGenerationInfo>> v = CreateAllVariations(node.AllMembers, node, generator);

            var q = v.Select(param => CreateMethod(generator, node, options, param));

            return q.ToArray();


        }

        private static SyntaxNode CreateMethod(SyntaxGenerator generator, Node node, Options options, List<ParameterGenerationInfo> param)
        {
            return generator.MethodDeclaration(node.Name == "Type" ? "TypeNode" : node.Name,
                param.SelectMany(x => x.Parameters),
                null,
                SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)),
                Accessibility.Public,
                DeclarationModifiers.Static,
                param.SelectMany(x => x.Code)
                    .Concat(new[]
                    {
                        generator.LocalDeclarationStatement("__green",
                            generator.ObjectCreationExpression(
                                SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(node.Name)),
                                param.Select(x => generator.IdentifierName(x.FinalIdentifier)))),
                        generator.LocalDeclarationStatement("__red", generator.CastExpression(
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(node.Name)),
                            generator.InvocationExpression(
                                generator.MemberAccessExpression(generator.IdentifierName("__green"),
                                    options.CreateRed), generator.LiteralExpression(null),
                                generator.LiteralExpression(0)))),

                        generator.ReturnStatement(generator.IdentifierName("__red"))
                    })
            );
        }

        private static List<ParameterGenerationInfo> GenerateWays(Member member, Node node, SyntaxGenerator generator, bool last)
        {
            if (member.IsNode)
            {
                return GenerateGreenParameters(generator, member, node, last);
            }
            else
            {
                return GeneratePropertyParameters(generator, member, node);
            }
        }

        private static List<ParameterGenerationInfo> GenerateGreenParameters(SyntaxGenerator generator,
            Member member, Node node, bool last)
        {
            string identifier = GenerateUnusedIdentifier();

            SyntaxNode initializer = null;
            if (member.NullAllowed)
            {
                initializer = generator.LiteralExpression(null);
            }
            var std = new ParameterGenerationInfo()
            {
                Code = new[]
                {

                    generator.LocalDeclarationStatement(
                        member.GetRepresentation(TypeClassContext.None), identifier,
                        generator.ConditionalExpression(
                            generator.ReferenceNotEqualsExpression(
                                generator.IdentifierName(member.ParameterName()),
                                generator.LiteralExpression(null)), generator.CastExpression(
                                member.GetRepresentation(TypeClassContext.None),
                                generator.MemberAccessExpression(generator.IdentifierName(member.ParameterName()),
                                    "Green")), generator.LiteralExpression(null)))


                },
                Parameters = new[]
                {

                    generator.ParameterDeclaration(member.ParameterName(),
                        member.GetRepresentation(TypeClassContext.Red), initializer)
                },
                FinalIdentifier = identifier
            };

            if (!member.IsList)
                return new List<ParameterGenerationInfo>(1)
                {
                    std
                };
            else
            {
                var items = new List<ParameterGenerationInfo>
                {
                    std,
                    CreateListCreator(generator, member,
                        generator.ParameterDeclaration(member.ParameterName(),
                            member.GetRepresentation(TypeClassContext.Red | TypeClassContext.NotList)))
                };

                if (!last)
                {
                    items.Add(CreateListCreator(generator, member, SyntaxFactory.Parameter(
                        SyntaxFactory.List(new AttributeListSyntax[0]),
                        SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.ParamsKeyword)),
                        member.GetRepresentation(TypeClassContext.Red | TypeClassContext.Array) ,
                        SyntaxFactory.Identifier(member.ParameterName()), null)));
                }


                return items;
            }
        }

        private static ParameterGenerationInfo CreateListCreator(SyntaxGenerator generator, Member member, SyntaxNode theParameter)
        {
            string identifier = GenerateUnusedIdentifier();
            return new ParameterGenerationInfo()
            {
                FinalIdentifier = identifier,
                Parameters = new SyntaxNode[1]
                {
                    theParameter
                },
                Code = new[]
                {
                    generator.LocalDeclarationStatement(identifier,
                        generator.ObjectCreationExpression(
                            member.GetRepresentation(TypeClassContext.None),
                            SyntaxFactory.ParseExpression("SyntaxTree.NodeType.List"),
                            generator.MemberAccessExpression(generator.IdentifierName("Interval"), "Invalid"),
                            generator.InvocationExpression(generator.MemberAccessExpression(
                                generator.IdentifierName(member.ParameterName()), "Select"), generator.ValueReturningLambdaExpression("x", generator.MemberAccessExpression(generator.IdentifierName("x"), "Green")))))
                }
            };
        }

        private static List<ParameterGenerationInfo> GeneratePropertyParameters(SyntaxGenerator generator, Member member, Node node)
        {
            ExpressionType type;
            string typeName = member.Type;
            if (typeName== "NodeType")
            {
                string identifier = GenerateUnusedIdentifier();
                return new List<ParameterGenerationInfo>(1)
                {
                    new ParameterGenerationInfo()
                    {
                        Code = new[]
                        {
                            generator.LocalDeclarationStatement(SyntaxFactory.ParseTypeName("NodeType"), identifier,
                                SyntaxFactory.ParseExpression("SyntaxTree.NodeType." + node.Name))
                        },
                        FinalIdentifier = identifier
                    }
                };
            }
            else if (typeName == "ExpressionType" && Enum.TryParse(node.Name, true, out type))
            {
                string identifier = GenerateUnusedIdentifier();
                return new List<ParameterGenerationInfo>(1)
                {
                    new ParameterGenerationInfo()
                    {
                        Code = new[]
                        {
                            generator.LocalDeclarationStatement(SyntaxFactory.ParseTypeName("ExpressionType"), identifier,
                                SyntaxFactory.ParseExpression("SyntaxTree.ExpressionType." + type))
                        },
                        FinalIdentifier = identifier
                    }
                };
            }
            else if (typeName == "IEnumerable")
            {
                string identifier = GenerateUnusedIdentifier();
                return new List<ParameterGenerationInfo>()
                {
                    new ParameterGenerationInfo()
                    {
                        Parameters = new[]
                        {
                            generator.ParameterDeclaration(member.ParameterName(),
                                member.GetRepresentation())
                        },
                        Code = new[]
                        {
                            generator.LocalDeclarationStatement(identifier,
                                generator.InvocationExpression(
                                    generator.MemberAccessExpression(
                                        generator.IdentifierName(member.ParameterName()), "Select"),
                                    generator.ValueReturningLambdaExpression("z",
                                        generator.MemberAccessExpression(generator.IdentifierName("z"), "Green"))))
                        },
                        FinalIdentifier = identifier

                    },
                };
            }
            else
            {
                return new List<ParameterGenerationInfo>(1)
                {
                    new ParameterGenerationInfo()
                    {
                        Parameters = new[]
                        {
                            generator.ParameterDeclaration(member.ParameterName(),
                                member.GetRepresentation())
                        },
                        FinalIdentifier = member.ParameterName()
                    }
                };
            }
        }

        private static int _identifierCount;
        private static string GenerateUnusedIdentifier()
        {

            int id = Interlocked.Increment(ref _identifierCount);
            return $"__generated{id}";
        }

        private static List<List<ParameterGenerationInfo>> CreateAllVariations(List<Member> parameters, Node node, SyntaxGenerator generator)
        {
            /*
             * For a list of parameters (parameters) first find all ways to create that parameter (GenerateWays)
             * Then loop over all possible combinations and save them in results
             */

            List<List<ParameterGenerationInfo>> possibilities = parameters.Select((parameter, i)=> GenerateWays(parameter, node, generator, parameters.Count -1 != i)).ToList();
            int[] countersMax = possibilities.Select(x => x.Count).ToArray();
            int[] counters = new int[countersMax.Length];
            List<List<ParameterGenerationInfo>> results = new List<List<ParameterGenerationInfo>>(countersMax.Sum());

            while (true)
            {

                List<ParameterGenerationInfo> newEntry = new List<ParameterGenerationInfo>(counters.Length);

                for (int i = 0; i < counters.Length; i++)
                {
                    newEntry.Add(possibilities[i][counters[i]]);
                }

                results.Add(newEntry);


                for (int i = 0; i <= counters.Length; i++)
                {
                    if(i == counters.Length) goto _out;

                    counters[i]++;
                    if (counters[i] == countersMax[i])
                    {
                        counters[i] = 0;
                    }
                    else break;
                }
            }
            _out:;

            return results;
        }

        [DebuggerDisplay("{Name}")]
        class Parameter
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public ParameterType ParameterType { get; set; }
            public bool Null { get; set; }
        }

        class ParameterGenerationInfo
        {
            public SyntaxNode[] Parameters { get; set; } = new SyntaxNode[0];
            public SyntaxNode[] Code { get; set; } = new SyntaxNode[0];
            public string FinalIdentifier { get; set; }
        }
    }

    internal enum ParameterType
    {
        Normal,
        Green,
        Red
    }
}
