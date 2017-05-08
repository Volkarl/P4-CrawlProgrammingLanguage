using System.Collections.Generic;
using System.Linq;
using libcompiler.Scope;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage.SemanticAnalysis
{
    internal class AddScopeVisitor : SyntaxRewriter
    {
        protected override CrawlSyntaxNode VisitBlock(BlockNode block)
        {
            BlockScope scope = new BlockScope(block);
            var tmp = ((BlockNode) base.VisitBlock(block));
            var after = tmp.WithScope(scope);
            return after;
        }

        protected override CrawlSyntaxNode VisitMethodDecleration(MethodDeclerationNode methodDecleration)
        {
            GenericScope scope = new GenericScope(
                methodDecleration
                    .Parameters
                    .Select(
                        parameter =>
                            new KeyValuePair<string, TypeInformation>(parameter.Value,
                                new TypeInformation(
                                    null,
                                    ProtectionLevel.NotApplicable,
                                    parameter.Interval.b
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
                        null,
                        ProtectionLevel.NotApplicable,
                        forLoop.LoopVariable.Interval.a,
                        DeclaringScope.MethodLike)
                ),
            });

            ForLoopNode afterVisit = (ForLoopNode) base.VisitForLoop(forLoop);
            return afterVisit.WithScope(scope);
        }
    }
}