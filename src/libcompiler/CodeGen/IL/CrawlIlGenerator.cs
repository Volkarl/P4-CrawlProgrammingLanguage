using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using libcompiler.CompilerStage;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;

namespace libcompiler.CodeGen.IL
{
    class CrawlIlGenerator
    {
        private const string ASSEMBLY_NAME = "CRAWL_ASSEMBLY";
        public static AssemblyBuilder Generate(IEnumerable<AstData> files)
        {
            //Needs not be RunAndSave, but for testing, Run is probably nice
            AssemblyName name = new AssemblyName(ASSEMBLY_NAME);
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);

            string asmExeName = ASSEMBLY_NAME + ".dll";


            ModuleBuilder mainModule = builder.DefineDynamicModule(asmExeName, asmExeName, true);
            TypeBuilder bigtype = mainModule.DefineType("global", TypeAttributes.BeforeFieldInit | TypeAttributes.Public);
            bigtype.DefineField("baz", typeof(int), FieldAttributes.Public);
            
            foreach (AstData file in files)
            {
                TranslationUnitNode tu = (TranslationUnitNode) file.Tree.RootNode;
                foreach (var tree in tu.Code)
                {
                    switch (tree.Type)
                    {
                        case NodeType.MethodDecleration:
                        {
                            GenerateMethod((MethodDeclerationNode) tree, bigtype);
                        }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            bigtype.CreateType();
            mainModule.CreateGlobalFunctions();
            return builder;
        }

        private static void GenerateMethod(MethodDeclerationNode method, TypeBuilder owningType)
        {
            MethodBuilder mb = owningType.DefineMethod(method.Identifier.Value, MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard, typeof(int), new Type[0]);

            ILGenerator ilGenerator = mb.GetILGenerator();
            new ILGeneratorVisitor(ilGenerator).Visit(method.Body);
            
        }

        private static void GenerateMethod(MethodDeclerationNode method, ModuleBuilder owningModule)
        {
            MethodBuilder mb = owningModule.DefineGlobalMethod(method.Identifier.Value, MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard, typeof(int), new Type[0]);

            ILGenerator ilGenerator = mb.GetILGenerator();
            new ILGeneratorVisitor(ilGenerator).Visit(method.Body);

        }
    }
}