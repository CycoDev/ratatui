namespace CycoTui.Core.Backend;

/// <summary>
/// Represents an absolute terminal coordinate (0-based, origin at top-left).
/// </summary>
public readonly struct Position : System.IEquatable<Position>
{
    public int X { get; }
    public int Y { get; }

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override string ToString() => $"({X},{Y})";
    public bool Equals(Position other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Position p && Equals(p);
    public override int GetHashCode() => System.HashCode.Combine(X, Y);
    public static bool operator ==(Position left, Position right) => left.Equals(right);
    public static bool operator !=(Position left, Position right) => !left.Equals(right);
}
