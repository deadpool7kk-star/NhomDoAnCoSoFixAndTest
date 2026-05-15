using System;
using System.Globalization;
using System.Text;

namespace Book2.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes diacritics (accents) from a string.
        /// Example: "Tiếng Việt" -> "Tieng Viet"
        /// </summary>
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace('đ', 'd')
                .Replace('Đ', 'D');
        }

        /// <summary>
        /// Calculates the Levenshtein distance between two strings.
        /// Useful for fuzzy search.
        /// </summary>
        public static int CalculateLevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
            {
                if (string.IsNullOrEmpty(target)) return 0;
                return target.Length;
            }

            if (string.IsNullOrEmpty(target)) return source.Length;

            int n = source.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];

            // Initialize the distance matrix
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 1; j <= m; d[0, j] = j++) { }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }

            return d[n, m];
        }
    }
}
