using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using libcompiler.SyntaxTree;
using libcompiler.TypeChecker;
using libcompiler.TypeSystem;

namespace libcompiler.Namespace
{
    public static class NamespaceLoader
    {
        public static List<Namespace> LoadFrom(string assemblyName)
        {
            Assembly assembly = Assembly.ReflectionOnlyLoadFrom(assemblyName);
            return LoadInner(assembly);
        }

        public static List<Namespace> Load(string assemblyString)
        {
            Assembly assembly = Assembly.ReflectionOnlyLoad(assemblyString);
            return LoadInner(assembly);
        }

        private static List<Namespace> LoadNonReflection(string assemblyName)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyName);
            return LoadInner(assembly);
        }

        public static List<Namespace> LoadSmart(string assembly)
        {
            //I'm only 70% sure those descriptions are accurate...
            List<Tuple<string, Func<string, List<Namespace>>>> loaders = new List<Tuple<string, Func<string, List<Namespace>>>>()
            {
                new Tuple<string, Func<string, List<Namespace>>>("file", LoadFrom),
                new Tuple<string, Func<string, List<Namespace>>>("global assembly cache", Load),
                new Tuple<string, Func<string, List<Namespace>>>("file (Hail marry)", LoadNonReflection)
            };


            foreach (Tuple<string, Func<string, List<Namespace>>> loader in loaders)
            {
                try
                {
                    TraceListners.AssemblyResolverListner?.WriteLine($"Trying to load {assembly} as {loader.Item1}",
                        "AssemblyResolver");
                    return loader.Item2(assembly);
                }
                catch (FileNotFoundException e)
                {
                    TraceListners.AssemblyResolverListner?.WriteLine($"Failed to load {assembly} as {loader.Item1} due FileNotFound",
                        "AssemblyResolver");
                }
                catch (ReflectionTypeLoadException e)
                {
                    TraceListners.AssemblyResolverListner?.WriteLine($"Failed to load {assembly} as {loader.Item1} due ReflectionTypeLoad",
                        "AssemblyResolver");
                }
            }

            throw new Exception("Failed to load assembly");
        }

        public static ConcurrentDictionary<string, Namespace>  LoadAll(IEnumerable<string> assemblyName)
        {
            ConcurrentDictionary<string, Namespace> allNameSpaces = new ConcurrentDictionary<string, Namespace>();
            foreach (string name in assemblyName)
            {
                foreach (Namespace ns in LoadSmart(name))
                {
                    allNameSpaces.AddOrUpdate(ns.Name, ns, (key, oldNamespace) => Namespace.Merge(oldNamespace, ns));
                }
            }
            return allNameSpaces;
        }

        private static List<Namespace> LoadInner(Assembly assembly)
        {
            return
                assembly.ExportedTypes
                    .GroupBy(x => x.Namespace)
                    .Select(ns =>
                        new Namespace(
                            ns.Key,
                            ns
                                .Select(type =>
                                    new CrawlSimpleType(type)
                                )
                        )
                    )
                    .ToList();
        }
    }

    //This would really be much more elegant if it could derive from type
    [DebuggerDisplay("{Name}({_scope.Count} items)")]
    public class Namespace : IScope
    {
        public string Name { get; }
        private readonly ConcurrentDictionary<string, TypeInformation[]> _scope = new ConcurrentDictionary<string, TypeInformation[]>();

        public Namespace(string name, IEnumerable<CrawlType> contents) : this(name)
        {
            foreach (CrawlType type in contents)
            {
                _scope.TryAdd(type.Identifier, new[] {new TypeInformation(type, ProtectionLevel.Public)});
            }
        }

        public Namespace(string name)
        {
            Name = name;
        }


        public TypeInformation[] FindSymbol(string symbol)
        {
            throw new NotImplementedException();
        }

        public static Namespace Merge(params Namespace[] namespaces)
        {
            if(namespaces.Select(x => x.Name).Distinct().Count() > 1)
                throw new ArgumentException("Cannot merge namespaces with different names");

            string name = namespaces.First().Name;

            //Pull out every value in each namespace, then pull out individual TypeInformation, then pull out the CrawlType
            return new Namespace(name,
                namespaces
                    .SelectMany(ns => ns._scope.Values)
                    .SelectMany(tinf => tinf)
                    .Select(tinf => tinf.Type)
            );
        }
    }
}