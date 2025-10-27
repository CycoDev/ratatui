using System;
using CycoTui.Core.Style;

namespace CycoTui.Core.Buffer;

/// <summary>
/// <summary>
/// A single cell within a Buffer. Stores a grapheme cluster, cached column width, style, and Skip flag indicating continuation of a multi-width grapheme.
/// </summary>
/// </summary>
public readonly struct Cell : IEquatable<Cell>
{
    /// <summary>Grapheme cluster stored for this cell (primary or continuation).</summary>
    public string Grapheme { get; }
    public StyleType Style { get; }

    /// <summary>
    /// Construct a cell. Control characters are replaced by a single space; null/empty becomes space.
    /// </summary>
    /// <summary>Display width in columns (1 or 2 typically).</summary>
    public byte Width { get; }
    /// <summary>If true, this cell is a continuation part of a multi-width grapheme and should be skipped during direct rendering.</summary>
    public bool Skip { get; }

    public Cell(string symbol, StyleType style, byte width = 1, bool skip = false)
    {
        if (string.IsNullOrEmpty(symbol))
            Grapheme = " ";
        else if (symbol.Length == 1 && char.IsControl(symbol[0]))
            Grapheme = " ";
        else
            Grapheme = symbol;
        Style = style;
        Width = width == 0 ? (byte)1 : width;
        Skip = skip;
    }

    public static Cell Empty => new(" ", StyleType.Empty, 1, false);

    /// <summary>
    /// Return a new cell with updated symbol (validation rules apply).
    /// </summary>
    public Cell WithSymbol(string symbol) => new(symbol, Style, Width, Skip);

    /// <summary>
    /// Return a new cell with updated style.
    /// </summary>
    public Cell WithStyle(StyleType style) => new(Grapheme, style, Width, Skip);

    public Cell AsContinuation() => new(Grapheme, Style, Width, true);

    public bool Equals(Cell other) => Grapheme == other.Grapheme && Style == other.Style && Width == other.Width && Skip == other.Skip;
    public override bool Equals(object? obj) => obj is Cell c && Equals(c);
    public override int GetHashCode() => HashCode.Combine(Grapheme, Style);
    public static bool operator ==(Cell left, Cell right) => left.Equals(right);
    public static bool operator !=(Cell left, Cell right) => !left.Equals(right);
}
