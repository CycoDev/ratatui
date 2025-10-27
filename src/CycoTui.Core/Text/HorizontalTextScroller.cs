using System.Collections.Generic;
using CycoTui.Core.Text;

namespace CycoTui.Core.Text;

/// <summary>
/// Provides helpers for horizontally scrolling (windowing) grapheme sequences.
/// </summary>
public static class HorizontalTextScroller
{
    public static IEnumerable<string> EnumerateVisibleGraphemes(string text, int startColumn, int maxWidth)
    {
        if (string.IsNullOrEmpty(text) || maxWidth <= 0) yield break;
        int consumed = 0;
        int skippedWidth = 0;
        foreach (var g in GraphemeEnumerator.EnumerateWithZwj(text))
        {
            int w = WidthService.GetWidth(g);
            if (skippedWidth + w <= startColumn)
            {
                skippedWidth += w;
                continue;
            }
            // part of grapheme falls inside viewport only if entire grapheme after skip
            if (consumed + w > maxWidth) yield break;
            yield return g;
            consumed += w;
        }
    }
}
