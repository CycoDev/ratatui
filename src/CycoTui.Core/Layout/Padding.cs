namespace CycoTui.Core.Layout;

/// <summary>
/// Represents inner spacing inside a rectangle.
/// </summary>
public readonly struct Padding
{
    public int Left { get; }
    public int Top { get; }
    public int Right { get; }
    public int Bottom { get; }

    public Padding(int left, int top, int right, int bottom)
    {
        Left = left < 0 ? 0 : left;
        Top = top < 0 ? 0 : top;
        Right = right < 0 ? 0 : right;
        Bottom = bottom < 0 ? 0 : bottom;
    }

    public static Padding Zero => new(0,0,0,0);

    public Rect Apply(Rect rect)
    {
        var x = rect.X + Left;
        var y = rect.Y + Top;
        var w = rect.Width - (Left + Right);
        var h = rect.Height - (Top + Bottom);
        if (w < 0) w = 0;
        if (h < 0) h = 0;
        return new Rect(x, y, w, h);
    }
}
