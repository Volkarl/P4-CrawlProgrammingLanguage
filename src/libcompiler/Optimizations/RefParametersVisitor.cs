using System.Collections.Generic;
using libcompiler.SyntaxTree;

namespace libcompiler.Optimizations
{
    public class RefParametersVisitor : SyntaxRewriter
    {
        protected override CrawlSyntaxNode VisitMethodDecleration(MethodDeclerationNode methodDecleration)
        {
            var decl = (MethodDeclerationNode) base.VisitMethodDecleration(methodDecleration);

            List<ParameterNode> newParams = new List<ParameterNode>();
            foreach (ParameterNode param in decl.Parameters)
            {
                if (param.Reference) //If it's a reference, don't mess with it.
                    newParams.Add(param);
                else
                {

                    //If there is any risk of side-effects affecting non-ref parameter, don't optimize to ref.
                    var visitor = new CheckSideEffectsOfSingleParameterVisitor(param.Identifier.Value);
                    if (visitor.Visit(methodDecleration.Body))
                        newParams.Add(param);
                    else
                        newParams.Add(CrawlSyntaxNode.Parameter(param.Interval, param.Reference,
                            false, param.ParameterType, param.Identifier));
                }
            }

            return CrawlSyntaxNode.MethodDecleration(decl.Interval, decl.ProtectionLevel, decl.Scope, decl.MethodSignature,
                newParams, decl.Body, decl.Identifier, decl.GenericParameters);
        }
    }
}