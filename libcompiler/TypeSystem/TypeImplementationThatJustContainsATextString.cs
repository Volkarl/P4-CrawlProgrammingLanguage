namespace libcompiler.TypeSystem
{
    /// <summary>
    /// Temporary class to contain current results from type calculations.
    /// </summary>
    public class TypeImplementationThatJustContainsATextString : CrawlType
    {
        public TypeImplementationThatJustContainsATextString(string theFamousTextString)
        {
            TheFamousTextString = theFamousTextString;
        }

        public string TheFamousTextString { get; }


    }
}