using System;
// TODO: Capability probing (underline color, scrolling regions, mouse); buffering optimization

using System.Collections.Generic;
using CycoTui.Core.Backend;
using CycoTui.Core.Style;

namespace CycoTui.Backend.Unix;

/// <summary>
/// Minimal ANSI/Unix backend. Phase-1 implementation writing raw ANSI directly.
/// </summary>
public sealed class UnixTerminalBackend : ITerminalBackend
{
    private BackendCapabilities ProbeCapabilities()
    {
        var colorterm = Environment.GetEnvironmentVariable("COLORTERM") ?? string.Empty;
        var trueColor = colorterm.Contains("truecolor", StringComparison.OrdinalIgnoreCase);
        var level = trueColor ? ColorLevel.TrueColor : ColorLevel.Ansi256;
        return new BackendCapabilities(level, supportsUnderlineColor: false, supportsMouse: true, supportsScrollingRegions: true, supportsTrueColor: trueColor, supportsUnicodeWidthReliably: false);
    }
    private bool _disposed;
    public BackendCapabilities Capabilities { get; }

    public UnixTerminalBackend()
    {
        Capabilities = ProbeCapabilities();
        // Phase-1: capability probing omitted.
    }

    public void Draw(IEnumerable<CellUpdate> updates)
    {
        foreach (var u in updates)
        {
            if (u.X < 0 || u.Y < 0) continue;
            WriteRaw($"\u001b[{u.Y + 1};{u.X + 1}H"); // ANSI cursor move is 1-based
            Console.Write(u.Cell.Grapheme);
        }
    }

    public void WriteRaw(string sequence) => Console.Write(sequence);
    public void Flush() { /* stdout unbuffered enough for phase-1 */ }
    public void HideCursor() => Console.Write("\u001b[?25l");
    public void ShowCursor() => Console.Write("\u001b[?25h");
    public Position GetCursorPosition() => new(0,0); // Phase-1 placeholder
    public void SetCursorPosition(Position position) => WriteRaw($"\u001b[{position.Y + 1};{position.X + 1}H");
    public void Clear(ClearType type = ClearType.All)
    {
        if (type == ClearType.All) WriteRaw("\u001b[2J\u001b[H");
    }
    public void AppendLines(int count = 1)
    {
        for (int i = 0; i < count; i++) Console.Write("\n");
    }
    public Size GetSize()
    {
        try
        {
            // Attempt environment-based size detection; fallback 80x24.
            int w = Console.BufferWidth;
            int h = Console.BufferHeight;
            return new Size(w, h);
        }
        catch { return new Size(80,24); }
    }
    public WindowSize GetWindowSize() => new(GetSize(), Size.Empty);
    public void ScrollRegionUp(Range region, int lineCount) { /* no-op phase-1 */ }
    public void ScrollRegionDown(Range region, int lineCount) { /* no-op phase-1 */ }

    public void Dispose()
    {
        if (_disposed) return;
        ShowCursor();
        _disposed = true;
    }
}
