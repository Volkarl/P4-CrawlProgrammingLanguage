using System;
using libcompiler.SyntaxTree;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneratorDriver
{
    public class Member
    {
        public bool IsNode { get; }
        public bool NullAllowed { get; }
        public bool IsList => Type == "List";

        public string Name { get; }
        public string Type { get; }
        private readonly string _genericPart;

        public Member(Child child)
        {
            string[] parts = child.Type.Split('\'');
            Type = parts[0];
            if (parts.Length == 2)
                _genericPart = parts[1];

            Name = child.Name;

            IsNode = true;
            NullAllowed = child.NullDefault;
        }

        public Member(Property property)
        {
            string[] parts = property.Type.Split('\'');
            Type = parts[0];
            if (parts.Length == 2)
                _genericPart = parts[1];

            Name = property.Name;
        }

        public bool IsImplicitlyAssigned(string name)
        {
            if (Type == "NodeType")
                return true;

            ExpressionType type;
            if (Type == "ExpressionType" && Enum.TryParse(name, true, out type))
                return true;

            return false;
        }

        public TypeSyntax GetRepresentation(TypeClassContext context = TypeClassContext.None)
        {
            string name;

            if ((context & TypeClassContext.Array) != 0)
            {
                name = $"{GenerateGenericType(_genericPart, context)}[]";
            }
            else
            {
                if (_genericPart != null)
                {
                    name = $"{GenerateTypeName(Type, context)}<{GenerateGenericType(_genericPart, context)}>";
                }
                else
                {
                    name = GenerateTypeName(Type, context);
                }
            }
            return SyntaxFactory.ParseTypeName(name);
        }

        private string GenerateGenericType(string genericPart, TypeClassContext context)
        {
            if (IsNode)
            {


                return $"{genericPart}Nodes";
            }
            else if ((context & TypeClassContext.GreenIEnumerableParameter) != 0)
                return $"Green{genericPart}Nodes";
            return genericPart;
        }

        private string GenerateTypeName(string type, TypeClassContext context)
        {
            if (IsList && (context & TypeClassContext.NotList) != 0)
                return "IEnumerable";

            if (IsNode)
            {
                if ((context & TypeClassContext.Red) != 0)
                {
                    return $"{type}Nodes";
                }
                else
                {
                    return $"Green{type}Nodes";
                }
            }
            else
            {
                return type;
            }

        }

        public string ParameterName()
        {
            if (string.IsNullOrEmpty(Name)) return Name;

            return char.ToLower(Name[0]) + Name.Substring(1);
        }

        public string PropertyName() => Name;

    }

    [Flags]
    public enum TypeClassContext
    {
        None = 0,

        Red = 1 << 0,
        NotList = 1 << 1,
        Array = 1 << 2,
        GreenIEnumerableParameter = 1 << 3
    }
}