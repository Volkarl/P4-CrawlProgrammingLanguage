namespace libcompiler.SyntaxTree
{
    //TODO: Should probably maybe be in root or a TypeSystem folder
    public enum ProtectionLevel
    {
        None,
        Public,
        Internal,
        Protected,
        ProtectedInternal,
        Private,
        //Literally, it is not applicable. Method parameters ect
        NotApplicable
    }
}