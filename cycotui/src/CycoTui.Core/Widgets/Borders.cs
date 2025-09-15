using System;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Widgets
{
    /// <summary>
    /// Specifies which borders should be rendered for a widget.
    /// </summary>
    [Flags]
    public enum Borders
    {
        /// <summary>
        /// No borders.
        /// </summary>
        None = 0,

        /// <summary>
        /// Top border.
        /// </summary>
        Top = 1,

        /// <summary>
        /// Right border.
        /// </summary>
        Right = 2,

        /// <summary>
        /// Bottom border.
        /// </summary>
        Bottom = 4,

        /// <summary>
        /// Left border.
        /// </summary>
        Left = 8,

        /// <summary>
        /// All borders.
        /// </summary>
        All = Top | Right | Bottom | Left
    }

    /// <summary>
    /// Defines the style and characters used for rendering borders.
    /// </summary>
    public readonly struct BorderType : IEquatable<BorderType>
    {
        /// <summary>
        /// Gets the character used for the top-left corner.
        /// </summary>
        public char TopLeft { get; }

        /// <summary>
        /// Gets the character used for the top-right corner.
        /// </summary>
        public char TopRight { get; }

        /// <summary>
        /// Gets the character used for the bottom-left corner.
        /// </summary>
        public char BottomLeft { get; }

        /// <summary>
        /// Gets the character used for the bottom-right corner.
        /// </summary>
        public char BottomRight { get; }

        /// <summary>
        /// Gets the character used for horizontal lines.
        /// </summary>
        public char Horizontal { get; }

        /// <summary>
        /// Gets the character used for vertical lines.
        /// </summary>
        public char Vertical { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderType"/> struct.
        /// </summary>
        /// <param name="topLeft">The top-left corner character.</param>
        /// <param name="topRight">The top-right corner character.</param>
        /// <param name="bottomLeft">The bottom-left corner character.</param>
        /// <param name="bottomRight">The bottom-right corner character.</param>
        /// <param name="horizontal">The horizontal line character.</param>
        /// <param name="vertical">The vertical line character.</param>
        public BorderType(char topLeft, char topRight, char bottomLeft, char bottomRight, char horizontal, char vertical)
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            Horizontal = horizontal;
            Vertical = vertical;
        }

        /// <summary>
        /// Gets a plain border type using ASCII characters.
        /// </summary>
        public static BorderType Plain => new BorderType('+', '+', '+', '+', '-', '|');

        /// <summary>
        /// Gets a rounded border type using Unicode box-drawing characters.
        /// </summary>
        public static BorderType Rounded => new BorderType('╭', '╮', '╰', '╯', '─', '│');

        /// <summary>
        /// Gets a double border type using Unicode box-drawing characters.
        /// </summary>
        public static BorderType Double => new BorderType('╔', '╗', '╚', '╝', '═', '║');

        /// <summary>
        /// Gets a thick border type using Unicode box-drawing characters.
        /// </summary>
        public static BorderType Thick => new BorderType('┏', '┓', '┗', '┛', '━', '┃');

        /// <summary>
        /// Gets a solid border type using Unicode block characters.
        /// </summary>
        public static BorderType Solid => new BorderType('█', '█', '█', '█', '█', '█');

        /// <inheritdoc />
        public bool Equals(BorderType other) =>
            TopLeft == other.TopLeft &&
            TopRight == other.TopRight &&
            BottomLeft == other.BottomLeft &&
            BottomRight == other.BottomRight &&
            Horizontal == other.Horizontal &&
            Vertical == other.Vertical;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is BorderType other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(TopLeft, TopRight, BottomLeft, BottomRight, Horizontal, Vertical);
#else
            HashCode.Combine(TopLeft, TopRight, BottomLeft, BottomRight, Horizontal, Vertical);
#endif

        /// <inheritdoc />
        public override string ToString() =>
            $"BorderType(TL='{TopLeft}', TR='{TopRight}', BL='{BottomLeft}', BR='{BottomRight}', H='{Horizontal}', V='{Vertical}')";

        /// <summary>
        /// Determines whether two border types are equal.
        /// </summary>
        public static bool operator ==(BorderType left, BorderType right) => left.Equals(right);

        /// <summary>
        /// Determines whether two border types are not equal.
        /// </summary>
        public static bool operator !=(BorderType left, BorderType right) => !(left == right);
    }
}