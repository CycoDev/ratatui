using System;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Style
{
    /// <summary>
    /// Represents a color that can be used for terminal text foreground or background.
    /// Supports 16-color, 256-color, and RGB color modes.
    /// </summary>
    public readonly struct Color : IEquatable<Color>
    {
        private readonly ColorKind _kind;
        private readonly byte _red;
        private readonly byte _green;
        private readonly byte _blue;
        private readonly byte _index;

        /// <summary>
        /// Gets the default terminal color (usually terminal's configured foreground/background).
        /// </summary>
        public static Color Default => new Color(ColorKind.Default);

        /// <summary>
        /// Gets the black color.
        /// </summary>
        public static Color Black => new Color(ColorKind.Ansi, 0);

        /// <summary>
        /// Gets the red color.
        /// </summary>
        public static Color Red => new Color(ColorKind.Ansi, 1);

        /// <summary>
        /// Gets the green color.
        /// </summary>
        public static Color Green => new Color(ColorKind.Ansi, 2);

        /// <summary>
        /// Gets the yellow color.
        /// </summary>
        public static Color Yellow => new Color(ColorKind.Ansi, 3);

        /// <summary>
        /// Gets the blue color.
        /// </summary>
        public static Color Blue => new Color(ColorKind.Ansi, 4);

        /// <summary>
        /// Gets the magenta color.
        /// </summary>
        public static Color Magenta => new Color(ColorKind.Ansi, 5);

        /// <summary>
        /// Gets the cyan color.
        /// </summary>
        public static Color Cyan => new Color(ColorKind.Ansi, 6);

        /// <summary>
        /// Gets the white color.
        /// </summary>
        public static Color White => new Color(ColorKind.Ansi, 7);

        /// <summary>
        /// Gets the bright black (gray) color.
        /// </summary>
        public static Color Gray => new Color(ColorKind.Ansi, 8);

        /// <summary>
        /// Gets the bright red color.
        /// </summary>
        public static Color LightRed => new Color(ColorKind.Ansi, 9);

        /// <summary>
        /// Gets the bright green color.
        /// </summary>
        public static Color LightGreen => new Color(ColorKind.Ansi, 10);

        /// <summary>
        /// Gets the bright yellow color.
        /// </summary>
        public static Color LightYellow => new Color(ColorKind.Ansi, 11);

        /// <summary>
        /// Gets the bright blue color.
        /// </summary>
        public static Color LightBlue => new Color(ColorKind.Ansi, 12);

        /// <summary>
        /// Gets the bright magenta color.
        /// </summary>
        public static Color LightMagenta => new Color(ColorKind.Ansi, 13);

        /// <summary>
        /// Gets the bright cyan color.
        /// </summary>
        public static Color LightCyan => new Color(ColorKind.Ansi, 14);

        /// <summary>
        /// Gets the bright white color.
        /// </summary>
        public static Color LightGray => new Color(ColorKind.Ansi, 15);

        private Color(ColorKind kind, byte index = 0, byte red = 0, byte green = 0, byte blue = 0)
        {
            _kind = kind;
            _index = index;
            _red = red;
            _green = green;
            _blue = blue;
        }

        /// <summary>
        /// Creates an RGB color.
        /// </summary>
        /// <param name="red">The red component (0-255).</param>
        /// <param name="green">The green component (0-255).</param>
        /// <param name="blue">The blue component (0-255).</param>
        /// <returns>An RGB color.</returns>
        public static Color FromRgb(byte red, byte green, byte blue) => new Color(ColorKind.Rgb, 0, red, green, blue);

        /// <summary>
        /// Creates an RGB color from integer values.
        /// </summary>
        /// <param name="red">The red component (0-255).</param>
        /// <param name="green">The green component (0-255).</param>
        /// <param name="blue">The blue component (0-255).</param>
        /// <returns>An RGB color.</returns>
        public static Color FromRgb(int red, int green, int blue) =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            FromRgb((byte)Compat.Clamp(red, 0, 255), (byte)Compat.Clamp(green, 0, 255), (byte)Compat.Clamp(blue, 0, 255));
#else
            FromRgb((byte)Math.Clamp(red, 0, 255), (byte)Math.Clamp(green, 0, 255), (byte)Math.Clamp(blue, 0, 255));
#endif

        /// <summary>
        /// Creates a color from a hex string (e.g., "#FF0000" or "FF0000").
        /// </summary>
        /// <param name="hex">The hex color string.</param>
        /// <returns>An RGB color parsed from the hex string.</returns>
        /// <exception cref="ArgumentException">Thrown when the hex string is invalid.</exception>
        public static Color FromHex(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                throw new ArgumentException("Hex string cannot be null or empty.", nameof(hex));

            hex = hex.TrimStart('#');
            if (hex.Length != 6)
                throw new ArgumentException("Hex string must be 6 characters long (RRGGBB).", nameof(hex));

            try
            {
                var red = Convert.ToByte(hex.Substring(0, 2), 16);
                var green = Convert.ToByte(hex.Substring(2, 2), 16);
                var blue = Convert.ToByte(hex.Substring(4, 2), 16);
                return FromRgb(red, green, blue);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Invalid hex color string: {hex}", nameof(hex), ex);
            }
        }

        /// <summary>
        /// Creates a 256-color palette color.
        /// </summary>
        /// <param name="index">The color index (0-255).</param>
        /// <returns>A 256-color palette color.</returns>
        public static Color FromIndex(byte index) => new Color(ColorKind.Indexed, index);

        /// <summary>
        /// Creates a 256-color palette color.
        /// </summary>
        /// <param name="index">The color index (0-255).</param>
        /// <returns>A 256-color palette color.</returns>
        public static Color FromIndex(int index) =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            FromIndex((byte)Compat.Clamp(index, 0, 255));
#else
            FromIndex((byte)Math.Clamp(index, 0, 255));
#endif

        /// <summary>
        /// Gets the color kind (Default, ANSI, Indexed, or RGB).
        /// </summary>
        public ColorKind Kind => _kind;

        /// <summary>
        /// Gets the red component (valid for RGB colors).
        /// </summary>
        public byte R => _red;

        /// <summary>
        /// Gets the green component (valid for RGB colors).
        /// </summary>
        public byte G => _green;

        /// <summary>
        /// Gets the blue component (valid for RGB colors).
        /// </summary>
        public byte B => _blue;

        /// <summary>
        /// Gets the color index (valid for ANSI and Indexed colors).
        /// </summary>
        public byte Index => _index;

        /// <inheritdoc />
        public bool Equals(Color other) =>
            _kind == other._kind &&
            _red == other._red &&
            _green == other._green &&
            _blue == other._blue &&
            _index == other._index;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Color other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(_kind, _red, _green, _blue, _index);
#else
            HashCode.Combine(_kind, _red, _green, _blue, _index);
#endif

        /// <inheritdoc />
        public override string ToString() => _kind switch
        {
            ColorKind.Default => "Default",
            ColorKind.Ansi => $"Ansi({_index})",
            ColorKind.Indexed => $"Indexed({_index})",
            ColorKind.Rgb => $"Rgb({_red}, {_green}, {_blue})",
            _ => "Unknown"
        };

        /// <summary>
        /// Determines whether two colors are equal.
        /// </summary>
        public static bool operator ==(Color left, Color right) => left.Equals(right);

        /// <summary>
        /// Determines whether two colors are not equal.
        /// </summary>
        public static bool operator !=(Color left, Color right) => !(left == right);
    }

    /// <summary>
    /// Specifies the type of color representation.
    /// </summary>
    public enum ColorKind : byte
    {
        /// <summary>
        /// Default terminal color.
        /// </summary>
        Default,

        /// <summary>
        /// 16-color ANSI color (0-15).
        /// </summary>
        Ansi,

        /// <summary>
        /// 256-color indexed color (0-255).
        /// </summary>
        Indexed,

        /// <summary>
        /// 24-bit RGB color.
        /// </summary>
        Rgb
    }
}