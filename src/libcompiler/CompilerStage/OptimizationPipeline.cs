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
    }
}