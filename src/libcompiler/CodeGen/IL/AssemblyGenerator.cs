using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.CodeGen.IL
{
    class AssemblyGenerator
    {
        private readonly ModuleBuilder _module;
        private string workingNamespace = "";
        private MethodBuilder entryprt;

        public AssemblyGenerator(ModuleBuilder assemblySingleModule)
        {
            _module = assemblySingleModule;
        }

        public void EmitFreeMethod(MethodDeclerationNode method)
        {
            MethodBuilder finishedMethod = EmitMethodInternal(
                GetFreeStandingType(), 
                method, 
                MethodAttributes.Static | MethodAttributes.HideBySig
            );

            if (method.Identifier.Value == ReservedNames.Entrypoint)
            {
                entryprt = finishedMethod;   
            }
        }

        public void EmitFreeVariable(SingleVariableDeclerationNode variable)
        {
            EmitFieldInternal(GetFreeStandingType(), variable, FieldAttributes.Static);
        }

        public bool EmitClassFirstPass(ClassTypeDeclerationNode type)
        {
            if (type.ClassType.Ancestor.ClrType == null) return false;

            TypeBuilder builder = _module.DefineType(type.ClassType.FullName, type.GetTypeAttributes(), type.ClassType.Ancestor.ClrType);
            type.ClassType.SetClrType(builder);
            return true;
        }

        public void EmitClassSecondPass(ClassTypeDeclerationNode type)
        {
            TypeBuilder builder = (TypeBuilder)type.ClassType.ClrType;

            foreach (MethodDeclerationNode node in type.Body.OfType<MethodDeclerationNode>())
            {
                EmitMethodInternal(builder, node);
            }

            builder.CreateType();
        }

        private MethodBuilder EmitMethodInternal(TypeBuilder containingType, MethodDeclerationNode method, MethodAttributes extraAttributes = 0)
        {
            CrawlMethodType type = (CrawlMethodType) method.MethodSignature.ActualType;
            MethodBuilder builder = containingType.DefineMethod(
                method.Identifier.Value, 
                method.GetTypeAttributes() | extraAttributes, 
                CallingConventions.Standard,
                type.ReturnType.ClrType, 
                type.Parameters.Select(x => x.ClrType).ToArray());


            new ILGeneratorVisitor(builder).EmitFor(method.Body);

            return builder;
        }

        private void EmitFieldInternal(TypeBuilder containingType, SingleVariableDeclerationNode decl,
            FieldAttributes extraAttributes)
        {

            VariableDeclerationNode declList = (VariableDeclerationNode) decl.Parent.Parent;


            extraAttributes |= declList.GetFieldAttributes();
            FieldBuilder builder = containingType.DefineField(decl.Identifier.Name, declList.DeclerationType.ActualType.ClrType,
                extraAttributes);

            decl.Identifier.UniqueItemTracker.Item.FieldInfo = builder;

            //Probably most certainly needs to save this somewhere
        }

        public void SetWorkingNamespace(string ns)
        {
            workingNamespace = ns ?? "";
        }

        public void Finish(AssemblyBuilder builder, PEFileKinds kind)
        {
            foreach (TypeBuilder typeBuilder in _freeStandingTypes.Values)
            {
                typeBuilder.CreateType();
            }
            if (entryprt != null)
            {
                builder.SetEntryPoint(entryprt, kind);
            }
        }

        private readonly Dictionary<string, TypeBuilder> _freeStandingTypes = new Dictionary<string, TypeBuilder>();
        TypeBuilder GetFreeStandingType()
        {
            TypeBuilder builder;
            if (!_freeStandingTypes.TryGetValue(workingNamespace, out builder))
            {
                string freeStandingName;
                if (string.IsNullOrWhiteSpace(workingNamespace))
                {
                    freeStandingName = "CRAWL_FREESTANDING";
                }
                else
                {
                    freeStandingName = workingNamespace + ".CRAWL_FREESTANDING";
                }
                builder = _module.DefineType(freeStandingName, TypeAttributes.Public );
                _freeStandingTypes[workingNamespace] = builder;
            }

            return builder;
        }
        
    }
}
