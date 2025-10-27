namespace CycoTui.Core.Widgets;

/// <summary>
/// Tracks horizontal scrolling position for a viewport. Immutable except for Offset mutation methods.
/// </summary>
public sealed class HorizontalScrollState
{
    public int Offset { get; private set; }
    public int ContentWidth { get; private set; }
    public int ViewportWidth { get; private set; }

    public HorizontalScrollState(int contentWidth, int viewportWidth, int initialOffset = 0)
    {
        ContentWidth = contentWidth;
        ViewportWidth = viewportWidth;
        Offset = Clamp(initialOffset);
    }

    private int Clamp(int offset) => offset < 0 ? 0 : (offset > MaxOffset ? MaxOffset : offset);
    public int MaxOffset => ContentWidth <= ViewportWidth ? 0 : ContentWidth - ViewportWidth;

    public void ScrollRight(int amount = 1) => Offset = Clamp(Offset + amount);
    public void ScrollLeft(int amount = 1) => Offset = Clamp(Offset - amount);
    public void Reset(int contentWidth, int viewportWidth)
    {
        ContentWidth = contentWidth;
        ViewportWidth = viewportWidth;
        Offset = Clamp(Offset);
    }
}
