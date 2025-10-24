namespace CycoTui.Core.Widgets;

/// <summary>
/// State for a List widget: selected index tracking.
/// </summary>
public sealed class ListState
{
    public int? Selected { get; private set; }

    public void Select(int? index)
    {
        Selected = index;
    }
}
