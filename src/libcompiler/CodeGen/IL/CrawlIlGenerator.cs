using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Antlr4.Runtime.Misc;
using libcompiler.CompilerStage;
using libcompiler.ExtensionMethods;
using libcompiler.SyntaxTree;

namespace libcompiler.CodeGen.IL
{
    class CrawlIlGenerator
    {
        public static AssemblyBuilder Generate(IEnumerable<AstData> files, CrawlCompilerConfiguration configuration, out string saveAs)
        {
            //Find a filename to save it to, also generate an assembly name. TODO: AssemblyName config
            saveAs = GetFileNameFor(configuration.DestinationFile, configuration.OutputType);
            AssemblyName name = new AssemblyName(Path.GetFileNameWithoutExtension(saveAs));


            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            ModuleBuilder mainModule = builder.DefineDynamicModule(name.Name, saveAs, true);
            
            AssemblyGenerator generator = new AssemblyGenerator(mainModule);
            
            List<VariableDeclerationNode> freeVariables = new List<VariableDeclerationNode>();
            List<MethodDeclerationNode> freeMethods = new List<MethodDeclerationNode>();
            List<ClassTypeDeclerationNode> types = new List<ClassTypeDeclerationNode>();

            foreach (AstData file in files)
            {
                TranslationUnitNode tu = (TranslationUnitNode) file.Tree.RootNode;
                generator.SetWorkingNamespace(tu.Namespace.Module);

                foreach (var tree in tu.Code)
                {
                    switch (tree.Type)
                    {
                        case NodeType.MethodDecleration:
                            freeMethods.Add((MethodDeclerationNode) tree);
                            break;

                        case NodeType.ClassTypeDecleration:
                            types.Add((ClassTypeDeclerationNode) tree);
                            break;

                        case NodeType.VariableDecleration:
                            freeVariables.Add((VariableDeclerationNode) tree);
                            break;

                        default: throw new NotImplementedException();
                    }
                }
            }

            for (int index = 0; index < types.Count; index++)
            {
                //Emit types one after another. If dependency is now emitted yet, move to back
                //TODO: detect circular dependency
                ClassTypeDeclerationNode type = types[index];
                bool success = generator.EmitClassFirstPass(type);
                if (!success)
                {
                    types[index] = null;
                    types.Add(type);
                }
            }

            foreach (MethodDeclerationNode node in freeMethods)
            {
                generator.EmitFreeMethod(node);
            }

            foreach (VariableDeclerationNode node in freeVariables)
            {
                generator.EmitFreeVariable(node);
            }

            foreach (ClassTypeDeclerationNode type in types.Where(type => type != null))
            {
                generator.EmitClassSecondPass(type);
            }

            generator.Finish(builder, configuration.OutputType);
            return builder;
        }

        
        private static string GetFileNameFor(string name, PEFileKinds type)
        {
            string recomendedExtension = type == PEFileKinds.Dll ? "dll" : "exe";

            if (name == null)
            {
                name = "out";
            }


            if (!Path.HasExtension(name))
                name = name + "." + recomendedExtension;

            return name;
        }
    }
}