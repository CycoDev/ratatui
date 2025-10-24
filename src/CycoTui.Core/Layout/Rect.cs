namespace CycoTui.Core.Layout;

/// <summary>
/// Represents an axis-aligned rectangle using top-left origin and width/height.
/// </summary>
public readonly struct Rect
{
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }

    public Rect(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width < 0 ? 0 : width;
        Height = height < 0 ? 0 : height;
    }

    public static Rect Empty => new(0,0,0,0);

    public bool Contains(int x, int y) => x >= X && y >= Y && x < X + Width && y < Y + Height;
    public bool Contains(Rect other) => other.X >= X && other.Y >= Y && other.X + other.Width <= X + Width && other.Y + other.Height <= Y + Height;

    public Rect Intersect(Rect other)
    {
        var nx = X > other.X ? X : other.X;
        var ny = Y > other.Y ? Y : other.Y;
        var rx = (X + Width) < (other.X + other.Width) ? (X + Width) : (other.X + other.Width);
        var ry = (Y + Height) < (other.Y + other.Height) ? (Y + Height) : (other.Y + other.Height);
        var w = rx - nx;
        var h = ry - ny;
        if (w <= 0 || h <= 0) return Empty;
        return new Rect(nx, ny, w, h);
    }

    public override string ToString() => $"Rect({X},{Y},{Width}x{Height})";
}
