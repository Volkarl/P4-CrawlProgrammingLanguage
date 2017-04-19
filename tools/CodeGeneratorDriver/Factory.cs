using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        internal static SyntaxNode[] CreateFactoryFor(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options)
        {
            List<Parameter> greenParameters = CreateInitialParameters(node).ToList();
            List<List<ParameterGenerationInfo>> v = CreateAllVariations(greenParameters, node, generator);

            var q = v.Select(param => CreateMethod(generator, node, options, param));

            return q.ToArray();


        }

        private static SyntaxNode CreateMethod(SyntaxGenerator generator, Node node, SyntaxGenerationOptions options, List<ParameterGenerationInfo> param)
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

        private static List<ParameterGenerationInfo> GenerateWays(Parameter parameter, Node node, SyntaxGenerator generator, bool last)
        {
            if (parameter.ParameterType == ParameterType.Green)
            {
                return GenerateGreenParameters(generator, parameter, node, last);
            }
            else if (parameter.ParameterType == ParameterType.Normal)
            {
                return GeneratePropertyParameters(generator, parameter, node);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private static List<ParameterGenerationInfo> GenerateGreenParameters(SyntaxGenerator generator,
            Parameter parameter, Node node, bool last)
        {
            string identifier = GenerateUnusedIdentifier();

            SyntaxNode initializer = null;
            if (parameter.Null)
            {
                initializer = generator.LiteralExpression(null);
            }
            var std = new ParameterGenerationInfo()
            {
                Code = new[]
                {

                    generator.LocalDeclarationStatement(
                        SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(parameter.Type)), identifier,
                        generator.ConditionalExpression(
                            generator.ReferenceNotEqualsExpression(
                                generator.IdentifierName(parameter.Name.AsParameter()),
                                generator.LiteralExpression(null)), generator.CastExpression(
                                SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(parameter.Type)),
                                generator.MemberAccessExpression(generator.IdentifierName(parameter.Name.AsParameter()),
                                    "Green")), generator.LiteralExpression(null)))


                },
                Parameters = new[]
                {

                    generator.ParameterDeclaration(parameter.Name.AsParameter(),
                        SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(parameter.Type)), initializer)
                },
                FinalIdentifier = identifier
            };

            if (!parameter.Type.StartsWith("List'"))
                return new List<ParameterGenerationInfo>(1)
                {
                    std
                };
            else
            {
                var items = new List<ParameterGenerationInfo>
                {
                    std,
                    CreateListCreator(generator, parameter,
                        generator.ParameterDeclaration(parameter.Name.AsParameter(),
                            SyntaxFactory.ParseTypeName(
                                $"IEnumerable<{SharedGeneratorion.RedNodeName(parameter.Type.Split('\'')[1])}>")))
                };

                if (!last)
                {
                    items.Add(CreateListCreator(generator, parameter, SyntaxFactory.Parameter(
                        SyntaxFactory.List(new AttributeListSyntax[0]),
                        SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.ParamsKeyword)),
                        SyntaxFactory.ParseTypeName(
                            SharedGeneratorion.RedNodeName(parameter.Type.Split('\'')[1]) + "[]"),
                        SyntaxFactory.Identifier(parameter.Name.AsParameter()), null)));
                }


                return items;
            }
        }

        private static ParameterGenerationInfo CreateListCreator(SyntaxGenerator generator, Parameter parameter, SyntaxNode theParameter)
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
                            SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(parameter.Type)),
                            SyntaxFactory.ParseExpression("SyntaxTree.NodeType.List"),
                            generator.MemberAccessExpression(generator.IdentifierName("Interval"), "Invalid"),
                            generator.InvocationExpression(generator.MemberAccessExpression(
                                generator.IdentifierName(parameter.Name.AsParameter()), "Select"), generator.ValueReturningLambdaExpression("x", generator.MemberAccessExpression(generator.IdentifierName("x"), "Green")))))
                }
            };
        }

        private static List<ParameterGenerationInfo> GeneratePropertyParameters(SyntaxGenerator generator, Parameter parameter, Node node)
        {
            ExpressionType type;
            if (parameter.Type == "NodeType")
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
            else if (parameter.Type == "ExpressionType" && Enum.TryParse(node.Name, true, out type))
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
            else if (parameter.Type == "IEnumerable<CrawlSyntaxNode>")
            {
                string identifier = GenerateUnusedIdentifier();
                return new List<ParameterGenerationInfo>()
                {
                    new ParameterGenerationInfo()
                    {
                        Parameters = new[]
                        {
                            generator.ParameterDeclaration(parameter.Name.AsParameter(),
                                SyntaxFactory.ParseTypeName(parameter.Type))
                        },
                        Code = new[]
                        {
                            generator.LocalDeclarationStatement(identifier,
                                generator.InvocationExpression(
                                    generator.MemberAccessExpression(
                                        generator.IdentifierName(parameter.Name.AsParameter()), "Select"),
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
                            generator.ParameterDeclaration(parameter.Name.AsParameter(),
                                SyntaxFactory.ParseTypeName(parameter.Type))
                        },
                        FinalIdentifier = parameter.Name.AsParameter()
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

        private static List<List<ParameterGenerationInfo>> CreateAllVariations(List<Parameter> parameters, Node node, SyntaxGenerator generator)
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

        private static IEnumerable<Parameter> CreateInitialParameters(Node node)
        {
            foreach (Property property in node.AllProperties())
            {
                yield return new Parameter
                {
                    Name = property.Name,
                    Type = property.Type,
                    ParameterType = ParameterType.Normal
                };
            }

            foreach (Child child in node.AllChildren())
            {
                yield return new Parameter
                {
                    Name = child.Name,
                    Type = child.Type,
                    ParameterType = ParameterType.Green,
                    Null = child.NullDefault
                };
            }
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
