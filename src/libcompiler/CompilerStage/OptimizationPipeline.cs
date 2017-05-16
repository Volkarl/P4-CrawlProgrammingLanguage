using System.Runtime.Remoting;
using libcompiler.Optimizations;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage
{
    public class OptimizationPipeline
    {
        internal static AstData FoldConstants(AstData astData, SideeffectHelper helper)
        {
            var constantFoldingVisitor = new ConstantFoldingVisitor();
            var operandSortingVisitor = new OperandSortingVisitor();

            var newTree = astData.Tree.RootNode;
            do
            {
                constantFoldingVisitor.OptimizationsWereMade = false;
                newTree = operandSortingVisitor.Visit(newTree);
                newTree = constantFoldingVisitor.Visit(newTree);
            } while (constantFoldingVisitor.OptimizationsWereMade);

            return new AstData(astData.TokenStream, astData.Filename, newTree.OwningTree);
        }

        /// <summary>
        /// Changes parameters to ref where possible to avoid deep cloning.
        /// </summary>
        internal static AstData RefWherePossible(AstData astData, SideeffectHelper helper)
        {
            var visitor = new RefParametersVisitor();
            var newTree = visitor.Visit(astData.Tree.RootNode).OwningTree;

            return new AstData(astData.TokenStream, astData.Filename, newTree);
        }
    }
}