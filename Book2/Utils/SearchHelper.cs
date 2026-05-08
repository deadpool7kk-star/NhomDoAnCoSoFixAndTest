using Book2.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Book2.Utils
{
    public static class SearchHelper
    {
        public static IEnumerable<Sach> SmartSearch(IEnumerable<Sach> books, string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return books;
            }

            // 1. Prepare Queries
            string originalQuery = query.Trim().ToLowerInvariant();
            string normalizedQuery = originalQuery.RemoveDiacritics();
            
            var stopwords = new[] { "sach", "cuon", "quyen", "tap", "bo", "truyen" };
            
            // Tokens without diacritics
            string[] queryTokens = normalizedQuery
                .Split(new[] { ' ', '\t', '\n', '\r', ',', '.', '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            // Meaningful tokens (remove stopwords)
            string[] cleanTokens = queryTokens.Where(t => !stopwords.Contains(t)).ToArray();
            if (cleanTokens.Length == 0) cleanTokens = queryTokens; // fallback if query was only stopwords

            string cleanQuery = string.Join(" ", cleanTokens);
            string originalCleanQuery = string.Join(" ", originalQuery
                .Split(new[] { ' ', '\t', '\n', '\r', ',', '.', '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(t => !stopwords.Contains(t.RemoveDiacritics())));

            var scoredBooks = books.Select(book =>
            {
                int score = 0;
                int matchedTokensCount = 0;

                // Prepare book fields for searching
                string titleOriginal = (book.TenSach ?? "").ToLowerInvariant();
                string authorOriginal = (book.TacGia ?? "").ToLowerInvariant();
                string categoryOriginal = (book.TheLoai?.TenTheLoai ?? "").ToLowerInvariant();

                string title = titleOriginal.RemoveDiacritics();
                string author = authorOriginal.RemoveDiacritics();
                string category = categoryOriginal.RemoveDiacritics();

                var titleTokens = title.Split(new[] { ' ', ',', '.', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
                var authorTokens = author.Split(new[] { ' ', ',', '.', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);

                // --- A. PHRASE MATCHING (Very High Priority) ---
                
                // 1. Diacritic-Sensitive Phrase Match
                if (titleOriginal == originalCleanQuery) score += 200;
                else if (titleOriginal.Contains(originalCleanQuery)) score += 100;

                if (authorOriginal == originalCleanQuery) score += 150;
                else if (authorOriginal.Contains(originalCleanQuery)) score += 80;

                if (categoryOriginal == originalCleanQuery) score += 120;
                else if (categoryOriginal.Contains(originalCleanQuery)) score += 60;

                // 2. Diacritic-Insensitive Phrase Match
                if (title == cleanQuery) score += 100;
                else if (title.Contains(cleanQuery)) score += 50;

                if (author == cleanQuery) score += 80;
                else if (author.Contains(cleanQuery)) score += 40;

                if (category == cleanQuery) score += 60;
                else if (category.Contains(cleanQuery)) score += 30;

                // --- B. TOKEN MATCHING ---
                foreach (var token in cleanTokens)
                {
                    bool tokenMatched = false;

                    // Token match in Title
                    if (titleTokens.Contains(token))
                    {
                        score += 15;
                        tokenMatched = true;
                    }
                    else if (token.Length >= 3 && titleTokens.Any(t => t.StartsWith(token))) // Partial match
                    {
                        score += 5;
                        tokenMatched = true;
                    }

                    // Token match in Author
                    if (!tokenMatched)
                    {
                        if (authorTokens.Contains(token))
                        {
                            score += 12;
                            tokenMatched = true;
                        }
                        else if (token.Length >= 3 && authorTokens.Any(t => t.StartsWith(token)))
                        {
                            score += 4;
                            tokenMatched = true;
                        }
                    }

                    // Token match in Category
                    if (!tokenMatched && categoryTokensContains(category, token))
                    {
                        score += 10;
                        tokenMatched = true;
                    }

                    // Fuzzy Search
                    if (!tokenMatched && token.Length > 3)
                    {
                        foreach (var tToken in titleTokens)
                        {
                            if (tToken.Length > 3)
                            {
                                int distance = StringExtensions.CalculateLevenshteinDistance(token, tToken);
                                if (distance <= 1 || (token.Length > 5 && distance <= 2))
                                {
                                    score += 5;
                                    tokenMatched = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (tokenMatched)
                    {
                        matchedTokensCount++;
                    }
                }

                // --- C. BONUS SCORING ---
                if (cleanTokens.Length > 1)
                {
                    float matchRatio = (float)matchedTokensCount / cleanTokens.Length;
                    if (matchRatio >= 1.0f) score += 30; // matched all tokens
                    else if (matchRatio >= 0.5f) score += 10; // matched at least half
                }

                return new { Book = book, Score = score, MatchedTokens = matchedTokensCount };
            });

            // If a query has multiple clean tokens, and a book only matches 1 token (and no phrase match), 
            // its score will typically be around 15. If it matches 2 tokens, score is 30 + 10 (ratio) = 40.
            // A threshold of 20 ensures we filter out books that only coincidentally matched a single short syllable.
            return scoredBooks
                .Where(x => x.Score >= 20)
                .OrderByDescending(x => x.Score)
                .Select(x => x.Book)
                .ToList();
        }

        private static bool categoryTokensContains(string category, string token)
        {
            var tokens = category.Split(new[] { ' ', ',', '.', '-', '_' }, StringSplitOptions.RemoveEmptyEntries);
            return tokens.Contains(token);
        }
    }
}
