namespace libcompiler.SyntaxTree
{
    public enum NodeType
    {
        Forloop,
        If,
        IfElse,
        While,
        Return,
        Assignment,
        Call,
        Index,
        MultiExpression,
        BinaryExpression,
        UnaryExpression,
        Variable,
        ClassDecleration,
        VariableDecleration,
        VariableDeclerationSingle,
        MethodDecleration,
        Block,
        Imports,
        Import,
        TranslationUnit,
        Literal,
        NodeList,
        Type,
        GenericUnpack,
        GenericParametersNode,
        Reference,
        Identifier,
        NameSpace,
        Parameter
    }
}