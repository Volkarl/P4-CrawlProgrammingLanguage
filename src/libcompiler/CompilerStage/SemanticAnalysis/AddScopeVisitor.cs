using System.Collections.Generic;
using System.Linq;
using libcompiler.Datatypes;
using libcompiler.Scope;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.CompilerStage.SemanticAnalysis
{
    internal class AddScopeVisitor : SyntaxRewriter
    {
        protected override CrawlSyntaxNode VisitBlock(BlockNode block)
        {
            BlockScope scope = block.Scope.CollectIdentifiers(block);
            var tmp = ((BlockNode) base.VisitBlock(block));
            var after = tmp.WithScope(scope);
            return after;
        }

        protected override CrawlSyntaxNode VisitMethodDecleration(MethodDeclerationNode methodDecleration)
        {
            IScope containingScope = methodDecleration.FindFirstScope();

            GenericScope scope = new GenericScope(
                methodDecleration
                    .Parameters
                    .Select(
                        (parameter, index) =>
                            new KeyValuePair<string, TypeInformation>(parameter.Identifier.Value,
                                new TypeInformation(
                                    parameter.ParameterType.ActualType,
                                    ProtectionLevel.NotApplicable,
                                    parameter.Interval.b,
                                    new UniqueItem()
                                )
                            )
                    )
            );

            MethodDeclerationNode node = (MethodDeclerationNode) base.VisitMethodDecleration(methodDecleration);

            return node.WithScope(scope);
        }

        protected override CrawlSyntaxNode VisitConstructor(ConstructorNode constructor)
        {
            return base.VisitConstructor(constructor);
        }

        protected override CrawlSyntaxNode VisitClassTypeDecleration(ClassTypeDeclerationNode classTypeDecleration)
        {
            return base.VisitClassTypeDecleration(classTypeDecleration);
        }

        //NB: This one might break, there was talk about refactoring all for loops to while loops behind the scenes
        protected override CrawlSyntaxNode VisitForLoop(ForLoopNode forLoop)
        {
            GenericScope scope = new GenericScope(new[]
            {
                new KeyValuePair<string, TypeInformation>(
                    forLoop.LoopVariable.Value,
                    new TypeInformation(
                        forLoop.Loopvariable.ActualType,
                        ProtectionLevel.NotApplicable,
                        forLoop.LoopVariable.Interval.a,
                        new UniqueItem(),
                        DeclaringScope.MethodLike)
                ),
            });

            ForLoopNode afterVisit = (ForLoopNode) base.VisitForLoop(forLoop);
            return afterVisit.WithScope(scope);
        }
    }
}