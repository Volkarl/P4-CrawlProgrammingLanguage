namespace libcompiler.TypeSystem
{
    public class CrawlType
    {
        public string Textdef { get; }

        public CrawlType(string textdef)
        {
            Textdef = textdef;
        }

        public override string ToString()
        {
            return Textdef;
        }
    }
}