namespace libcompiler.SyntaxTree
{
    public enum ExpressionType
    {
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
        ShortCircuitAnd
    }
}