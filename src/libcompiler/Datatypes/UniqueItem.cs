using System.Reflection;
using System.Reflection.Emit;

namespace libcompiler.Datatypes
{
    public class UniqueItem
    {
        private static int _id;

        public int Id { get;  } = System.Threading.Interlocked.Increment(ref _id);

        public override string ToString()
        {
            return Id.ToString();
        }

        public FieldBuilder FieldInfo { get; set; }
        public MethodBuilder MethodInfo { get; set; }
        public LocalBuilder VariableInfo { get; set; }

        //TODO: Add other things it can be
    }


}