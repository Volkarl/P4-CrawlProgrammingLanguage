namespace libcompiler.CompilerStage
{
    class Rewriters
    {
        public static AstData MoveDeclerations(AstData arg1, SideeffectHelper arg2)
        {
            return new AstData(
                arg1.TokenStream,
                arg1.Filename,
                new MoveDeclerationsRewriter().Visit(arg1.Tree.RootNode).OwningTree
            );
        }
    }
}
