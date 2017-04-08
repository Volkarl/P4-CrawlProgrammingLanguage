namespace libcompiler.ExtensionMethods
{
    public static class StringExtensions
    {
        /// <summary>
        /// For the last char in a string(e.g. ']') find matching other character(e.g. '['). Matching as in assuming parenthesis behavior, so that the string <c>"asdf[as[fd[sa] asdf[]]fis]"</c> would return 4, the bracket matching the last one.
        /// </summary>
        /// <param name="other">The character used as left parenthesis/bracket/whichever. The right is implicitly the last char in the string.</param>
        /// <returns>Index of matching(parenthesis-style) character.</returns>
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