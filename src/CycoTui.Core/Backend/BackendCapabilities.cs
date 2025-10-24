namespace CycoTui.Core.Backend;

/// <summary>
/// Immutable snapshot of backend feature capabilities. Used by higher layers to decide
/// whether to emit certain sequences (e.g., underline color) or fall back gracefully.
/// </summary>
public readonly struct BackendCapabilities
{
    public ColorLevel ColorLevel { get; }
    public bool SupportsUnderlineColor { get; }
    public bool SupportsMouse { get; }
    public bool SupportsScrollingRegions { get; }
    public bool SupportsTrueColor { get; }
    public bool SupportsUnicodeWidthReliably { get; }

    public BackendCapabilities(
        ColorLevel colorLevel,
        bool supportsUnderlineColor,
        bool supportsMouse,
        bool supportsScrollingRegions,
        bool supportsTrueColor,
        bool supportsUnicodeWidthReliably)
    {
        ColorLevel = colorLevel;
        SupportsUnderlineColor = supportsUnderlineColor;
        SupportsMouse = supportsMouse;
        SupportsScrollingRegions = supportsScrollingRegions;
        SupportsTrueColor = supportsTrueColor;
        SupportsUnicodeWidthReliably = supportsUnicodeWidthReliably;
    }

    public static BackendCapabilities Minimal => new(ColorLevel.None, false, false, false, false, false);
}
