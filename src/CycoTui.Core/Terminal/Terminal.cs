using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using CycoTui.Core.Backend;
using CycoTui.Core.Buffer;
using CycoTui.Core.Logging;
using CycoTui.Core.Style;

namespace CycoTui.Core.Terminal;

/// <summary>
/// Provides the high-level drawing API for CycoTui.
/// Manages double-buffered rendering (previous/current) against an ITerminalBackend, performs diffing
/// and optimized style transitions, and emits a style reset only when styles changed.
/// Thread confinement: not thread-safe; callers must serialize Draw invocations.
/// </summary>
public sealed class Terminal : IDisposable
{
    private readonly ITerminalBackend _backend;
    private readonly LoggingContext _logging;
    private BufferType _previous;
    private BufferType _current;
    private bool _disposed;
    private bool _styleTransitionOccurred;
    // Style state tracker nested type
    private sealed class StyleState
    {
        private readonly BackendCapabilities _capabilities;
        private StyleType _current = StyleType.Empty;
        public bool HasActiveStyle => _current != StyleType.Empty;

        public StyleState(BackendCapabilities capabilities)
        {
            _capabilities = capabilities;
        }

        public string Apply(StyleType next)
        {
            if (_current.Equals(next)) return string.Empty;
            var seq = StyleType.StyleEmitterIntegration(_current, next, _capabilities.SupportsUnderlineColor, mapUnderlineToForeground: true);
            _current = next;
            return seq;
        }
    }

    /// <summary>
    /// Create a new terminal with the specified backend and logging context.
    /// </summary>
    /// <param name="backend">Backend implementing terminal operations.</param>
    /// <param name="logging">Logging context (null-safe; provides logger factory).</param>


    public Terminal(ITerminalBackend backend, LoggingContext logging)
    {
        _backend = backend;
        _logging = logging;
        var size = backend.GetSize();
        _previous = BufferType.Empty(size);
        _current = BufferType.Empty(size);
    }

    /// <summary>
    /// Perform a draw cycle: ensure buffer size, create frame, invoke <paramref name="render"/>, diff, emit changes.
    /// </summary>
    /// <param name="render">User rendering callback populating the frame.</param>
    public void Draw(Action<Frame> render)
    {
        EnsureSize();
        _styleTransitionOccurred = false;
        var frame = new Frame(_current);
        render(frame);
        EmitDiffAndSwap();
    }

    private void EnsureSize()
    {
        var size = _backend.GetSize();
        if (size != _current.Size)
        {
            _previous = BufferType.Empty(size);
            _current = BufferType.Empty(size);
            _logging.GetLogger<Terminal>().LogInformation("[Terminal] Resized buffers to {width}x{height}", size.Width, size.Height);
        }
    }

    private void EmitDiffAndSwap()
    {
        var logger = _logging.GetLogger<Terminal>();
        var segments = BufferDiff.EnumerateSegments(_previous, _current).ToList();
        logger.LogInformation("[Terminal] Changed segments: {count}", segments.Count);

        // Style tracking
        var styleState = new StyleState(_backend.Capabilities);
        var cellUpdates = new List<CellUpdate>();

        foreach (var seg in segments)
        {
            EmitSegment(seg, styleState, cellUpdates);
        }

        _backend.Draw(cellUpdates);
        _backend.Flush();
        EmitReset(styleState);
        logger.LogInformation("[Terminal] Emitted {cells} cells", cellUpdates.Count(c => c.X >= 0));
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
                _styleTransitionOccurred = true;
                _backend.WriteRaw(emittedStyleSeq);
            }
            // Emit cell symbol (full grapheme)
            cellUpdates.Add(new CellUpdate(seg.StartX + i, seg.Y, new CellData(cell.Grapheme)));
        }
    }

    private void EmitReset(StyleState styleState)
    {
        if (!_styleTransitionOccurred) return; // If no style transitions happened, skip reset
        _logging.GetLogger<Terminal>().LogInformation("[Terminal] Emitting style reset");
        _backend.WriteRaw("\u001b[0m");
    }

    private void SwapBuffers()
    {
        // Previous becomes old current.
        var tmp = _previous;
        _previous = _current;
        _current = tmp;
        _current.Clear(); // in-place reuse
    }

    /// <summary>Dispose backend and release resources. Safe to call multiple times.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _backend.Dispose();
        _disposed = true;
    }
}
