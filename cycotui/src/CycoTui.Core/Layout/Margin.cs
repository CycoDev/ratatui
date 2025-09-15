using System;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Layout
{
    /// <summary>
    /// Represents margin spacing for layout elements.
    /// Margins are applied around the content area.
    /// </summary>
    public readonly struct Margin : IEquatable<Margin>
    {
        /// <summary>
        /// Gets the left margin.
        /// </summary>
        public int Left { get; }

        /// <summary>
        /// Gets the top margin.
        /// </summary>
        public int Top { get; }

        /// <summary>
        /// Gets the right margin.
        /// </summary>
        public int Right { get; }

        /// <summary>
        /// Gets the bottom margin.
        /// </summary>
        public int Bottom { get; }

        /// <summary>
        /// Gets a margin with no spacing.
        /// </summary>
        public static Margin None => new Margin(0);

        /// <summary>
        /// Gets the total horizontal margin (left + right).
        /// </summary>
        public int Horizontal => Left + Right;

        /// <summary>
        /// Gets the total vertical margin (top + bottom).
        /// </summary>
        public int Vertical => Top + Bottom;

        /// <summary>
        /// Initializes a new instance of the <see cref="Margin"/> struct with uniform spacing.
        /// </summary>
        /// <param name="margin">The uniform margin to apply to all sides.</param>
        public Margin(int margin)
        {
            if (margin < 0)
                throw new ArgumentOutOfRangeException(nameof(margin), "Margin cannot be negative.");

            Left = Top = Right = Bottom = margin;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Margin"/> struct with horizontal and vertical spacing.
        /// </summary>
        /// <param name="horizontal">The horizontal margin (left and right).</param>
        /// <param name="vertical">The vertical margin (top and bottom).</param>
        public Margin(int horizontal, int vertical)
        {
            if (horizontal < 0)
                throw new ArgumentOutOfRangeException(nameof(horizontal), "Horizontal margin cannot be negative.");
            if (vertical < 0)
                throw new ArgumentOutOfRangeException(nameof(vertical), "Vertical margin cannot be negative.");

            Left = Right = horizontal;
            Top = Bottom = vertical;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Margin"/> struct with individual side spacing.
        /// </summary>
        /// <param name="left">The left margin.</param>
        /// <param name="top">The top margin.</param>
        /// <param name="right">The right margin.</param>
        /// <param name="bottom">The bottom margin.</param>
        public Margin(int left, int top, int right, int bottom)
        {
            if (left < 0)
                throw new ArgumentOutOfRangeException(nameof(left), "Left margin cannot be negative.");
            if (top < 0)
                throw new ArgumentOutOfRangeException(nameof(top), "Top margin cannot be negative.");
            if (right < 0)
                throw new ArgumentOutOfRangeException(nameof(right), "Right margin cannot be negative.");
            if (bottom < 0)
                throw new ArgumentOutOfRangeException(nameof(bottom), "Bottom margin cannot be negative.");

            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        /// <summary>
        /// Creates a margin with only left spacing.
        /// </summary>
        /// <param name="left">The left margin.</param>
        /// <returns>A margin with left spacing.</returns>
        public static Margin WithLeft(int left) => new Margin(left, 0, 0, 0);

        /// <summary>
        /// Creates a margin with only top spacing.
        /// </summary>
        /// <param name="top">The top margin.</param>
        /// <returns>A margin with top spacing.</returns>
        public static Margin WithTop(int top) => new Margin(0, top, 0, 0);

        /// <summary>
        /// Creates a margin with only right spacing.
        /// </summary>
        /// <param name="right">The right margin.</param>
        /// <returns>A margin with right spacing.</returns>
        public static Margin WithRight(int right) => new Margin(0, 0, right, 0);

        /// <summary>
        /// Creates a margin with only bottom spacing.
        /// </summary>
        /// <param name="bottom">The bottom margin.</param>
        /// <returns>A margin with bottom spacing.</returns>
        public static Margin WithBottom(int bottom) => new Margin(0, 0, 0, bottom);

        /// <summary>
        /// Creates a margin with horizontal spacing.
        /// </summary>
        /// <param name="horizontal">The horizontal margin (left and right).</param>
        /// <returns>A margin with horizontal spacing.</returns>
        public static Margin WithHorizontal(int horizontal) => new Margin(horizontal, 0);

        /// <summary>
        /// Creates a margin with vertical spacing.
        /// </summary>
        /// <param name="vertical">The vertical margin (top and bottom).</param>
        /// <returns>A margin with vertical spacing.</returns>
        public static Margin WithVertical(int vertical) => new Margin(0, vertical);

        /// <summary>
        /// Creates a new margin with the specified left value.
        /// </summary>
        /// <param name="left">The new left margin.</param>
        /// <returns>A new margin with the updated left value.</returns>
        public Margin SetLeft(int left) => new Margin(left, Top, Right, Bottom);

        /// <summary>
        /// Creates a new margin with the specified top value.
        /// </summary>
        /// <param name="top">The new top margin.</param>
        /// <returns>A new margin with the updated top value.</returns>
        public Margin SetTop(int top) => new Margin(Left, top, Right, Bottom);

        /// <summary>
        /// Creates a new margin with the specified right value.
        /// </summary>
        /// <param name="right">The new right margin.</param>
        /// <returns>A new margin with the updated right value.</returns>
        public Margin SetRight(int right) => new Margin(Left, Top, right, Bottom);

        /// <summary>
        /// Creates a new margin with the specified bottom value.
        /// </summary>
        /// <param name="bottom">The new bottom margin.</param>
        /// <returns>A new margin with the updated bottom value.</returns>
        public Margin SetBottom(int bottom) => new Margin(Left, Top, Right, bottom);

        /// <summary>
        /// Adds margin values to create a new margin.
        /// </summary>
        /// <param name="other">The margin to add.</param>
        /// <returns>A new margin with added values.</returns>
        public Margin Add(Margin other) => new Margin(
            Left + other.Left,
            Top + other.Top,
            Right + other.Right,
            Bottom + other.Bottom);

        /// <summary>
        /// Subtracts margin values to create a new margin.
        /// </summary>
        /// <param name="other">The margin to subtract.</param>
        /// <returns>A new margin with subtracted values.</returns>
        public Margin Subtract(Margin other) => new Margin(
            Math.Max(0, Left - other.Left),
            Math.Max(0, Top - other.Top),
            Math.Max(0, Right - other.Right),
            Math.Max(0, Bottom - other.Bottom));

        /// <inheritdoc />
        public bool Equals(Margin other) =>
            Left == other.Left &&
            Top == other.Top &&
            Right == other.Right &&
            Bottom == other.Bottom;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Margin other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(Left, Top, Right, Bottom);
#else
            HashCode.Combine(Left, Top, Right, Bottom);
#endif

        /// <inheritdoc />
        public override string ToString() => $"Margin(left={Left}, top={Top}, right={Right}, bottom={Bottom})";

        /// <summary>
        /// Determines whether two margins are equal.
        /// </summary>
        public static bool operator ==(Margin left, Margin right) => left.Equals(right);

        /// <summary>
        /// Determines whether two margins are not equal.
        /// </summary>
        public static bool operator !=(Margin left, Margin right) => !(left == right);

        /// <summary>
        /// Adds two margins together.
        /// </summary>
        public static Margin operator +(Margin left, Margin right) => left.Add(right);

        /// <summary>
        /// Subtracts one margin from another.
        /// </summary>
        public static Margin operator -(Margin left, Margin right) => left.Subtract(right);
    }
}