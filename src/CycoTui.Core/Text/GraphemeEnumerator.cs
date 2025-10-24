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

    /// <summary>
    /// Enumerate graphemes with rudimentary ZWJ (zero width joiner) chaining: emoji sequences separated by U+200D
    /// are merged into a single grapheme string.
    /// </summary>
    public static IEnumerable<string> EnumerateWithZwj(string text)
    {
        if (string.IsNullOrEmpty(text)) yield break;
        var e = StringInfo.GetTextElementEnumerator(text);
        string? pending = null;
        while (e.MoveNext())
        {
            var element = e.GetTextElement();
            if (pending == null)
            {
                pending = element;
                continue;
            }
            if (pending.Contains('\u200D') || element == "\u200D" || (element.StartsWith("\uD83C") && pending.Contains('\u200D')))
            {
                pending += element;
                continue;
            }
            if (element.Contains('\u200D'))
            {
                pending += element;
                continue;
            }
            yield return pending;
            pending = element;
        }
        if (pending != null) yield return pending;
    }
}
