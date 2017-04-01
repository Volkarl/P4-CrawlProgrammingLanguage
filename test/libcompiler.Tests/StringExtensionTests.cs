using libcompiler.ExtensionMethods;
using NUnit.Framework;

namespace libcompiler.Tests
{
    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
        [TestCase("[]",'[', ExpectedResult=0)]
        [TestCase(" []",'[', ExpectedResult=1)]
        [TestCase("  [  ]",'[', ExpectedResult=2)]
        [TestCase("[[]]",'[', ExpectedResult=0)]
        [TestCase("asdas]",'[', ExpectedResult=-1)]
        public int TestFromBackGetMatchingIndex(string s, char o) => s.FromBackGetMatchingIndex(o);

    }
}