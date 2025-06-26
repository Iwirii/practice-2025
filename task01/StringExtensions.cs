namespace task01
{
    public static class StringExtensions
    {
        public static bool IsPalindrome(this string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            string lower = input.ToLower();

            string filtered_text = "";
            foreach (char c in lower)
            {
                if (!char.IsWhiteSpace(c) && !char.IsPunctuation(c))
                {
                    filtered_text += c;
                }
            }

            if (string.IsNullOrEmpty(filtered_text)) return false;

            string reversed = new string(filtered_text.Reverse().ToArray());

            return reversed == filtered_text;
        }
    }
}
