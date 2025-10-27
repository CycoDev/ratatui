using System;
using System.Collections.Generic;
using CycoTui.Core.Style;

namespace CycoTui.Core.Buffer;

/// <summary>
/// Provides diffing between two buffers producing per-cell updates or contiguous horizontal segments.
/// Phase-1: simple equality comparison (Symbol + Style). Multi-width invalidation to be added later.
/// </summary>
public static class BufferDiff
{
    /// <summary>
    /// Enumerate changed cells between <paramref name="previous"/> and <paramref name="current"/>.
    /// </summary>
    public static IEnumerable<ChangedCell> EnumerateCellDiff(BufferType previous, BufferType current)
    {
        if (previous.Size != current.Size)
            throw new ArgumentException("Buffers must have identical size for diff.");

        var width = current.Size.Width;
        var height = current.Size.Height;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var prev = previous.GetCell(x + previous.Origin.X, y + previous.Origin.Y);
                var cur = current.GetCell(x + current.Origin.X, y + current.Origin.Y);
                if (!prev.Equals(cur))
                {
                    yield return new ChangedCell(x + current.Origin.X, y + current.Origin.Y, cur);
                }
            }
        }
    }

    /// <summary>
    /// Enumerate changed segments (contiguous horizontal runs) between two buffers.
    /// </summary>
    public static IEnumerable<DiffSegment> EnumerateSegments(BufferType previous, BufferType current)
    {
        if (previous.Size != current.Size)
            throw new ArgumentException("Buffers must have identical size for diff.");

        var width = current.Size.Width;
        var height = current.Size.Height;
        for (int y = 0; y < height; y++)
        {
            int x = 0;
            while (x < width)
            {
                var prev = previous.GetCell(x + previous.Origin.X, y + previous.Origin.Y);
                var cur = current.GetCell(x + current.Origin.X, y + current.Origin.Y);
                if (prev.Equals(cur))
                {
                    x++;
                    continue;
                }
                int start = x;
                var cells = new List<Cell>();
                while (x < width)
                {
                    prev = previous.GetCell(x + previous.Origin.X, y + previous.Origin.Y);
                    cur = current.GetCell(x + current.Origin.X, y + current.Origin.Y);
                    if (!prev.Equals(cur))
                    {
                        cells.Add(cur);
                        x++;
                        continue;
                    }
                    break;
                }
                yield return new DiffSegment(y + current.Origin.Y, start + current.Origin.X, cells);
            }
        }
    }
}

/// <summary>
/// Represents a changed cell.
/// </summary>
public readonly struct ChangedCell
{
    public int X { get; }
    public int Y { get; }
    public Cell Cell { get; }
    public ChangedCell(int x, int y, Cell cell)
    {
        X = x; Y = y; Cell = cell;
    }
}

/// <summary>
/// Represents a contiguous horizontal run of changed cells on a single row.
/// </summary>
public sealed class DiffSegment
{
    public int Y { get; }
    public int StartX { get; }
    public IReadOnlyList<Cell> Cells { get; }
    public int Length => Cells.Count;

    public DiffSegment(int y, int startX, List<Cell> cells)
    {
        Y = y;
        StartX = startX;
        Cells = cells;
    }
}
