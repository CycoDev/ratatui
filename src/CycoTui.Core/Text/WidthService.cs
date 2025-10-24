namespace CycoTui.Core.Text;

/// <summary>
/// Global access point for width calculations. Mode can be set prior to rendering loop.
/// Changing mode mid-frame is undefined (documented behavior).
/// </summary>
public static class WidthService
{
    private static UnicodeWidthProvider _provider = new(WidthMode.Standard);

    public static WidthMode Mode { get; private set; } = WidthMode.Standard;

    public static void SetMode(WidthMode mode)
    {
        Mode = mode;
        _provider = new UnicodeWidthProvider(mode);
    }

    /// <summary>Get width of a grapheme using current <see cref="Mode"/>.</summary>
    public static int GetWidth(string grapheme) => _provider.GetWidth(grapheme);
}
