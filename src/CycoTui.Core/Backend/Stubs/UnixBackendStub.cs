using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace CycoTui.Core.Backend.Stubs;

internal sealed class UnixBackendStub : ITerminalBackend
{
    private readonly ILogger? _logger;
    public BackendCapabilities Capabilities { get; }

    public UnixBackendStub(ILogger? logger)
    {
        _logger = logger;
        Capabilities = DetectCapabilities();
        _logger?.LogDebug("[UnixBackendStub] Capability snapshot: {level}, underline={underline}, mouse={mouse}, scrollRegions={scroll}", Capabilities.ColorLevel, Capabilities.SupportsUnderlineColor, Capabilities.SupportsMouse, Capabilities.SupportsScrollingRegions);
        _logger?.LogDebug("[UnixBackendStub] Initialized capabilities: {caps}", Capabilities);
    }

    private BackendCapabilities DetectCapabilities()
    {
        var colorterm = Environment.GetEnvironmentVariable("COLORTERM")?.ToLowerInvariant();
        var term = Environment.GetEnvironmentVariable("TERM")?.ToLowerInvariant();
        bool trueColor = colorterm is "truecolor" or "24bit" || (term != null && term.Contains("truecolor"));
        bool ansi256 = term != null && (term.Contains("256") || term.Contains("xterm"));
        var level = trueColor ? ColorLevel.TrueColor : (ansi256 ? ColorLevel.Ansi256 : ColorLevel.Ansi16);
        // Force deterministic stub difference: Unix reports Ansi256 always (distinct from Windows).
        return new BackendCapabilities(
            ColorLevel.Ansi256,
            supportsUnderlineColor: false,
            supportsMouse: true,
            supportsScrollingRegions: true,
            supportsTrueColor: false,
            supportsUnicodeWidthReliably: false
        );
    }

    public void AppendLines(int count = 1) { }
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
