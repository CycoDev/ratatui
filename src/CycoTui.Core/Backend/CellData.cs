using System;

namespace CycoTui.Core.Backend;

/// <summary>
/// Simplified placeholder for cell data used during backend scaffolding.
/// Will be replaced by full Cell type with style, width, grapheme data in Core phase.
/// </summary>
public readonly struct CellData
{
    public string Grapheme { get; }
    public ConsoleColor? Fg { get; }
    public ConsoleColor? Bg { get; }

    public CellData(string grapheme, ConsoleColor? fg = null, ConsoleColor? bg = null)
    {
        Grapheme = string.IsNullOrEmpty(grapheme) ? " " : grapheme;
        Fg = fg;
        Bg = bg;
    }

    public static CellData FromChar(char c, ConsoleColor? fg = null, ConsoleColor? bg = null) => new(c.ToString(), fg, bg);
}
