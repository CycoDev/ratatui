using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CycoTui.Core.Backend.Stubs;

internal sealed class MinimalBackendStub : ITerminalBackend
{
    private readonly ILogger? _logger;
    public BackendCapabilities Capabilities { get; }

    public MinimalBackendStub(ILogger? logger)
    {
        _logger = logger;
        Capabilities = BackendCapabilities.Minimal; // no detection for minimal backend
        _logger?.LogDebug("[MinimalBackendStub] Initialized minimal capabilities");
    }

    public void AppendLines(int count = 1) { }
    public void Clear(ClearType type = ClearType.All) { }
    public void Dispose() { }
    public void Draw(IEnumerable<CellUpdate> updates) { }
    public void WriteRaw(string sequence) { /* no-op for stub */ }
    public void Flush() { }
    public Position GetCursorPosition() => new(0, 0);
    public Size GetSize() => new(80, 25);
    public WindowSize GetWindowSize() => new(new Size(80, 25), Size.Empty);
    public void HideCursor() { }
    public void ScrollRegionDown(Range region, int lineCount) { }
    public void ScrollRegionUp(Range region, int lineCount) { }
    public void SetCursorPosition(Position position) { }
    public void ShowCursor() { }
}
