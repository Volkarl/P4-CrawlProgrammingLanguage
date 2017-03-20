namespace libcompiler
{
    //TODO: Should probably be in a TypeSystem folder
    public class CrawlType
    {
        public string Textdef { get; }

        public CrawlType(string textdef)
        {
            Textdef = textdef;
        }
    }
}