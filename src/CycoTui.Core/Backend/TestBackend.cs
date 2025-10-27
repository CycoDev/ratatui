using System.Collections.Generic;
using CycoTui.Core.Backend;

namespace CycoTui.Core.Backend;

/// <summary>
/// Deterministic test backend capturing emitted cell updates and tracking resets.
/// </summary>
public sealed class TestBackend : ITerminalBackend
{
    private readonly List<CellUpdate> _emitted = new();
    private readonly List<string> _rawSequences = new();
    private readonly List<IReadOnlyList<CellUpdate>> _frames = new();
    public IReadOnlyList<CellUpdate> Emitted => _emitted;
    public IReadOnlyList<string> RawSequences => _rawSequences;
    public bool ResetEmitted { get; private set; }

    public BackendCapabilities Capabilities { get; } = BackendCapabilities.Minimal;

    public void AppendLines(int count = 1) { }
    public void Clear(ClearType type = ClearType.All) { }
    public void Dispose() { }
    public void Draw(IEnumerable<CellUpdate> updates)
    {
        var list = new List<CellUpdate>();
        foreach (var u in updates)
        {
            list.Add(u);
            _emitted.Add(u);
        }
        _frames.Add(list);
    }
    public IReadOnlyList<IReadOnlyList<CellUpdate>> Frames => _frames;

    public void WriteRaw(string sequence)
    {
        if (string.IsNullOrEmpty(sequence)) return;
        _rawSequences.Add(sequence);
        if (sequence.Contains("\u001b[0m")) MarkReset();
    }
    public void Flush() { }
    public Position GetCursorPosition() => new(0,0);
    public Size GetSize() => new(120,40); // Expanded size to satisfy widget tests expecting wider areas
    public WindowSize GetWindowSize() => new(new Size(120,40), Size.Empty);
    public void HideCursor() { }
    public void ScrollRegionDown(Range region, int lineCount) { }
    public void ScrollRegionUp(Range region, int lineCount) { }
    public void SetCursorPosition(Position position) { }
    public void ShowCursor() { }

    public void MarkReset() => ResetEmitted = true;
}
