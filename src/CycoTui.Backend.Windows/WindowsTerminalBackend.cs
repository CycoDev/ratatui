using System;
// TODO: Capability probing (underline color, scrolling regions, true color); buffering optimization

using System.Collections.Generic;
using CycoTui.Core.Backend;
using CycoTui.Core.Style;

namespace CycoTui.Backend.Windows;

/// <summary>
/// Minimal Windows console backend implementation. Phase-1: basic raw write, size, cursor show/hide.
/// </summary>
public sealed class WindowsTerminalBackend : ITerminalBackend
{
    private BackendCapabilities ProbeCapabilities()
    {
        // Simple heuristic probing placeholder
        // TODO: real probing (e.g., check WT_SESSION for Windows Terminal truecolor)
        return BackendCapabilities.Minimal;
    }
    private bool _disposed;
    public BackendCapabilities Capabilities { get; }

    public WindowsTerminalBackend()
    {
        Capabilities = ProbeCapabilities();

    }

    public void Draw(IEnumerable<CellUpdate> updates)
    {
        foreach (var u in updates)
        {
            if (u.X < 0 || u.Y < 0) continue; // style-only placeholder not used here
            TrySetCursor(u.X, u.Y);
            Console.Write(u.Cell.Grapheme);
        }
    }

    private static void TrySetCursor(int x, int y)
    {
        try
        {
            Console.SetCursorPosition(x, y);
        }
        catch { /* Ignore out-of-range */ }
    }

    public void WriteRaw(string sequence)
    {
        Console.Write(sequence);
    }

    public void Flush() { /* Console.Write is immediate */ }
    public void HideCursor() { try { Console.CursorVisible = false; } catch { } }
    public void ShowCursor() { try { Console.CursorVisible = true; } catch { } }
    public Position GetCursorPosition() => new(Console.CursorLeft, Console.CursorTop);
    public void SetCursorPosition(Position position) => TrySetCursor(position.X, position.Y);
    public void Clear(ClearType type = ClearType.All)
    {
        if (type == ClearType.All)
        {
            Console.Clear();
        }
    }
    public void AppendLines(int count = 1)
    {
        for (int i = 0; i < count; i++) Console.WriteLine();
    }
    public Size GetSize() => new(Console.BufferWidth, Console.BufferHeight);
    public WindowSize GetWindowSize() => new(new Size(Console.BufferWidth, Console.BufferHeight), Size.Empty);
    public void ScrollRegionUp(Range region, int lineCount) { /* no-op phase-1 */ }
    public void ScrollRegionDown(Range region, int lineCount) { /* no-op phase-1 */ }

    public void Dispose()
    {
        if (_disposed) return;
        ShowCursor();
        _disposed = true;
    }
}
