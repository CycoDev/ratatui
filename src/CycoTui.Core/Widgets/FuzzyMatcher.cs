using System;
using System.Collections.Generic;
using System.Linq;

namespace CycoTui.Core.Widgets;

/// <summary>
/// Provides fuzzy matching capabilities for completion lists.
/// Scores matches based on character positions, consecutive matches, and word boundaries.
/// </summary>
public static class FuzzyMatcher
{
    /// <summary>
    /// Represents a fuzzy match result with score and matched string.
    /// </summary>
    public readonly record struct FuzzyMatch(string Text, int Score)
    {
        public static implicit operator string(FuzzyMatch match) => match.Text;
    }

    /// <summary>
    /// Filters and scores items based on fuzzy matching against the query.
    /// Returns items sorted by score (highest first).
    /// </summary>
    /// <param name="items">Items to search</param>
    /// <param name="query">Search query</param>
    /// <returns>Matched items sorted by relevance score</returns>
    public static IReadOnlyList<string> Filter(IReadOnlyList<string> items, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return items;

        var lowerQuery = query.ToLowerInvariant();

        var matches = new List<FuzzyMatch>();
        foreach (var item in items)
        {
            int score = CalculateScore(item.ToLowerInvariant(), lowerQuery);
            if (score > 0)
            {
                matches.Add(new FuzzyMatch(item, score));
            }
        }

        // Sort by score descending, then by length ascending (prefer shorter matches)
        return matches
            .OrderByDescending(m => m.Score)
            .ThenBy(m => m.Text.Length)
            .Select(m => m.Text)
            .ToList();
    }

    /// <summary>
    /// Calculates fuzzy match score. Returns 0 if no match.
    /// Higher scores indicate better matches.
    /// </summary>
    private static int CalculateScore(string text, string query)
    {
        if (query.Length == 0) return 0;
        if (text.Length == 0) return 0;

        int score = 0;
        int queryIndex = 0;
        int consecutiveMatches = 0;
        bool lastWasSeparator = true; // Start of string counts as word boundary

        for (int textIndex = 0; textIndex < text.Length && queryIndex < query.Length; textIndex++)
        {
            char textChar = text[textIndex];
            char queryChar = query[queryIndex];

            bool isSeparator = textChar == '/' || textChar == '\\' || textChar == '_' ||
                               textChar == '-' || textChar == '.' || textChar == ' ';

            if (textChar == queryChar)
            {
                // Base score for any match
                score += 100;

                // Bonus for consecutive matches (compound bonus)
                consecutiveMatches++;
                score += consecutiveMatches * 50;

                // Large bonus for matching at word boundary
                if (lastWasSeparator)
                {
                    score += 200;
                }

                // Bonus for early matches (closer to start of string)
                score += Math.Max(0, 100 - textIndex);

                queryIndex++;
            }
            else
            {
                // Reset consecutive counter when match breaks
                consecutiveMatches = 0;
            }

            lastWasSeparator = isSeparator;
        }

        // Return 0 if we didn't match all query characters
        if (queryIndex < query.Length)
            return 0;

        return score;
    }

    /// <summary>
    /// Tests if text matches query (without scoring).
    /// </summary>
    public static bool IsMatch(string text, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return true;

        var lowerText = text.ToLowerInvariant();
        var lowerQuery = query.ToLowerInvariant();

        int queryIndex = 0;
        for (int i = 0; i < lowerText.Length && queryIndex < lowerQuery.Length; i++)
        {
            if (lowerText[i] == lowerQuery[queryIndex])
            {
                queryIndex++;
            }
        }

        return queryIndex == lowerQuery.Length;
    }
}
