using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;

namespace libcompiler.ExtensionMethods
{
    static class CodeGenHelper
    {
        public static string AsCSharpString(this ProtectionLevel protectionLevel)
        {
            switch (protectionLevel)
            {

                case ProtectionLevel.Public:
                case ProtectionLevel.None:
                    return "public";
                case ProtectionLevel.Internal:
                    return "internal";
                case ProtectionLevel.Protected:
                    return "protected";
                case ProtectionLevel.ProtectedInternal:
                    return "protected internal";
                case ProtectionLevel.Private:
                    return "private";
                case ProtectionLevel.NotApplicable:
                    return "";
                default:
                    throw new ArgumentOutOfRangeException(nameof(protectionLevel), protectionLevel, null);
            }
        }
    }
}
