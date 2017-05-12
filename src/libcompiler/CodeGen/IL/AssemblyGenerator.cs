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

        #region skeletons

        public bool MakeTypeSkeleton(ClassTypeDeclerationNode type)
        {
            if (type.ClassType.Ancestor.ClrType == null) return false;

            TypeBuilder builder = _module.DefineType(type.ClassType.FullName, type.GetTypeAttributes(), type.ClassType.Ancestor.ClrType);
            type.ClassType.SetClrType(builder);
            return true;
        }

        public void MakeFreeMethodSkeleton(MethodDeclerationNode method)
        {
            var builder = MakeMethodSkeletonInternal(GetFreeStandingType(), method, MethodAttributes.Static);

            if (method.Identifier.Value == ReservedNames.Entrypoint)
                entryprt = builder;
        }

        public void MakeMemberSkeletons(ClassTypeDeclerationNode type, List<MethodDeclerationNode> listOfMethods)
        {
            TypeBuilder builder = (TypeBuilder)type.ClassType.ClrType;
            foreach (CrawlSyntaxNode node in type.Body)
            {
                VariableDeclerationNode asVar = node as VariableDeclerationNode;
                MethodDeclerationNode asMethod = node as MethodDeclerationNode;

                if (asVar != null)
                {
                    foreach (SingleVariableDeclerationNode decleration in asVar.Declerations)
                    {
                        EmitFieldInternal(builder, decleration, 0);
                    }
                }
                else if (asMethod != null)
                {
                    MakeMethodSkeletonInternal(builder, asMethod, 0);
                    listOfMethods.Add(asMethod);
                }
                else { throw new NotImplementedException(); }
            }
        }

        private MethodBuilder MakeMethodSkeletonInternal(TypeBuilder typeBuilder, MethodDeclerationNode method,
            MethodAttributes extraAttributes)
        {
            CrawlMethodType type = (CrawlMethodType) method.MethodSignature.ActualType;
            MethodBuilder builder = typeBuilder
                .DefineMethod(
                    method.Identifier.Value,
                    method.GetTypeAttributes() | extraAttributes,
                    CallingConventions.Standard,
                    type.ReturnType.ClrType,
                    type.Parameters.Select(t => t.ClrType).ToArray()
                );

            method.UniqueItemTracker.Item.MethodInfo = builder;
            return builder;
        }

        #endregion


        #region Emit
        public void EmitFreeMethod(MethodDeclerationNode method)
        {
            EmitMethodInternal(method);
        }

        public void EmitFreeVariable(SingleVariableDeclerationNode variable)
        {
            EmitFieldInternal(GetFreeStandingType(), variable, FieldAttributes.Static);
        }

        private void EmitFieldInternal(TypeBuilder containingType, SingleVariableDeclerationNode decl,
            FieldAttributes extraAttributes)
        {
            VariableDeclerationNode declList = (VariableDeclerationNode) decl.Parent.Parent;

            extraAttributes |= declList.GetFieldAttributes();
            FieldBuilder builder = containingType.DefineField(
                decl.Identifier.Name,
                declList.DeclerationType.ActualType.ClrType,
                extraAttributes);

            decl.Identifier.UniqueItemTracker.Item.FieldInfo = builder;
        }

        private void EmitMethodInternal( MethodDeclerationNode method)
        {
            new ILGeneratorVisitor(method.UniqueItemTracker.Item.MethodInfo).EmitFor(method.Body);
        }

        #endregion

        public void SetWorkingNamespace(string ns)
        {
            workingNamespace = ns ?? "";
        }




        #region Implementation



        #endregion





        public void MakeMethodSkeleton(TypeBuilder containingType, MethodDeclerationNode method,  MethodAttributes extraAttributes = 0)
        {
            CrawlMethodType type = (CrawlMethodType) method.MethodSignature.ActualType;
            MethodBuilder builder = containingType.DefineMethod(
                method.Identifier.Value,
                method.GetTypeAttributes() | extraAttributes,
                CallingConventions.Standard,
                type.ReturnType.ClrType,
                type.Parameters.Select(x => x.ClrType).ToArray());
            method.UniqueItemTracker.Item.MethodInfo = builder;
        }

        public void FinishClass(ClassTypeDeclerationNode type)
        {
            TypeBuilder builder = (TypeBuilder)type.ClassType.ClrType;
            builder.CreateType();
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



        #region  Should really have its own type

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

        #endregion
    }
}
