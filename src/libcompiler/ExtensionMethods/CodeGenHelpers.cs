using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.ExtensionMethods
{
    static class CodeGenHelpers
    {
        public static MethodAttributes GetTypeAttributes(this MethodDeclerationNode self)
        {
            MethodAttributes attributes = MethodAttributes.PrivateScope; 
            switch (self.ProtectionLevel)
            {
                case ProtectionLevel.None:
                    break;
                case ProtectionLevel.Public:
                    attributes = MethodAttributes.Public;
                    break;
                case ProtectionLevel.Internal:
                    attributes = MethodAttributes.Assembly;
                    break;
                case ProtectionLevel.Protected:
                    attributes = MethodAttributes.Family;
                    break;
                case ProtectionLevel.ProtectedInternal:
                    attributes = MethodAttributes.FamANDAssem;
                    break;
                case ProtectionLevel.Private:
                    attributes = MethodAttributes.Private;
                    break;
                case ProtectionLevel.NotApplicable:
                    break;
            }

            return attributes;
        }

        public static TypeAttributes GetTypeAttributes(this ClassTypeDeclerationNode self)
        {
            TypeAttributes attributes = TypeAttributes.Public;
            switch (self.ProtectionLevel)
            {
                case ProtectionLevel.None:
                    break;
                case ProtectionLevel.Public:
                    attributes = TypeAttributes.Public;
                    break;
                case ProtectionLevel.Internal:
                    attributes = TypeAttributes.NestedAssembly;
                    break;
                case ProtectionLevel.Protected:
                    attributes = TypeAttributes.NestedFamily;
                    break;
                case ProtectionLevel.ProtectedInternal:
                    attributes = TypeAttributes.NestedFamORAssem;
                    break;
                case ProtectionLevel.Private:
                    attributes = TypeAttributes.NestedAssembly;
                    break;
                case ProtectionLevel.NotApplicable:
                    break;
            }

            return attributes;
        }

        public static TypeToken GetTypeToken(this CrawlType self, ModuleBuilder module)
        {
            return module.GetTypeToken(self.FullName);

        }
    }
}
