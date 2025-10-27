namespace CycoTui.Core.Widgets;

/// <summary>
/// State for ListWidget including selected index and vertical scroll offset.
/// </summary>
public sealed class ListState
{
    public int? Selected { get; private set; }
    public int Count { get; }
    public int Offset { get; private set; }

    public ListState(int count = 0)
    {
        Count = count;
    }

    public void Select(int? index, int viewportHeight)
    {
        if (index.HasValue && (index < 0 || index >= Count)) return;
        Selected = index;
        EnsureSelectedVisible(viewportHeight);
    }

    public void Next(int viewportHeight)
    {
        if (!Selected.HasValue) { if (Count > 0) Selected = 0; EnsureSelectedVisible(viewportHeight); return; }
        if (Selected.Value + 1 < Count) Selected = Selected.Value + 1;
        EnsureSelectedVisible(viewportHeight);
    }

    public void Previous(int viewportHeight)
    {
        if (!Selected.HasValue) { if (Count > 0) Selected = 0; EnsureSelectedVisible(viewportHeight); return; }
        if (Selected.Value - 1 >= 0) Selected = Selected.Value - 1;
        EnsureSelectedVisible(viewportHeight);
    }

    public void ScrollDown(int viewportHeight)
    {
        Offset = System.Math.Min(Offset + 1, System.Math.Max(0, Count - viewportHeight));
        EnsureSelectedVisible(viewportHeight);
    }

    public void ScrollUp(int viewportHeight)
    {
        Offset = System.Math.Max(Offset - 1, 0);
        EnsureSelectedVisible(viewportHeight);
    }

    private void EnsureSelectedVisible(int viewportHeight = 0)
    {
        if (!Selected.HasValue) return;
        if (viewportHeight <= 0) return; // require positive height to enforce
        if (Selected.Value < Offset) Offset = Selected.Value;
        else if (Selected.Value >= Offset + viewportHeight) Offset = Selected.Value - viewportHeight + 1;
    }
}
