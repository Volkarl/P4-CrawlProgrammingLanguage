namespace libcompiler.SyntaxTree
{
    public enum ExpressionType
    {
        None = 0,
        Call,
        MemberAccess,
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
        RealLiteral,
        StringLiteral,
        IntegerLiteral,
        BooleanLiteral,
        Variable,
        Negate,
        Not,
        Divide,
        Modulous,
        GenericsUnpack,
        ArrayConstructor,
        Cast
    }

   
}