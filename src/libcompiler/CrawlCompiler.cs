﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using libcompiler.CompilerStage;
using libcompiler.ExtensionMethods;
using libcompiler.Namespaces;
using libcompiler.SyntaxTree;

namespace libcompiler
{
    public static class CrawlCompiler
    {
        public static CompilationResult Compile(CrawlCompilerConfiguration configuration)
        {
            TextWriter output = Utils.GetPrimaryOutputStream(configuration);
            //TraceListners.AssemblyResolverListner =
            //    new System.Diagnostics.TextWriterTraceListener(Utils.GetPrimaryOutputStream(configuration));
            ConcurrentDictionary<string, Namespace> allNamespaces = NamespaceLoader.LoadAll(configuration.Assemblies);
            allNamespaces.MergeInto(Namespace.BuiltinNamespace.AsSingleIEnumerable());

            SideeffectHelper sideeffectHelper = new SideeffectHelper();
            CompilationStatus status = CompilationStatus.Success;

            try
            {
                
                bool parallel = !configuration.ForceSingleThreaded;

                //The ConcurrentBag is an unordered, thread safe, collection
                ConcurrentBag<AstData> parsedFiles = Run<AstData, string>(configuration.Files, parallel, sideeffectHelper,
                    (destination, helper) =>
                    {
                        //Syntax is slightly wonky, but cannot assign variable to method group.
                        //_Could_ be hidden in ParsePipeline by making them properties instead....
                        
                        //Get the starting transformaton
                        Func<string, SideeffectHelper, ParseTreeData> parsePT = ParsePipeline.ReadFileToPt;

                        //Jump out if we are intrested in earlier stage.
                        if (configuration.TargetStage == TargetStage.ParseTree) return parsePT.EndWith(output.WriteLine, helper);
                        
                        //.Then adds another stage
                        var parseASt = parsePT 
                        .Then(ParsePipeline.CreateAst);

                        if (configuration.TargetStage == TargetStage.AbstractSyntaxTree)
                            return parseASt.EndWith(output.WriteLine, helper);

                        var firstscopepass = parseASt.Then(SemanticAnalysisPipeline.CollectTypes);

                        //.EndWith collects it
                        return firstscopepass.EndWith(destination.Add, helper);
                    }
                );

                MaybeDie(sideeffectHelper);

                foreach (AstData file in parsedFiles)
                {
                    TranslationUnitNode node = (TranslationUnitNode) file.Tree.RootNode;
                    Namespace exportedTypes = new Namespace(node.Namespace.Module, node.Code.Scope.Classes());

                    allNamespaces.MergeInto(exportedTypes.AsSingleIEnumerable());
                }

                //TODO: finish Semantic analysis
                ConcurrentBag<AstData> filesWithScope = Run<AstData, AstData>(parsedFiles, parallel, sideeffectHelper,
                    (destination, helper) =>
                    {
                        //NamespaceDecorator is a hack to pass all namespaces in to the function that finds the relevant ones
                        Func<AstData, SideeffectHelper, AstData> first = new SemanticAnalysisPipeline.NamespaceDecorator(allNamespaces).AddExport;

                        return first
                            .Then(SemanticAnalysisPipeline.PutTypes)
                            .Then(SemanticAnalysisPipeline.SecondScopePass)
                            .Then(SemanticAnalysisPipeline.FinishTypes)
                            .EndWith(destination.Add, helper);

                    }
                );

                //TODO: Merge namespaces again with extra (Methods/Variables) from scope. Right now, only classes are visible
                //in other files

                ConcurrentBag<AstData> decoratedAsts = Run<AstData, AstData>(filesWithScope, parallel, sideeffectHelper,
                    (destination, helper) =>
                    {
                        Func<AstData, SideeffectHelper, AstData> first = SemanticAnalysisPipeline.DeclerationOrderCheck;
                        var final = first.EndWith(destination.Add, helper);  //Typechecker would be added here or line above

                        return final;
                    }
                );


                //TODO: Interpeter or code generation

                //Until meaningfull end, print everything

                Execute(decoratedAsts, output.WriteLine, parallel);

                if (sideeffectHelper.CompilationMessages.Count(message => message.Severity >= MessageSeverity.Error) > 0)
                    status = CompilationStatus.Failure;
                
            }
            catch(ExitCompilationException)
            { status = CompilationStatus.Failure; }
            catch (Exception e) when(false && !Debugger.IsAttached)
            {

                sideeffectHelper.CompilationMessages.Add(CompilationMessage.CreateNonCodeMessage(MessageCode.InternalCompilerError, e.ToString(), MessageSeverity.Fatal));
                status = CompilationStatus.Failure;

            }

            return new CompilationResult(status, sideeffectHelper.CompilationMessages);
        }

        private static void MaybeDie(SideeffectHelper sideeffectHelper)
        {
            if (sideeffectHelper.CompilationMessages.Count(message => message.Severity >= MessageSeverity.Error) > 0)
                throw new ExitCompilationException();
        }

        //No real difference between the meaning of execute and run, except it would be bad form to name them the same.
        //Run makes Execute less insane to use, but limits it to outputting Seperate data. All of them (for example) writing to an exe file is not possible
        private static ConcurrentBag<TOut> Run<TOut, TIn>(
            IEnumerable<TIn> indata, 
            bool parallel,
            SideeffectHelper helper,
                //The type of a function that takes a empty collection of something, a collection of messages
            //and produces a function that starts a pipeline, returning when it reaches the empty collection
            Func<
                ConcurrentBag<TOut>, 
                SideeffectHelper,
                Action<TIn>
            > createTransform) where TIn : class
            where TOut : class
        {
            ConcurrentBag<TOut> destinationBag = new ConcurrentBag<TOut>();

            Execute(indata, createTransform(destinationBag, helper), parallel);

            return destinationBag;
        }
        
        private static void Execute<TIn>(IEnumerable<TIn> indata, Action<TIn> action, bool parallel) where TIn : class
        {
            if (parallel)
            {
                Task.WaitAll(indata.Select(item => Task.Run(() =>
                {
                    try { action(item); }
                    catch (ExitStageException) { }
                })).ToArray());
            }
            else
            {
                foreach (TIn input in indata)
                {
                    try{ action(input);} catch (ExitStageException) { }
                }
            }
        }
    }

    internal class ExitCompilationException : Exception
    {
    }

    public class SideeffectHelper
    {
        public ConcurrentBag<CompilationMessage> CompilationMessages { get; } = new ConcurrentBag<CompilationMessage>();

        public Exception FailWith(CompilationMessage message)
        {
            CompilationMessages.Add(message);
            return new ExitStageException();
        }
    }

    public class ExitStageException : Exception
    {
    }
}
 