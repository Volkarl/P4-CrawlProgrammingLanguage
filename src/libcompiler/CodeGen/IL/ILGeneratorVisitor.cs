using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using libcompiler.Datatypes;
using libcompiler.SyntaxTree;
using libcompiler.TypeSystem;

namespace libcompiler.CodeGen.IL
{
    class ILGeneratorVisitor : SyntaxVisitor
    {
        private readonly ILGenerator _ilGenerator;
        private readonly MethodBuilder _builder;
        private bool assigning = false;


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
            VariableNode variable = node.Target as VariableNode;
            CrawlMethodType callee;
            if (member != null )
            {
                if(node.CalleeType  == null) throw new NotImplementedException();

                _ilGenerator.EmitCall(OpCodes.Call, node.CalleeType.MethodInfo, Type.EmptyTypes);

            }
            else if (variable != null)
            {
                if(variable.UniqueItemTracker.Item.MethodInfo == null) throw new NotImplementedException();

                _ilGenerator.EmitCall(OpCodes.Call, variable.UniqueItemTracker.Item.MethodInfo, Type.EmptyTypes);
            }
            else throw new NotImplementedException();

            if (node.Parent.Type == NodeType.Block && !node.ResultType.Equals(CrawlSimpleType.Intet)) //AFAIK, best way to see if value is thrown away.
            {
                _ilGenerator.Emit(OpCodes.Pop);
            }
        }

        protected override void VisitVariable(VariableNode node)
        {
            UniqueItem item = node.UniqueItemTracker.Item;
            if (item.FieldInfo != null)
            {
                if (item.FieldInfo.IsStatic)
                {
                    _ilGenerator.Emit(OpCodes.Ldsfld, item.FieldInfo);
                }
                else
                {
                    //TODO put this on stack
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            //throw new NotImplementedException();
        }

        protected override void VisitSingleVariableDecleration(SingleVariableDeclerationNode node)
        {
            VariableDeclerationNode parrent = (VariableDeclerationNode) node.Parent.Parent;
            LocalBuilder variable = _ilGenerator.DeclareLocal(parrent.DeclerationType.ActualType.ClrType);
            node.Identifier.UniqueItemTracker.Item.VariableInfo = variable;

            if (node.DefaultValue != null)
            {
                Visit(node.DefaultValue);

                _ilGenerator.Emit(OpCodes.Stloc, variable);
            }
        }

        protected override void VisitAssignment(AssignmentNode node)
        {
            FieldInfo info;
            OpCode opcode;

            if (node.Target is VariableNode)
            {
                VariableNode target = (VariableNode) node.Target;
                info = target.UniqueItemTracker.Item.FieldInfo;
                if (target.UniqueItemTracker.Item.FieldInfo.IsStatic)
                {
                    opcode = OpCodes.Stsfld;
                }
                else
                {
                    //TODO put this on stack
                    opcode = OpCodes.Stfld;
                }
            }
            else throw new NotImplementedException();

            Visit(node.Value);

            _ilGenerator.Emit(opcode, info);

            //base.VisitAssignment(node);
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
