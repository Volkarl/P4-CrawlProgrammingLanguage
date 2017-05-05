using System.Collections.Concurrent;
using System.Linq;
using libcompiler.CompilerStage.SemanticAnalysis;
using libcompiler.Namespaces;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage
{
    internal class SemanticAnalysisPipeline
    {
        internal class NamespaceDecorator
        {
            private readonly ConcurrentDictionary<string, Namespace> _allNamespaces;

            public NamespaceDecorator(ConcurrentDictionary<string, Namespace> allNamespaces)
            {
                _allNamespaces = allNamespaces;
            }

            public AstData AddExport(AstData arg, SideeffectHelper helper)
            {
                var tu = ((TranslationUnitNode) arg.Tree.RootNode);
                try
                {
                    return new AstData(
                        arg.TokenStream,
                        arg.Filename,
                        tu
                            .WithImportedNamespaces(
                                Namespace.Merge(tu.Imports.Select(TryGetNamespace).ToArray())
                            )
                            .OwningTree
                    );
                }
                catch (NamespaceNotFoundException nsnfe)
                {
                    throw helper.FailWith(CompilationMessage.Create(arg.TokenStream, nsnfe.FaultyNode.Interval,
                        MessageCode.NamespaceNotFound, arg.Filename));
                }
            }

            private Namespace TryGetNamespace(ImportNode arg)
            {
                Namespace ns;
                if (_allNamespaces.TryGetValue(arg.Module, out ns))
                    return ns;

                throw new NamespaceNotFoundException(arg);
            }
        }

        internal static AstData PutTypes(AstData arg, SideeffectHelper helper)
        {
            return new AstData(
                arg.TokenStream,
                arg.Filename,
                new PutTypeVisitor(helper.CompilationMessages, arg.TokenStream, arg.Filename).Visit(arg.Tree.RootNode)
                    .OwningTree);
        }

        internal static AstData DeclerationOrderCheck(AstData arg, SideeffectHelper helper)
        {
            new CheckDeclerationOrderVisitor(helper.CompilationMessages, arg).Visit(arg.Tree.RootNode);
            return arg;
        }

        internal static AstData CollectScopeInformation(AstData withoutScope, SideeffectHelper notused)
        {
            return new AstData(
                withoutScope.TokenStream,
                withoutScope.Filename,
                new AddScopeVisitor().Visit(withoutScope.Tree.RootNode).OwningTree);
        }
    }
}