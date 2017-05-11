using libcompiler.CompilerStage.CodeGen;

namespace libcompiler.CompilerStage
{
    public class CodeGenPipeline
    {
        internal static string GenerateCode(AstData tree, SideeffectHelper notUsed)
        {
            return new WriteCsPlaintextVisitor().Visit(tree.Tree.RootNode);
        }
    }
}