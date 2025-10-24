using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace CycoTui.Core.Text;

/// <summary>
/// Computes display width (columns) for grapheme clusters. Phase-1 uses simplified tables.
/// TODO: Expand with full Unicode East Asian Width data + emoji sequences.
/// </summary>
internal sealed class UnicodeWidthProvider
{
    private readonly WidthMode _mode;
    // Simplified ranges (inclusive) for wide characters (CJK Unified Ideographs + basic emoji range placeholder).
    private static readonly (int Start, int End)[] WideRanges =
    {
        (0x1100, 0x115F), // Hangul Jamo init
        (0x2E80, 0x2FFF), // CJK Radicals Supplement + Kangxi Radicals
        (0x3000, 0x303F), // CJK Symbols and Punctuation
        (0x3040, 0x9FFF), // Hiragana + Katakana + CJK Unified Ideographs
        (0x1F300, 0x1F5FF), // Misc Symbols & Pictographs
        (0x1F600, 0x1F64F), // Emoticons
        (0x1F680, 0x1F6FF), // Transport & Map
        (0x1F900, 0x1F9FF), // Supplemental Symbols & Pictographs
    };

    // Ambiguous characters subset placeholder (common examples)
    private static readonly int[] AmbiguousCodePoints =
    {
        0x00A1, 0x00A4, 0x00A7, 0x00A8, 0x00AA, 0x00AD, 0x00AE, 0x00B0, 0x00B1, 0x00B2,
        0x00B3, 0x00B4, 0x00B5, 0x00B6, 0x00B7, 0x00B8, 0x00B9, 0x00BA, 0x00BC, 0x00BD,
        0x00BE, 0x00BF, 0x00C6, 0x00D0, 0x00D7, 0x00D8, 0x00DE, 0x00DF, 0x00E6, 0x00E7
    };
    private static readonly HashSet<int> AmbiguousSet = new(AmbiguousCodePoints);

    public UnicodeWidthProvider(WidthMode mode)
    {
        _mode = mode;
    }

    /// <summary>
    /// Get width for a grapheme cluster. If cluster length &gt; 1, treat width as max of constituent scalars
    /// (placeholder). Combining marks yield width 0.
    /// </summary>
    public int GetWidth(string grapheme)
    {
        if (string.IsNullOrEmpty(grapheme)) return 0;
        // Use first scalar for classification; refine later for emoji sequences.
        var e = StringInfo.GetTextElementEnumerator(grapheme);
        e.MoveNext();
        var element = e.GetTextElement();
        var runeEnum = element.EnumerateRunes();
        int width = 0;
        foreach (var rune in runeEnum)
        {
            width = Math.Max(width, ClassifyRuneWidth(rune));
        }
        return width;
    }

    private int ClassifyRuneWidth(Rune rune)
    {
        if (IsCombiningMark(rune)) return 0;
        if (IsWide(rune)) return 2;
        if (IsAmbiguous(rune)) return _mode == WidthMode.EastAsian ? 2 : 1;
        // Basic emoji fallback width assumption = 2 when surrogate pair outside BMP.
        if (rune.Value > 0xFFFF && char.IsSurrogatePair(rune.ToString(), 0)) return 2; // placeholder logic
        return 1;
    }

    private static bool IsWide(Rune rune)
    {
        int v = rune.Value;
        for (int i = 0; i < WideRanges.Length; i++)
        {
            var (s, e) = WideRanges[i];
            if (v >= s && v <= e) return true;
        }
        return false;
    }

    private static bool IsAmbiguous(Rune rune) => AmbiguousSet.Contains(rune.Value);

    private static bool IsCombiningMark(Rune rune)
    {
        var cat = Rune.GetUnicodeCategory(rune);
        return cat is UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark or UnicodeCategory.EnclosingMark;
    }
}
