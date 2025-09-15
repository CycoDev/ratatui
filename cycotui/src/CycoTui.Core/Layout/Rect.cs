using System;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Layout
{
    /// <summary>
    /// Represents a rectangular area in terminal coordinates.
    /// Coordinates are 0-based with (0,0) at the top-left corner.
    /// </summary>
    public readonly struct Rect : IEquatable<Rect>
    {
        /// <summary>
        /// Gets the x-coordinate of the left edge.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the y-coordinate of the top edge.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the width of the rectangle.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of the rectangle.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the x-coordinate of the right edge (exclusive).
        /// </summary>
        public int Right => X + Width;

        /// <summary>
        /// Gets the y-coordinate of the bottom edge (exclusive).
        /// </summary>
        public int Bottom => Y + Height;

        /// <summary>
        /// Gets the area of the rectangle (width * height).
        /// </summary>
        public int Area => Width * Height;

        /// <summary>
        /// Gets a value indicating whether this rectangle is empty (zero width or height).
        /// </summary>
        public bool IsEmpty => Width <= 0 || Height <= 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Rect"/> struct.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = Math.Max(0, width);
            Height = Math.Max(0, height);
        }

        /// <summary>
        /// Creates a rectangle from coordinates and size.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>A new rectangle.</returns>
        public static Rect FromCoords(int x, int y, int width, int height) => new Rect(x, y, width, height);

        /// <summary>
        /// Creates a rectangle from size only, positioned at origin.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>A new rectangle at (0,0).</returns>
        public static Rect FromSize(int width, int height) => new Rect(0, 0, width, height);

        /// <summary>
        /// Determines whether this rectangle contains the specified point.
        /// </summary>
        /// <param name="x">The x-coordinate to test.</param>
        /// <param name="y">The y-coordinate to test.</param>
        /// <returns>true if the point is inside this rectangle; otherwise, false.</returns>
        public bool Contains(int x, int y) => x >= X && x < Right && y >= Y && y < Bottom;

        /// <summary>
        /// Determines whether this rectangle contains the specified rectangle.
        /// </summary>
        /// <param name="other">The rectangle to test.</param>
        /// <returns>true if the other rectangle is entirely within this rectangle; otherwise, false.</returns>
        public bool Contains(Rect other) =>
            other.X >= X && other.Y >= Y && other.Right <= Right && other.Bottom <= Bottom;

        /// <summary>
        /// Determines whether this rectangle intersects with another rectangle.
        /// </summary>
        /// <param name="other">The rectangle to test for intersection.</param>
        /// <returns>true if the rectangles intersect; otherwise, false.</returns>
        public bool Intersects(Rect other) =>
            X < other.Right && Right > other.X && Y < other.Bottom && Bottom > other.Y;

        /// <summary>
        /// Returns the intersection of this rectangle with another rectangle.
        /// </summary>
        /// <param name="other">The rectangle to intersect with.</param>
        /// <returns>The intersection rectangle, or an empty rectangle if no intersection exists.</returns>
        public Rect Intersect(Rect other)
        {
            var left = Math.Max(X, other.X);
            var top = Math.Max(Y, other.Y);
            var right = Math.Min(Right, other.Right);
            var bottom = Math.Min(Bottom, other.Bottom);

            if (left >= right || top >= bottom)
                return new Rect(0, 0, 0, 0);

            return new Rect(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Returns a new rectangle with the specified margins applied.
        /// </summary>
        /// <param name="margin">The margin to apply to all sides.</param>
        /// <returns>A new rectangle with margins applied.</returns>
        public Rect WithMargin(int margin) => WithMargin(margin, margin, margin, margin);

        /// <summary>
        /// Returns a new rectangle with the specified margins applied.
        /// </summary>
        /// <param name="left">The left margin.</param>
        /// <param name="top">The top margin.</param>
        /// <param name="right">The right margin.</param>
        /// <param name="bottom">The bottom margin.</param>
        /// <returns>A new rectangle with margins applied.</returns>
        public Rect WithMargin(int left, int top, int right, int bottom)
        {
            var newX = X + left;
            var newY = Y + top;
            var newWidth = Math.Max(0, Width - left - right);
            var newHeight = Math.Max(0, Height - top - bottom);
            return new Rect(newX, newY, newWidth, newHeight);
        }

        /// <summary>
        /// Returns a new rectangle with the specified margin applied.
        /// </summary>
        /// <param name="margin">The margin to apply.</param>
        /// <returns>A new rectangle with margin applied.</returns>
        public Rect WithMargin(Margin margin) => WithMargin(margin.Left, margin.Top, margin.Right, margin.Bottom);

        /// <inheritdoc />
        public bool Equals(Rect other) => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Rect other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(X, Y, Width, Height);
#else
            HashCode.Combine(X, Y, Width, Height);
#endif

        /// <inheritdoc />
        public override string ToString() => $"Rect(x={X}, y={Y}, width={Width}, height={Height})";

        /// <summary>
        /// Determines whether two rectangles are equal.
        /// </summary>
        public static bool operator ==(Rect left, Rect right) => left.Equals(right);

        /// <summary>
        /// Determines whether two rectangles are not equal.
        /// </summary>
        public static bool operator !=(Rect left, Rect right) => !(left == right);
    }
}