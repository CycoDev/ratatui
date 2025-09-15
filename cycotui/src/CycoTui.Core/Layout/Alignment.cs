using System;

namespace CycoAI.CycoTui.Core.Layout
{
    /// <summary>
    /// Specifies how content is aligned horizontally within its container.
    /// </summary>
    public enum HorizontalAlignment
    {
        /// <summary>
        /// Align content to the left edge.
        /// </summary>
        Left,

        /// <summary>
        /// Center content horizontally.
        /// </summary>
        Center,

        /// <summary>
        /// Align content to the right edge.
        /// </summary>
        Right
    }

    /// <summary>
    /// Specifies how content is aligned vertically within its container.
    /// </summary>
    public enum VerticalAlignment
    {
        /// <summary>
        /// Align content to the top edge.
        /// </summary>
        Top,

        /// <summary>
        /// Center content vertically.
        /// </summary>
        Center,

        /// <summary>
        /// Align content to the bottom edge.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Represents combined horizontal and vertical alignment for positioning content within a container.
    /// </summary>
    public readonly struct Alignment : IEquatable<Alignment>
    {
        /// <summary>
        /// Gets the horizontal alignment.
        /// </summary>
        public HorizontalAlignment Horizontal { get; }

        /// <summary>
        /// Gets the vertical alignment.
        /// </summary>
        public VerticalAlignment Vertical { get; }

        /// <summary>
        /// Gets the default alignment (top-left).
        /// </summary>
        public static Alignment Default => new Alignment(HorizontalAlignment.Left, VerticalAlignment.Top);

        /// <summary>
        /// Gets center alignment (center-center).
        /// </summary>
        public static Alignment Center => new Alignment(HorizontalAlignment.Center, VerticalAlignment.Center);

        /// <summary>
        /// Gets top-left alignment.
        /// </summary>
        public static Alignment TopLeft => new Alignment(HorizontalAlignment.Left, VerticalAlignment.Top);

        /// <summary>
        /// Gets top-center alignment.
        /// </summary>
        public static Alignment TopCenter => new Alignment(HorizontalAlignment.Center, VerticalAlignment.Top);

        /// <summary>
        /// Gets top-right alignment.
        /// </summary>
        public static Alignment TopRight => new Alignment(HorizontalAlignment.Right, VerticalAlignment.Top);

        /// <summary>
        /// Gets center-left alignment.
        /// </summary>
        public static Alignment CenterLeft => new Alignment(HorizontalAlignment.Left, VerticalAlignment.Center);

        /// <summary>
        /// Gets center-right alignment.
        /// </summary>
        public static Alignment CenterRight => new Alignment(HorizontalAlignment.Right, VerticalAlignment.Center);

        /// <summary>
        /// Gets bottom-left alignment.
        /// </summary>
        public static Alignment BottomLeft => new Alignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);

        /// <summary>
        /// Gets bottom-center alignment.
        /// </summary>
        public static Alignment BottomCenter => new Alignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);

        /// <summary>
        /// Gets bottom-right alignment.
        /// </summary>
        public static Alignment BottomRight => new Alignment(HorizontalAlignment.Right, VerticalAlignment.Bottom);

        /// <summary>
        /// Initializes a new instance of the <see cref="Alignment"/> struct.
        /// </summary>
        /// <param name="horizontal">The horizontal alignment.</param>
        /// <param name="vertical">The vertical alignment.</param>
        public Alignment(HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        /// <summary>
        /// Creates a new alignment with the specified horizontal alignment.
        /// </summary>
        /// <param name="horizontal">The horizontal alignment.</param>
        /// <returns>A new alignment with the updated horizontal alignment.</returns>
        public Alignment WithHorizontal(HorizontalAlignment horizontal) => new Alignment(horizontal, Vertical);

        /// <summary>
        /// Creates a new alignment with the specified vertical alignment.
        /// </summary>
        /// <param name="vertical">The vertical alignment.</param>
        /// <returns>A new alignment with the updated vertical alignment.</returns>
        public Alignment WithVertical(VerticalAlignment vertical) => new Alignment(Horizontal, vertical);

        /// <summary>
        /// Applies this alignment to position a rectangle within a container.
        /// </summary>
        /// <param name="container">The container rectangle.</param>
        /// <param name="contentSize">The size of the content to align.</param>
        /// <returns>The aligned rectangle positioned within the container.</returns>
        public Rect ApplyAlignment(Rect container, (int Width, int Height) contentSize)
        {
            var (contentWidth, contentHeight) = contentSize;

            // Ensure content doesn't exceed container
            contentWidth = Math.Min(contentWidth, container.Width);
            contentHeight = Math.Min(contentHeight, container.Height);

            var x = Horizontal switch
            {
                HorizontalAlignment.Left => container.X,
                HorizontalAlignment.Center => container.X + (container.Width - contentWidth) / 2,
                HorizontalAlignment.Right => container.X + container.Width - contentWidth,
                _ => container.X
            };

            var y = Vertical switch
            {
                VerticalAlignment.Top => container.Y,
                VerticalAlignment.Center => container.Y + (container.Height - contentHeight) / 2,
                VerticalAlignment.Bottom => container.Y + container.Height - contentHeight,
                _ => container.Y
            };

            return new Rect(x, y, contentWidth, contentHeight);
        }

        /// <summary>
        /// Applies this alignment to position a rectangle within a container.
        /// </summary>
        /// <param name="container">The container rectangle.</param>
        /// <param name="content">The content rectangle to align (only size is used).</param>
        /// <returns>The aligned rectangle positioned within the container.</returns>
        public Rect ApplyAlignment(Rect container, Rect content) => ApplyAlignment(container, (content.Width, content.Height));

        /// <inheritdoc />
        public bool Equals(Alignment other) => Horizontal == other.Horizontal && Vertical == other.Vertical;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Alignment other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => ((int)Horizontal << 2) | (int)Vertical;

        /// <inheritdoc />
        public override string ToString() => $"Alignment({Horizontal}, {Vertical})";

        /// <summary>
        /// Determines whether two alignments are equal.
        /// </summary>
        public static bool operator ==(Alignment left, Alignment right) => left.Equals(right);

        /// <summary>
        /// Determines whether two alignments are not equal.
        /// </summary>
        public static bool operator !=(Alignment left, Alignment right) => !(left == right);
    }
}