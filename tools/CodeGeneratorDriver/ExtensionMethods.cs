using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeGeneratorDriver
{
    public static class ExtensionMethods
    {
        public static TypeSyntax RedBase(this Options options)
        {
            return SyntaxFactory.ParseTypeName(SharedGeneratorion.RedNodeName(options.BaseName));
        }

        public static TypeSyntax GreenBase(this Options options)
        {
            return SyntaxFactory.ParseTypeName(SharedGeneratorion.GreenNodeName(options.BaseName));
        }

        public static IEnumerable<Property> AllProperties(this Node node)
        {
            if (node.BaseNode == null)
                return node.Properties;

            return node.BaseNode.AllProperties().Concat(node.Properties);
        }

        public static IEnumerable<Child> AllChildren(this Node node)
        {
            if (node.BaseNode == null)
                return node.Children;

            return node.BaseNode.AllChildren().Concat(node.Children);
        }

        public static string AsParameter(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            return char.ToLower(s[0]) + s.Substring(1);
        }

        public static string NonGenericPart(this string s) => s.Split('\'')[0];
    }
}