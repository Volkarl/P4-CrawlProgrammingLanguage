namespace libcompiler.ExtensionMethods
{
    public static class StringExtensions
    {
        public static int FromBackGetMatchingIndex(this string s, char other)
        {
            int count = 1;
            char open = s[s.Length - 1];
            for (int i = s.Length - 2; i >= 0; i--)
            {
                if (s[i] == open) count++;
                if (s[i] == other) count--;

                if (count == 0) return i;
            }

            return -1;
        }
    }
}