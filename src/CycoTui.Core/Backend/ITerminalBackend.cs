using System;
using System.Collections.Generic;

namespace CycoTui.Core.Backend;

/// <summary>
/// Abstraction for low-level terminal operations. Implementations are not thread-safe
/// and expected to be confined to a single UI thread. Optional operations (scrolling,
/// append lines) MUST perform a no-op if the capability is not supported; they MUST NOT throw.
/// </summary>
public interface ITerminalBackend : IDisposable
{
    /// <summary>
    /// Draw a sequence of cell updates. Each update represents a single absolute cell to be written.
    /// Implementations should batch writes and minimize cursor movement.
    /// </summary>
    void Draw(IEnumerable<CellUpdate> updates);

    /// <summary>Write a raw ANSI/terminal sequence directly (style changes, cursor moves, etc.).</summary>
    void WriteRaw(string sequence);

    /// <summary>Flush any buffered output to the terminal device.</summary>
    void Flush();

    /// <summary>Hide the cursor (no-op if unsupported).</summary>
    void HideCursor();

    /// <summary>Show the cursor (no-op if unsupported).</summary>
    void ShowCursor();

    /// <summary>Get current cursor position (best effort; may return (0,0) if unknown).</summary>
    Position GetCursorPosition();

    /// <summary>Set cursor position (clamped silently if outside bounds).</summary>
    void SetCursorPosition(Position position);

    /// <summary>Clear content based on <paramref name="type"/>.</summary>
    void Clear(ClearType type = ClearType.All);

    /// <summary>Append <paramref name="count"/> new lines below the cursor if supported; otherwise no-op.</summary>
    void AppendLines(int count = 1);

    /// <summary>Get current terminal character size (columns x rows).</summary>
    Size GetSize();

    /// <summary>Get window size including pixel metrics if available (Pixels may be (0,0)).</summary>
    WindowSize GetWindowSize();

    /// <summary>Scroll a region up by <paramref name="lineCount"/> lines (no-op if unsupported).</summary>
    void ScrollRegionUp(Range region, int lineCount);

    /// <summary>Scroll a region down by <paramref name="lineCount"/> lines (no-op if unsupported).</summary>
    void ScrollRegionDown(Range region, int lineCount);

    /// <summary>Backend capability snapshot populated at initialization.</summary>
    BackendCapabilities Capabilities { get; }
}

// NOTE: Remaining related types moved to separate files as part of refactor.
