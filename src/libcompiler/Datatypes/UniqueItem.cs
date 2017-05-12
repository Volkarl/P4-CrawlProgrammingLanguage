using System.Reflection;

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

        public FieldInfo FieldInfo { get; set; }
        //TODO: Add other things it can be
    }


}