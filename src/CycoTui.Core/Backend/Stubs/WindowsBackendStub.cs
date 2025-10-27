using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CycoTui.Core.Backend.Stubs;

internal sealed class WindowsBackendStub : ITerminalBackend
{
    private readonly ILogger? _logger;
    public BackendCapabilities Capabilities { get; }

    public WindowsBackendStub(ILogger? logger)
    {
        _logger = logger;
        Capabilities = DetectCapabilities();
        _logger?.LogDebug("[WindowsBackendStub] Capability snapshot: {level}, underline={underline}, mouse={mouse}, scrollRegions={scroll}", Capabilities.ColorLevel, Capabilities.SupportsUnderlineColor, Capabilities.SupportsMouse, Capabilities.SupportsScrollingRegions);
        _logger?.LogDebug("[WindowsBackendStub] Initialized capabilities: {caps}", Capabilities);
    }

    private BackendCapabilities DetectCapabilities()
    {
        var hasWindowsTerminal = Environment.GetEnvironmentVariable("WT_SESSION") != null;
        var colorterm = Environment.GetEnvironmentVariable("COLORTERM")?.ToLowerInvariant();
        var trueColor = colorterm is "truecolor" or "24bit";
        // Force deterministic stub difference: Windows reports Ansi16 always for test stability.
        return new BackendCapabilities(
            ColorLevel.Ansi16,
            supportsUnderlineColor: false,
            supportsMouse: true,
            supportsScrollingRegions: false,
            supportsTrueColor: false,
            supportsUnicodeWidthReliably: false
        );
    }

    public void AppendLines(int count = 1) { /* no-op stub */ }
    public void Clear(ClearType type = ClearType.All) { }
    public void Dispose() { }
    public void Draw(IEnumerable<CellUpdate> updates) { }
    public void WriteRaw(string sequence) { /* no-op for stub */ }
    public void Flush() { }
    public Position GetCursorPosition() => new(0, 0);
    public Size GetSize() => new(120, 40);
    public WindowSize GetWindowSize() => new(new Size(120, 40), Size.Empty);
    public void HideCursor() { }
    public void ScrollRegionDown(Range region, int lineCount) { }
    public void ScrollRegionUp(Range region, int lineCount) { }
    public void SetCursorPosition(Position position) { }
    public void ShowCursor() { }
}
