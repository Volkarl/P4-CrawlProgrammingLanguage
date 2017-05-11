using System.Collections.Concurrent;
using System.Linq;
using libcompiler.CompilerStage.SemanticAnalysis;
using libcompiler.Namespaces;
using libcompiler.SyntaxTree;
using libcompiler.TypeChecker;

namespace libcompiler.CompilerStage
{
    internal class SemanticAnalysisPipeline
    {

        /// <summary>
        /// On all blocknodes, all types are added. No other scope information exists once this finishes
        /// </summary>
        public static AstData CollectTypes(AstData data, SideeffectHelper helper)
        {
            return new AstData(
                data.TokenStream,
                data.Filename,
                new FirstScopePassVisitor().Visit(data.Tree.RootNode).OwningTree);
        }

        /// <summary>
        /// Adds the global namespace with all types.
        /// </summary>
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


        /// <summary>
        /// Adds type to all typenodes. Outputs error messages if types are not found. No regard for private types atm
        /// </summary>
        internal static AstData PutTypes(AstData arg, SideeffectHelper helper)
        {
            return new AstData(
                arg.TokenStream,
                arg.Filename,
                new PutTypeVisitor(helper.CompilationMessages, arg.TokenStream, arg.Filename).Visit(arg.Tree.RootNode)
                    .OwningTree);
        }

        /// <summary>
        /// Checks that variables are only used after they are declared
        /// </summary>
        internal static AstData DeclerationOrderCheck(AstData arg, SideeffectHelper helper)
        {
            new CheckDeclerationOrderVisitor(helper.CompilationMessages, arg).Visit(arg.Tree.RootNode);
            return arg;
        }

        /// <summary>
        /// Adds everything else to scope (Variables, methods)
        /// </summary>
        internal static AstData SecondScopePass(AstData withoutScope, SideeffectHelper notused)
        {
            return new AstData(
                withoutScope.TokenStream,
                withoutScope.Filename,
                new AddScopeVisitor().Visit(withoutScope.Tree.RootNode).OwningTree);
        }

        /// <summary>
        /// Check that expressions are used on correct types.
        /// </summary>
        internal static AstData TypeCheck(AstData tree, SideeffectHelper notused)
        {
            var newTree = new TypeVisitor().Visit(tree.Tree.RootNode).OwningTree;
            return new AstData(tree.TokenStream, tree.Filename, newTree);
        }

        /// <summary>
        /// Finishes parsing types
        /// </summary>
        public static AstData FinishTypes(AstData arg1, SideeffectHelper arg2)
        {
            new FinishTypesVisitor().Visit(arg1.Tree.RootNode);
            return arg1;
        }
    }

    internal class FinishTypesVisitor : SyntaxVisitor
    {
        protected override void VisitClassTypeDecleration(ClassTypeDeclerationNode node)
        {
            node.ClassType.Initialize(node);

            base.VisitClassTypeDecleration(node);
        }
    }
}