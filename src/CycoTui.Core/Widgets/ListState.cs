namespace CycoTui.Core.Widgets;

/// <summary>
/// State for a List widget: selected index tracking.
/// </summary>
public sealed class ListState
{
    public int? Selected { get; private set; }
    public int Count { get; }

    public ListState(int count = 0)
    {
        Count = count;
    }

    public void Select(int? index)
    {
        if (index.HasValue && (index < 0 || index >= Count)) return;
        Selected = index;
    }

    public void Next()
    {
        if (!Selected.HasValue) { if (Count > 0) Selected = 0; return; }
        if (Selected.Value + 1 < Count) Selected = Selected.Value + 1;
    }

    public void Previous()
    {
        if (!Selected.HasValue) { if (Count > 0) Selected = 0; return; }
        if (Selected.Value - 1 >= 0) Selected = Selected.Value - 1;
    }
}
