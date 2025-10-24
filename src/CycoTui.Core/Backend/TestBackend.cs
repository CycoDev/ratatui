using System.Collections.Generic;
using CycoTui.Core.Backend;

namespace CycoTui.Core.Backend;

/// <summary>
/// Deterministic test backend capturing emitted cell updates and tracking resets.
/// </summary>
public sealed class TestBackend : ITerminalBackend
{
    private readonly List<CellUpdate> _emitted = new();
    public IReadOnlyList<CellUpdate> Emitted => _emitted;
    public bool ResetEmitted { get; private set; }

    public BackendCapabilities Capabilities { get; } = BackendCapabilities.Minimal;

    public void AppendLines(int count = 1) { }
    public void Clear(ClearType type = ClearType.All) { }
    public void Dispose() { }
    public void Draw(IEnumerable<CellUpdate> updates)
    {
        _emitted.AddRange(updates);
    }
    public void WriteRaw(string sequence)
    {
        if (sequence.Contains("\u001b[0m")) MarkReset();
        // Optionally capture raw sequences separately later.
    }
    public void Flush() { }
    public Position GetCursorPosition() => new(0,0);
    public Size GetSize() => new(10,5);
    public WindowSize GetWindowSize() => new(new Size(10,5), Size.Empty);
    public void HideCursor() { }
    public void ScrollRegionDown(Range region, int lineCount) { }
    public void ScrollRegionUp(Range region, int lineCount) { }
    public void SetCursorPosition(Position position) { }
    public void ShowCursor() { }

    public void MarkReset() => ResetEmitted = true;
}
