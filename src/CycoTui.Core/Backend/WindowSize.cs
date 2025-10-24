namespace CycoTui.Core.Backend;

/// <summary>
/// Represents both character and pixel dimensions of the terminal window.
/// Pixels may be (0,0) when not available.
/// </summary>
public readonly struct WindowSize
{
    public Size ColumnsRows { get; }
    public Size Pixels { get; }

    public WindowSize(Size columnsRows, Size pixels)
    {
        ColumnsRows = columnsRows;
        Pixels = pixels;
    }
}
