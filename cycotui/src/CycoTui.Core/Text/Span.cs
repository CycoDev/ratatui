using System;
using CycoAI.CycoTui.Core.Style;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Text
{
    /// <summary>
    /// Represents a styled span of text with consistent formatting.
    /// </summary>
    public readonly struct Span : IEquatable<Span>
    {
        /// <summary>
        /// Gets the text content of this span.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the style applied to this span.
        /// </summary>
        public TextStyle Style { get; }

        /// <summary>
        /// Gets the length of the text content.
        /// </summary>
        public int Length => Content?.Length ?? 0;

        /// <summary>
        /// Gets an empty span with no content or styling.
        /// </summary>
        public static Span Empty => new Span(string.Empty, TextStyle.Default);

        /// <summary>
        /// Initializes a new instance of the <see cref="Span"/> struct.
        /// </summary>
        /// <param name="content">The text content.</param>
        public Span(string content) : this(content, TextStyle.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Span"/> struct.
        /// </summary>
        /// <param name="content">The text content.</param>
        /// <param name="style">The style to apply.</param>
        public Span(string content, TextStyle style)
        {
            Content = content ?? string.Empty;
            Style = style;
        }

        /// <summary>
        /// Creates a new span with the specified foreground color.
        /// </summary>
        /// <param name="color">The foreground color.</param>
        /// <returns>A new span with the updated style.</returns>
        public Span WithForeground(Color color) => new Span(Content, Style.WithForeground(color));

        /// <summary>
        /// Creates a new span with the specified background color.
        /// </summary>
        /// <param name="color">The background color.</param>
        /// <returns>A new span with the updated style.</returns>
        public Span WithBackground(Color color) => new Span(Content, Style.WithBackground(color));

        /// <summary>
        /// Creates a new span with the specified modifiers.
        /// </summary>
        /// <param name="modifiers">The modifiers to apply.</param>
        /// <returns>A new span with the updated style.</returns>
        public Span WithModifiers(Modifier modifiers) => new Span(Content, Style.WithModifiers(modifiers));

        /// <summary>
        /// Creates a new span with additional modifiers added.
        /// </summary>
        /// <param name="modifiers">The modifiers to add.</param>
        /// <returns>A new span with the additional modifiers.</returns>
        public Span AddModifiers(Modifier modifiers) => new Span(Content, Style.AddModifiers(modifiers));

        /// <summary>
        /// Creates a new span with bold formatting.
        /// </summary>
        /// <returns>A new span with bold formatting.</returns>
        public Span Bold() => AddModifiers(Modifier.Bold);

        /// <summary>
        /// Creates a new span with italic formatting.
        /// </summary>
        /// <returns>A new span with italic formatting.</returns>
        public Span Italic() => AddModifiers(Modifier.Italic);

        /// <summary>
        /// Creates a new span with underlined formatting.
        /// </summary>
        /// <returns>A new span with underlined formatting.</returns>
        public Span Underlined() => AddModifiers(Modifier.Underlined);

        /// <inheritdoc />
        public bool Equals(Span other) => Content == other.Content && Style.Equals(other.Style);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Span other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(Content, Style);
#else
            HashCode.Combine(Content, Style);
#endif

        /// <inheritdoc />
        public override string ToString() => Content;

        /// <summary>
        /// Determines whether two spans are equal.
        /// </summary>
        public static bool operator ==(Span left, Span right) => left.Equals(right);

        /// <summary>
        /// Determines whether two spans are not equal.
        /// </summary>
        public static bool operator !=(Span left, Span right) => !(left == right);

        /// <summary>
        /// Implicitly converts a string to a span with default styling.
        /// </summary>
        /// <param name="content">The text content.</param>
        public static implicit operator Span(string content) => new Span(content);
    }
}