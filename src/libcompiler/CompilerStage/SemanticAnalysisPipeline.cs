using System;
using System.Collections.Concurrent;
using Antlr4.Runtime.Tree;
using libcompiler.SyntaxTree;
using libcompiler.TypeChecker;

namespace libcompiler.CompilerStage
{
    internal class SemanticAnalysisPipeline
    {
        private readonly ConcurrentBag<CompilationMessage> _messages;

        private SemanticAnalysisPipeline(ConcurrentBag<CompilationMessage> messages)
        {
            _messages = messages;
        }

        public static Action<AstData> DataCollection(ConcurrentBag<AstData> astDestination,
            ConcurrentBag<CompilationMessage> messages, TargetStage stopAt)
        {
            SemanticAnalysisPipeline self = new SemanticAnalysisPipeline(messages);
            Func<AstData, AstData> collect = self.CollectScopeInformation;
            return collect.EndWith(astDestination.Add);

        }

        private AstData CollectScopeInformation(AstData withoutScope)
        {
            return new AstData(
                withoutScope.TokenStream,
                withoutScope.Filename,
                new AddScopeVisitor().Visit(withoutScope.Tree.RootNode).OwningTree);
        }

        private class AddScopeVisitor : SyntaxRewriter
        {
            protected override CrawlSyntaxNode VisitBlock(BlockNode block)
            {
                BlockScope scope = new BlockScope(block);
                return ((BlockNode)base.VisitBlock(block)).WithScope(scope);
            }

            protected override CrawlSyntaxNode VisitMethodDecleration(MethodDeclerationNode methodDecleration)
            {
                return base.VisitMethodDecleration(methodDecleration);
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
                return base.VisitForLoop(forLoop);
            }


        }
    }
}