using System.Collections.Concurrent;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using libcompiler.Scope;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage.SemanticAnalysis
{
    internal class CheckDeclerationOrderVisitor : SyntaxVisitor
    {
        private readonly ConcurrentBag<CompilationMessage> _messages;
        private readonly AstData _data;

        public CheckDeclerationOrderVisitor(ConcurrentBag<CompilationMessage> messages, AstData data)
        {
            _messages = messages;
            _data = data;
        }

        public override void Visit(CrawlSyntaxNode node)
        {
            if (node is IScope)
            {
                CheckHides(node);
            }

            base.Visit(node);
        }

        protected override void VisitVariable(VariableNode nodes)
        {
            IScope scope = nodes.FindFirstScope();
            TypeInformation[] decl = scope.FindSymbol(nodes.Name);
            if (decl == null || decl.Length == 0)
            {
                //TODO: Edit lenght on everything...
                _messages.Add(CompilationMessage.Create(_data.TokenStream, nodes.Interval, MessageCode.NoSuchSymbol,
                    _data.Filename, null));
            }
            else if (decl.Length == 1)
            {
                //TODO: If TypeInformation needs a ScopeType that contains MethodLine and ClassLike. Supress for classLike
                if (decl[0].DeclarationLocation > nodes.Interval.a &&
                    decl[0].DeclaringScope == DeclaringScope.MethodLike)
                {
                    //TODO: Make CompilationMessage.Create take an IntervalSet of intresting locations instead of one location...
                    _messages.Add(CompilationMessage.Create(_data.TokenStream, nodes.Interval,
                        MessageCode.UseBeforeDecleration, _data.Filename, null));
                }
            }
            else //if(decl.Length > 1)
            {
                _messages.Add(CompilationMessage.Create(_data.TokenStream, nodes.Interval,
                    MessageCode.InternalCompilerError, _data.Filename, "Not allowed due technical (lazy) reasons",
                    MessageSeverity.Fatal));
            }




            base.VisitVariable(nodes);
        }

        void CheckHides(CrawlSyntaxNode aScope)
        {
            //TODO: This should be optimized by adding and removing from mapping with a stack
            //Would go from O(n^2) -> O(1)
            //This checks if something gets redefined.
            Dictionary<string, CrawlSyntaxNode> mapping = new Dictionary<string, CrawlSyntaxNode>();
            while (aScope != null)
            {
                IScope scope = aScope as IScope;
                if (scope != null)
                {
                    var local = scope.LocalSymbols();

                    //TODO: REMOVE. Just to stop it crashing
                    if (local == null)
                        local = new List<string>();

                    foreach (string symbol in local)
                    {
                        if (mapping.ContainsKey(symbol))
                        {
                            Interval first = mapping[symbol].Interval;
                            _messages.Add(CompilationMessage.Create(_data.TokenStream, first,
                                MessageCode.HidesOtherSymbol, _data.Filename,
                                $"Unable to declare {symbol} as it hides {symbol} from {aScope}"));
                        }
                        else
                        {
                            mapping.Add(symbol, aScope);
                        }
                    }
                }

                aScope = aScope.Parent;
            }
        }
    }
}