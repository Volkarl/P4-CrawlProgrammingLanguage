namespace libcompiler.Datatypes
{
    public class Reference<T>
    {
        public T Item { get; set; }

        public override string ToString()
        {
            if (Item == null) return "empty";

            return Item.ToString();
        }
    }
}