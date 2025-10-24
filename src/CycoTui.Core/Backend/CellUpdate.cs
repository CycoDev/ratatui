namespace CycoTui.Core.Backend;

/// <summary>
/// Represents a single cell update instruction. Transitional until full buffer model lands.
/// </summary>
public readonly struct CellUpdate
{
    public int X { get; }
    public int Y { get; }
    public CellData Cell { get; }

    public CellUpdate(int x, int y, CellData cell)
    {
        X = x;
        Y = y;
        Cell = cell;
    }
}
