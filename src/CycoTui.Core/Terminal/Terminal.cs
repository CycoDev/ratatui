using System;
using System.Collections.Generic;
using System.Linq;
using CycoTui.Core.Backend;
using CycoTui.Core.Buffer;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;

namespace CycoTui.Core.Terminal;

/// <summary>
/// Manages double-buffered rendering using an ITerminalBackend.
/// Responsibilities:
/// - Maintain previous and current buffers for diffing
/// - Invoke user render callback to populate frame
/// - Diff and emit changed cells + style transitions
/// - Emit style reset only when styles changed
/// NOTE: Not thread-safe; single UI thread confinement required.
/// </summary>
public sealed class Terminal : IDisposable
{
    private readonly ITerminalBackend _backend;
    private readonly LoggingContext _logging;
    private Buffer _previous;
    private Buffer _current;
    private bool _disposed;
    // Style state tracker nested type
    private sealed class StyleState
    {
        private Style _current = Style.Empty;
        public bool HasActiveStyle => _current != Style.Empty;
    /// <summary>
    /// Create a new terminal with the specified backend and logging context.
    /// </summary>
    /// <param name="backend">Backend implementing terminal operations.</param>
    /// <param name="logging">Logging context (null-safe; provides logger factory).</param>

        public string Apply(Style next)
        {
            if (_current.Equals(next)) return string.Empty;
            var seq = Style.StyleEmitterIntegration(_current, next);
            _current = next;
            return seq;
        }
    }


    public Terminal(ITerminalBackend backend, LoggingContext logging)
    {
        _backend = backend;
        _logging = logging;
        var size = backend.GetSize();
        _previous = Buffer.Empty(size);
        _current = Buffer.Empty(size);
    }

    /// <summary>
    /// Perform a draw cycle: ensure buffer size, create frame, invoke <paramref name="render"/>, diff, emit changes.
    /// </summary>
    /// <param name="render">User rendering callback populating the frame.</param>
    public void Draw(Action<Frame> render)
    {
        EnsureSize();
        var frame = new Frame(_current);
        render(frame);
        EmitDiffAndSwap();
    }

    private void EnsureSize()
    {
        var size = _backend.GetSize();
        if (size != _current.Size)
        {
            _previous = Buffer.Empty(size);
            _current = Buffer.Empty(size);
            _logging.GetLogger<Terminal>().LogDebug("[Terminal] Resized buffers to {width}x{height}", size.Width, size.Height);
        }
    }

    private void EmitDiffAndSwap()
    {
        var logger = _logging.GetLogger<Terminal>();
        var segments = BufferDiff.EnumerateSegments(_previous, _current).ToList();
        logger.LogDebug("[Terminal] Changed segments: {count}", segments.Count);

        // Style tracking
        var styleState = new StyleState();
        var cellUpdates = new List<CellUpdate>();

        foreach (var seg in segments)
        {
            EmitSegment(seg, styleState, cellUpdates);
        }

        _backend.Draw(cellUpdates);
        _backend.Flush();
        EmitReset(styleState);
        logger.LogDebug("[Terminal] Emitted {cells} cells", cellUpdates.Count(c => c.X >= 0));
        SwapBuffers();
    }

    private void EmitSegment(DiffSegment seg, StyleState styleState, List<CellUpdate> cellUpdates)
    {
        for (int i = 0; i < seg.Length; i++)
        {
            var cell = seg.Cells[i];
            if (cell.Skip) continue;
            var emittedStyleSeq = styleState.Apply(cell.Style);
            if (!string.IsNullOrEmpty(emittedStyleSeq))
            {
                // Convert style sequences into artificial cell updates (placeholder until backend supports raw sequence emission)
                // Using X/Y of -1 indicates style-only update; backend may interpret specially later.
                _backend.WriteRaw(emittedStyleSeq);
            }
            // Emit cell symbol (first char only for now) — TODO handle full grapheme emission in backend
            cellUpdates.Add(new CellUpdate(seg.StartX + i, seg.Y, new CellData(cell.Grapheme[0])));
        }
    }

    private void EmitReset(StyleState styleState)
    {
        if (!styleState.HasActiveStyle) return; // If no style changes happened, skip reset
        _logging.GetLogger<Terminal>().LogDebug("[Terminal] Emitting style reset");
        _backend.WriteRaw("\u001b[0m");
    }

    private void SwapBuffers()
    {
        // Reuse current buffer as previous without reallocation, allocate a fresh current and reuse previous storage next frame.
        _previous = _current;
        var size = _backend.GetSize();
        // Reuse existing previous buffer cells by clearing rather than allocating a new one.
        _current = ReuseOrAllocate(size);
    }

    private Buffer ReuseOrAllocate(Size size)
    {
        // If dimensions changed we allocated fresh earlier in EnsureSize.
        return Buffer.Empty(size); // TODO: Implement in-place clear & reuse pool
    }

    /// <summary>Dispose backend and release resources. Safe to call multiple times.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _backend.Dispose();
        _disposed = true;
    }
}
