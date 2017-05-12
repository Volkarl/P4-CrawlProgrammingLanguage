using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.CodeGen.IL
{
    class ILGeneratorVisitor : SyntaxVisitor
    {
        private readonly ILGenerator _ilGenerator;
        private readonly MethodBuilder _builder;

        public ILGeneratorVisitor(MethodBuilder builder)
        {
            _builder = builder;
            _ilGenerator = builder.GetILGenerator();
        }

        protected override void VisitReturnStatement(ReturnStatementNode node)
        {
            base.VisitReturnStatement(node);
            _ilGenerator.Emit(OpCodes.Ret);
        }

        protected override void VisitCall(CallNode node)
        {
            //Exacty what to do in a call depends on what we are calling.
            //If we are calling a known method, just call it
            //Otherwiser we have to load the callee and then call it.

            //Make sure all parameters are present on the stack
            foreach (ArgumentNode argument in node.Arguments)
            {
                Visit(argument.Value);
            }

            MemberAccessNode member = node.Target as MemberAccessNode;
            CrawlMethodType callee;
            if (member != null)
            {
                if(node.CalleeType  == null) throw new NotImplementedException();

                _ilGenerator.EmitCall(OpCodes.Call, node.CalleeType .MethodInfo, Type.EmptyTypes);

            }
            else throw new NotImplementedException();

            if (node.Parent.Type == NodeType.Block && !node.ResultType.Equals(CrawlSimpleType.Intet)) //AFAIK, best way to see if value is thrown away.
            {
                _ilGenerator.Emit(OpCodes.Pop);
            }
        }

        protected override void VisitIntegerLiteral(IntegerLiteralNode node)
        {
            _ilGenerator.Emit(OpCodes.Ldc_I4, node.Value);
        }

        protected override void VisitRealLiteral(RealLiteralNode node)
        {
            _ilGenerator.Emit(OpCodes.Ldc_R8, node.Value);
        }

        protected override void VisitStringLiteral(StringLiteralNode node)
        {
            _ilGenerator.Emit(OpCodes.Ldstr, node.Value);
        }

        protected override void VisitBooleanLiteral(BooleanLiteralNode node)
        {
            if(node.Value)
                _ilGenerator.Emit(OpCodes.Ldc_I4_1);
            else 
                _ilGenerator.Emit(OpCodes.Ldc_I4_0);


        }

        public void EmitFor(BlockNode methodBody)
        {
            _ilGenerator.Emit(OpCodes.Nop);
            Visit(methodBody);

            //TODO: not do this unless proved not (follow multipath and check if called all directions
            _ilGenerator.Emit(OpCodes.Ret);
        }
    }
}
