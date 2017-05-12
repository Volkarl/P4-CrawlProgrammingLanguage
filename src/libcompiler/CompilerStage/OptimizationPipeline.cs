using libcompiler.Optimizations;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage
{
    public class OptimizationPipeline
    {
        internal static AstData FoldConstants(AstData astData, SideeffectHelper helper)
        {
            var visitor = new ConstantFoldingVisitor();

            var newTree = astData.Tree.RootNode;
            do
            {
                visitor.OptimizationsWereMade = false;
                 newTree = visitor.Visit(newTree);
            } while (visitor.OptimizationsWereMade);

            return new AstData(astData.TokenStream, astData.Filename, newTree.OwningTree);
        }
    }
}