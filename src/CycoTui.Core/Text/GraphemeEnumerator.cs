using System;
using System.Collections.Generic;
using System.Globalization;

namespace CycoTui.Core.Text;

/// <summary>
/// Enumerates grapheme clusters in a string. Phase-1 implementation uses <see cref="StringInfo"/>.
/// TODO: Extend for full emoji ZWJ sequence handling and custom segmentation adjustments.
/// </summary>
public static class GraphemeEnumerator
{
    /// <summary>
    /// Enumerate graphemes in <paramref name="text"/>. Returns each text element substring.
    /// </summary>
    public static IEnumerable<string> Enumerate(string text)
    {
        if (string.IsNullOrEmpty(text)) yield break;
        var e = StringInfo.GetTextElementEnumerator(text);
        while (e.MoveNext())
        {
            yield return e.GetTextElement();
        }
    }
}
