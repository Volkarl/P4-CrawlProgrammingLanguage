using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libcompiler.CompilerStage
{
    class Rewriters
    {
        public static AstData ReduceConstructs(AstData arg1, SideeffectHelper arg2)
        {
            return new AstData(
                arg1.TokenStream,
                arg1.Filename,
                new SyntaxModificationsRewriter().Visit(arg1.Tree.RootNode).OwningTree
            );
        }
    }
}
