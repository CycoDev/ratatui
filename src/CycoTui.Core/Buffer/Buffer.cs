using System;
using CycoTui.Core.Backend;
using CycoTui.Core.Style;

namespace CycoTui.Core.Buffer;

/// <summary>
/// Represents a 2D terminal drawing surface for a frame. Stores Cells in a flat array indexed by X/Y.
/// Supports grapheme-aware insertion (SetString) including multi-width (emoji/CJK) handling via skip flags.
/// </summary>
/// <remarks>
/// Buffer is reused across frames; Clear() resets all cells to Empty without reallocating.
/// Skip cells mark continuation parts of wide graphemes to avoid redundant emission on diff.
/// </remarks>
public sealed class Buffer
{
    private readonly Cell[] _cells;
    public Position Origin { get; }
    public Size Size { get; }

    /// <summary>
    /// Create a buffer with specified origin and size. All cells initialized to <see cref="Cell.Empty"/>.
    /// </summary>
    /// <param name="origin">Top-left coordinate of buffer area.</param>
    /// <param name="size">Dimensions (width x height). Negative values coerced to zero.</param>
    public Buffer(Position origin, Size size)
    {
        Origin = origin;
        Size = size.Width < 0 || size.Height < 0 ? Size.Empty : size;
        _cells = new Cell[Size.Width * Size.Height];
        for (int i = 0; i < _cells.Length; i++) _cells[i] = Cell.Empty;
    }

    /// <summary>
    /// Create an empty buffer at origin (0,0).
    /// </summary>
    public static Buffer Empty(Size size) => new(new Position(0, 0), size);

    /// <summary>Clear all cells to <see cref="Cell.Empty"/> without reallocating.</summary>
    public void Clear()
    {
        for (int i = 0; i < _cells.Length; i++) _cells[i] = Cell.Empty;
    }

    /// <summary>
    /// Returns a 0-based flattened index for coordinates. Throws with descriptive details if out of bounds.
    /// </summary>

    private int IndexOf(int x, int y)
    {
        if (x < Origin.X || y < Origin.Y || x >= Origin.X + Size.Width || y >= Origin.Y + Size.Height)
            throw new ArgumentOutOfRangeException(
                $"Coordinates ({x},{y}) outside buffer bounds Origin=({Origin.X},{Origin.Y}) Size=({Size.Width}x{Size.Height})");
        return (y - Origin.Y) * Size.Width + (x - Origin.X);
    }

    /// <summary>
    /// Retrieve cell at absolute coordinates. Throws if outside bounds.
    /// </summary>
    /// <param name="x">Column (0-based).</param>
    /// <param name="y">Row (0-based).</param>
    public Cell GetCell(int x, int y) => _cells[IndexOf(x, y)];

    /// <summary>
    /// Attempt to get a cell without throwing, returning false if out of bounds.
    /// </summary>
    /// <param name="x">Column (0-based).</param>
    /// <param name="y">Row (0-based).</param>
    /// <param name="cell">Result cell when true, or <see cref="Cell.Empty"/> when false.</param>
    public bool TryGetCell(int x, int y, out Cell cell)
    {
        if (x < Origin.X || y < Origin.Y || x >= Origin.X + Size.Width || y >= Origin.Y + Size.Height)
        {
            cell = Cell.Empty; return false;
        }
        cell = _cells[(y - Origin.Y) * Size.Width + (x - Origin.X)];
        return true;
    }

    /// <summary>
    /// Set a cell value at coordinates, overwriting previous content.
    /// </summary>
    /// <param name="x">Column (0-based).</param>
    /// <param name="y">Row (0-based).</param>
    /// <param name="cell">Cell to store.</param>
    public void SetCell(int x, int y, Cell cell)
    {
        _cells[IndexOf(x, y)] = cell;
    }


    /// <summary>
    /// Write a string starting at (x,y) truncating at buffer width.
    /// Grapheme-aware: multi-width graphemes consume multiple cells with continuation cells flagged Skip.
    /// </summary>
    /// <param name="x">Column (0-based).</param>
    /// <param name="y">Row (0-based).</param>
    /// <param name="text">Text to write; null ignored.</param>
    /// <param name="style">Style applied to each grapheme.</param>
    public void SetString(int x, int y, string text, StyleType style)
    {
        if (text == null) return;
        int cx = x;
        foreach (var grapheme in CycoTui.Core.Text.GraphemeEnumerator.Enumerate(text))
        {
            var w = CycoTui.Core.Text.WidthService.GetWidth(grapheme);
            if (cx >= Origin.X + Size.Width) break;
            // Invalidate trailing continuation cells if previous head at this position was wider than new grapheme width.
            if (TryGetCell(cx, y, out var existingHead) && existingHead.Width > 1 && existingHead.Width > w)
            {
                for (int k = 1; k < existingHead.Width; k++)
                {
                    int nx = cx + k;
                    if (nx >= Origin.X + Size.Width) break;
                    _cells[IndexOf(nx, y)] = Cell.Empty;
                }
            }
            _cells[IndexOf(cx, y)] = new Cell(grapheme, style, (byte)w, false);
            for (int k = 1; k < w; k++)
            {
                int nx = cx + k;
                if (nx >= Origin.X + Size.Width) break;
                _cells[IndexOf(nx, y)] = new Cell(grapheme, style, (byte)w, true);
            }
            cx += w;
        }

    }
}
