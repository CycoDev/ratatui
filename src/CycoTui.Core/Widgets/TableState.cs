namespace CycoTui.Core.Widgets;

/// <summary>
/// State for TableWidget tracking selected row and vertical scroll offset.
/// </summary>
public sealed class TableState
{
    public int? SelectedRow { get; private set; }
    public int RowCount { get; }
    public int Offset { get; private set; }

    public TableState(int rowCount = 0)
    {
        RowCount = rowCount;
    }

    public void Select(int? row, int viewportHeight)
    {
        if (row.HasValue && (row < 0 || row >= RowCount)) return;
        SelectedRow = row;
        EnsureVisible(viewportHeight);
    }

    public void Next(int viewportHeight)
    {
        if (!SelectedRow.HasValue) { if (RowCount > 0) SelectedRow = 0; EnsureVisible(viewportHeight); return; }
        if (SelectedRow.Value + 1 < RowCount) SelectedRow = SelectedRow.Value + 1;
        EnsureVisible(viewportHeight);
    }

    public void Previous(int viewportHeight)
    {
        if (!SelectedRow.HasValue) { if (RowCount > 0) SelectedRow = 0; EnsureVisible(viewportHeight); return; }
        if (SelectedRow.Value - 1 >= 0) SelectedRow = SelectedRow.Value - 1;
        EnsureVisible(viewportHeight);
    }

    public void ScrollDown(int viewportHeight)
    {
        Offset = System.Math.Min(Offset + 1, System.Math.Max(0, RowCount - viewportHeight));
        EnsureVisible(viewportHeight);
    }

    public void ScrollUp(int viewportHeight)
    {
        Offset = System.Math.Max(Offset - 1, 0);
        EnsureVisible(viewportHeight);
    }

    private void EnsureVisible(int viewportHeight = 0)
    {
        if (!SelectedRow.HasValue || viewportHeight <= 0) return;
        // Removed upward pinning to allow scroll while first row selected
        // if (SelectedRow.Value < Offset) Offset = SelectedRow.Value;
        else if (SelectedRow.Value >= Offset + viewportHeight) Offset = SelectedRow.Value - viewportHeight + 1;
    }
}
