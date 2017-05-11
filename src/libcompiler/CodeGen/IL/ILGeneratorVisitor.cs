using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;

namespace libcompiler.CodeGen.IL
{
    class ILGeneratorVisitor : SyntaxVisitor
    {
        private readonly ILGenerator _ilGenerator;

        public ILGeneratorVisitor(ILGenerator ilGenerator)
        {
            _ilGenerator = ilGenerator;
        }

        protected override void VisitReturnStatement(ReturnStatementNode node)
        {
            base.VisitReturnStatement(node);
            _ilGenerator.Emit(OpCodes.Ret);
        }

        protected override void VisitIntegerLiteral(IntegerLiteralNode node)
        {
            _ilGenerator.Emit(OpCodes.Ldc_I4, node.Value);
        }
    }
}
