using System;

namespace CycoTui.Core.Backend;

/// <summary>
/// Simplified placeholder for cell data used during backend scaffolding.
/// Will be replaced by full Cell type with style, width, grapheme data in Core phase.
/// </summary>
public readonly struct CellData
{
    public char Symbol { get; }
    public ConsoleColor? Fg { get; }
    public ConsoleColor? Bg { get; }

    public CellData(char symbol, ConsoleColor? fg = null, ConsoleColor? bg = null)
    {
        Symbol = symbol;
        Fg = fg;
        Bg = bg;
    }
}
