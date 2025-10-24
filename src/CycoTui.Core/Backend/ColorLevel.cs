namespace CycoTui.Core.Backend;

/// <summary>
/// Enumerates color depth support levels detected for a backend.
/// </summary>
public enum ColorLevel
{
    /// <summary>No color support detected.</summary>
    None = 0,
    /// <summary>Standard ANSI 16 colors (basic + bright).</summary>
    Ansi16 = 1,
    /// <summary>ANSI 256 color palette (indexed).</summary>
    Ansi256 = 2,
    /// <summary>24-bit true color (RGB) support.</summary>
    TrueColor = 3
}
