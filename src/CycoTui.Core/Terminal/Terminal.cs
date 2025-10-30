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

        foreach (var seg in segments)
        {
            EmitSegment(seg, styleState);
        }

        _backend.Flush();
        EmitReset(styleState);
        logger.LogInformation("[Terminal] Emitted segment cells");
        SwapBuffers();
    }

    private void EmitSegment(DiffSegment seg, StyleState styleState)
    {
        // Emit each cell immediately with its style - no batching
        for (int i = 0; i < seg.Length; i++)
        {
            var cell = seg.Cells[i];
            if (cell.Skip) continue;

            // Emit style transition if needed
            var emittedStyleSeq = styleState.Apply(cell.Style);
            if (!string.IsNullOrEmpty(emittedStyleSeq))
            {
                _styleTransitionOccurred = true;
                var compressed = CompressStyleSequence(emittedStyleSeq);
                _backend.WriteRaw(compressed);
            }

            // Emit cell immediately after style
            var cellUpdate = new CellUpdate(seg.StartX + i, seg.Y, new CellData(cell.Grapheme));
            _backend.Draw(new[] { cellUpdate });
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

    private static string CompressStyleSequence(string seq)
    {
        // Very small heuristic compression: collapse consecutive CSI sequences by merging their parameter lists
        // Example: "\u001b[1m\u001b[3m" -> "\u001b[1;3m".
        // Implementation: parse simple pattern ESC[<codes>m repeated.
        if (string.IsNullOrEmpty(seq)) return seq;
        var parts = new List<string>();
        int i = 0;
        while (i < seq.Length)
        {
            int esc = seq.IndexOf("\u001b[", i, StringComparison.Ordinal);
            if (esc < 0) break;
            int m = seq.IndexOf('m', esc);
            if (m < 0) break;
            var payload = seq.Substring(esc + 2, m - (esc + 2)); // between '[' and 'm'
            parts.Add(payload);
            i = m + 1;
        }
        if (parts.Count <= 1) return seq;
        // Attempt merge if all fragments are simple numbers or number;number forms (avoid complex sequences like 38;5)
        var simple = parts.All(p => p.Split(';').All(s => int.TryParse(s, out _)));
        if (!simple) return seq; // do not risk altering complex 38;2;r;g;b sequences ordering
        // Flatten and de-duplicate while preserving order
        var mergedTokens = new List<string>();
        var seen = new HashSet<string>();
        foreach (var p in parts)
        {
            foreach (var token in p.Split(';'))
            {
                if (token.Length == 0) continue;
                if (seen.Add(token)) mergedTokens.Add(token);
            }
        }
        return $"\u001b[{string.Join(';', mergedTokens)}m";
    }


}
