using libcompiler.SyntaxTree;

namespace libcompiler.Optimizations
{
    /// <summary>
    /// Used by RefParametersVisitor for checking potential side-effect for single parameter.
    /// Visit method returns true if there is one ore more potential side-effects
    /// on the parameter of name ParameterName in the visited tree.
    /// </summary>
    internal class CheckSideEffectsOfSingleParameterVisitor :SimpleSyntaxVisitor<bool>
    {
        public CheckSideEffectsOfSingleParameterVisitor(string parameterName)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }


        //Five potential side-effects exist:
        // - The parameter is used passed as reference in a method call. (fun(ref foo); )
        // - The parameter is the innermost variable
        //    of a chain of member-acceses that end in a method call. (foo.bar.fun(); )
        // - The parameter itself is the target of an assignment. (foo = value)
        // - The parameter is the innermost variable of
        //    a chain of memberacceses that are the target of an assignment. (foo.bar.baz = value; )
        // - An index of the parameter is the target of an assignment. (foo[i] = value; )

        protected override bool Combine(params bool[] parts)
        {
            bool result = false;
            foreach (bool part in parts)
            {
                result = result || part;
            }
            return result;
        }

        protected override bool VisitParameter(ParameterNode node)
        {
            if (base.VisitParameter(node)) //Save us the trouble if a side-effect has already been discovered.
                return true;

            return node.Reference && node.Identifier.Value == ParameterName;


        }


        protected override bool VisitCall(CallNode node)
        {
            if (base.VisitCall(node))
                return true;

            if (TargetIsOrBelongsToParameter(node.Target))
                return true;
            else
                return false;
        }

        protected override bool VisitAssignment(AssignmentNode node)
        {
            if (base.VisitAssignment(node))
                return true;

            if (TargetIsOrBelongsToParameter(node.Target))
                return true;

            var asIndex = node.Target as IndexNode;
            if (asIndex != null && TargetIsOrBelongsToParameter(asIndex.Target))
                return true;

            return false;
        }

        private bool TargetIsOrBelongsToParameter(ExpressionNode nodeTarget)
        {
            var asVar = nodeTarget as VariableNode;
            if (asVar != null)
            {
                return asVar.Name == ParameterName;
            }
            var asMemAcc = nodeTarget as MemberAccessNode;
            if (asMemAcc != null)
            {
                return TargetIsOrBelongsToParameter(asMemAcc.Target);
            }

            return false;
        }
    }
}