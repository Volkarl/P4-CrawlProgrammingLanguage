using libcompiler.ExtensionMethods;
using NUnit.Framework;

namespace libcompiler.Tests
{
    [TestFixture]
    public class StringExtensionTests
    {
        [Test]
        [TestCase("[]",'[', Result=0)]
        [TestCase(" []",'[', Result=1)]
        [TestCase("  [  ]",'[', Result=2)]
        [TestCase("[[]]",'[', Result=0)]
        [TestCase("asdas]",'[', Result=-1)]
        public int TestFromBackGetMatchingIndex(string s, char o) => s.FromBackGetMatchingIndex(o);

    }
}