using System.Collections.Generic;
using libcompiler.ExtensionMethods;
using libcompiler.Scope;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.CompilerStage
{
    internal class FirstScopePassVisitor : SyntaxRewriter
    {
        protected override CrawlSyntaxNode VisitBlock(BlockNode block)
        {
            string ns = block.FindNameSpace()?.Module ?? "";
            BlockScope scope = new BlockScope();
            List<CrawlSyntaxNode> children = new List<CrawlSyntaxNode>(); 

            foreach (CrawlSyntaxNode child in block)
            {
                CrawlSyntaxNode afterVisit = Visit(child);
                ClassTypeDeclerationNode asClass = afterVisit as ClassTypeDeclerationNode;
                if (asClass != null)
                {
                    CrawlConstructedType type = scope.AddClass(asClass, ns);
                    children.Add(asClass.WithClassType(type));
                }
                else
                {
                    children.Add(afterVisit);
                }
            }

            return block.Update(block.Interval, children, scope);
        }
    }
}