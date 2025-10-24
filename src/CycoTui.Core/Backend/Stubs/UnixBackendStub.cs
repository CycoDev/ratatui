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
    private BackendCapabilities DetectCapabilities()
    {
        var colorterm = Environment.GetEnvironmentVariable("COLORTERM")?.ToLowerInvariant();
        var term = Environment.GetEnvironmentVariable("TERM")?.ToLowerInvariant();
        bool trueColor = colorterm is "truecolor" or "24bit" || (term != null && term.Contains("truecolor"));
        bool ansi256 = term != null && (term.Contains("256") || term.Contains("xterm"));
        var level = trueColor ? ColorLevel.TrueColor : (ansi256 ? ColorLevel.Ansi256 : ColorLevel.Ansi16);
        return new BackendCapabilities(
            level,
            supportsUnderlineColor: false,
            supportsMouse: true, // optimistic placeholder
            supportsScrollingRegions: true, // common in modern terminals
            supportsTrueColor: trueColor,
            supportsUnicodeWidthReliably: level == ColorLevel.TrueColor // placeholder heuristic
        );
    }

        _logger?.LogDebug("[UnixBackendStub] Initialized capabilities: {caps}", Capabilities);
    }

    public void AppendLines(int count = 1) { }
    public void Clear(ClearType type = ClearType.All) { }
    public void Dispose() { }
    public void Draw(IEnumerable<CellUpdate> updates) { }
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
