namespace CycoTui.Core.Backend;

/// <summary>
/// Represents a width/height in character cells (non-negative logical units).
/// </summary>
public readonly struct Size : System.IEquatable<Size>
{
    public int Width { get; }
    public int Height { get; }

    public Size(int width, int height)
    {
        Width = width < 0 ? 0 : width;
        Height = height < 0 ? 0 : height;
    }

    public static Size Empty => new(0, 0);

    public override string ToString() => $"{Width}x{Height}";
    public bool Equals(Size other) => Width == other.Width && Height == other.Height;
    public override bool Equals(object? obj) => obj is Size s && Equals(s);
    public override int GetHashCode() => System.HashCode.Combine(Width, Height);
    public static bool operator ==(Size left, Size right) => left.Equals(right);
    public static bool operator !=(Size left, Size right) => !left.Equals(right);
}
