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

            //Builder classes that we just need
            AssemblyBuilder builder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Save);
            ModuleBuilder mainModule = builder.DefineDynamicModule(name.Name, saveAs, true);
            
            AssemblyGenerator generator = new AssemblyGenerator(mainModule);


            //Lists of everything we plan to emit
            List<SingleVariableDeclerationNode> freeVariables = new List<SingleVariableDeclerationNode>();
            List<MethodDeclerationNode> freeMethods = new List<MethodDeclerationNode>();
            List<ClassTypeDeclerationNode> types = new List<ClassTypeDeclerationNode>();

            //Sort everything in all the files
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
                            freeVariables.AddRange(((VariableDeclerationNode) tree).Declerations);
                            break;

                        default: throw new NotImplementedException();
                    }
                }
            }

            //Generate skeleton for all classes. This needs to be done before anything referencing those classes can be emitted
            for (int index = 0; index < types.Count; index++)
            {
                //Emit types one after another. If dependency is now emitted yet, move to back
                //TODO: detect circular dependency
                ClassTypeDeclerationNode type = types[index];
                bool success = generator.MakeTypeSkeleton(type);
                if (!success)
                {
                    types[index] = null;
                    types.Add(type);
                }
            }
            types = types.Where(x => x != null).ToList();





            //Now generate skeletons for all methods
            foreach (MethodDeclerationNode method in freeMethods)
            {
                generator.MakeFreeMethodSkeleton(method);
            }
            foreach (ClassTypeDeclerationNode type in types)
            {
                generator.MakeMemberSkeletons(type, freeMethods);
            }

            //Free variables needs to be done before methods, thats all
            foreach (SingleVariableDeclerationNode node in freeVariables)
            {
                generator.EmitFreeVariable(node);
            }

            //Emit methods
            foreach (MethodDeclerationNode method in freeMethods)
            {
                generator.EmitFreeMethod(method);
            }


            foreach (ClassTypeDeclerationNode type in types.Where(type => type != null))
            {
                generator.FinishClass(type);
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