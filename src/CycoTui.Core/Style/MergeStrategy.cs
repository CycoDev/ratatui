namespace CycoTui.Core.Style;

/// <summary>
/// Strategy for resolving symbol overlap when drawing layered borders or glyphs.
/// </summary>
/// <remarks>
/// Replace overwrites existing glyph; Preserve keeps existing non-space glyph. Combine reserved for future.
/// </remarks>
public enum MergeStrategy
{
    Replace,
    Preserve
    // Combine reserved for future implementation
}
