namespace libcompiler.SyntaxTree
{
    public enum ExpressionType
    {
        None = 0,
        Invocation,
        SubfieldAccess,
        Greater,
        GreaterEqual,
        Equal,
        NotEqual,
        LessEqual,
        Less,
        Index,
        Subtract,
        Power,
        Add,
        Multiply,
        Range,
        ShortCircuitOr,
        ShortCircuitAnd,
        Constant,
        Variable
    }
}