using System.Collections.Concurrent;
using Antlr4.Runtime;
using libcompiler.Scope;
using libcompiler.SyntaxTree;

namespace libcompiler.CompilerStage.SemanticAnalysis
{
    internal class PutTypeVisitor : SyntaxRewriter
    {
        private readonly ConcurrentBag<CompilationMessage> _messages;
        private readonly ITokenStream _tokens;
        private readonly string _file;

        public PutTypeVisitor(ConcurrentBag<CompilationMessage> messages, ITokenStream tokens, string file)
        {
            _messages = messages;
            _tokens = tokens;
            _file = file;
        }

        protected override CrawlSyntaxNode VisitType(TypeNode type)
        {
            IScope scope = type.FindFirstScope();
            try
            {
                CrawlType actualType = CrawlType.ParseDecleration(scope, type.TypeName);

                var v = type.WithActualType(actualType);
                return v;
            }
            catch (TypeNotFoundException tnfe)
            {
                _messages.Add(CompilationMessage.Create(_tokens, type.Interval, MessageCode.TypeNotFound, _file));
                return type;
            }
        }
    }
}