using System;
using System.Globalization;

namespace CycoTui.Core.Style;

/// <summary>
/// <summary>
/// Terminal color abstraction covering ANSI named (basic indices), 256-color indexed, and 24-bit RGB.
/// Use factory methods (AnsiIndex, Indexed, Rgb). Reset indicates terminal default color.
/// </summary>
/// </summary>
public readonly struct Color : IEquatable<Color>
{
    private readonly ColorKind _kind;
    private readonly byte _r;
    private readonly byte _g;
    private readonly byte _b;
    private readonly byte _index; // reused for Indexed, or for named lookup if needed

    private Color(ColorKind kind, byte r, byte g, byte b, byte index)
    {
        _kind = kind;
        _r = r; _g = g; _b = b; _index = index;
    }

    public static Color Reset => new(ColorKind.Reset, 0, 0, 0, 0);

    // ANSI base colors (subset, will expand when full enum mapping is added)
    public static Color Black => new(ColorKind.Ansi, 0, 0, 0, 0);
    public static Color Red => new(ColorKind.Ansi, 0, 0, 0, 1);
    public static Color Green => new(ColorKind.Ansi, 0, 0, 0, 2);
    public static Color Yellow => new(ColorKind.Ansi, 0, 0, 0, 3);
    public static Color Blue => new(ColorKind.Ansi, 0, 0, 0, 4);
    public static Color Magenta => new(ColorKind.Ansi, 0, 0, 0, 5);
    public static Color Cyan => new(ColorKind.Ansi, 0, 0, 0, 6);
    public static Color Gray => new(ColorKind.Ansi, 0, 0, 0, 7);

    public static Color AnsiIndex(byte i) => new(ColorKind.Ansi, 0, 0, 0, i);
    public static Color Indexed(byte i) => new(ColorKind.Indexed, 0, 0, 0, i);
    public static Color Rgb(byte r, byte g, byte b) => new(ColorKind.Rgb, r, g, b, 0);

    public ColorKind Kind => _kind;
    public byte R => _r;
    public byte G => _g;
    public byte B => _b;
    public byte Index => _index;

    public bool Equals(Color other) => _kind == other._kind && _r == other._r && _g == other._g && _b == other._b && _index == other._index;
    public override bool Equals(object? obj) => obj is Color c && Equals(c);
    public override int GetHashCode() => HashCode.Combine(_kind, _r, _g, _b, _index);
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !left.Equals(right);

    public override string ToString() => _kind switch
    {
        ColorKind.Reset => "Reset",
        ColorKind.Ansi => $"Ansi({Index})",
        ColorKind.Indexed => $"Indexed({Index})",
        ColorKind.Rgb => $"Rgb({_r},{_g},{_b})",
        _ => "Unknown"
    };
}

public enum ColorKind : byte
{
    Reset = 0,
    Ansi = 1,
    Indexed = 2,
    Rgb = 3
}
