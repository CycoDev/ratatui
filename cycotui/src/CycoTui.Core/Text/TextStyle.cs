using System;
using CycoAI.CycoTui.Core.Style;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Text
{
    /// <summary>
    /// Represents the styling information for text, including colors and modifiers.
    /// </summary>
    public readonly struct TextStyle : IEquatable<TextStyle>
    {
        /// <summary>
        /// Gets the foreground color.
        /// </summary>
        public Color Foreground { get; }

        /// <summary>
        /// Gets the background color.
        /// </summary>
        public Color Background { get; }

        /// <summary>
        /// Gets the text modifiers.
        /// </summary>
        public Modifier Modifiers { get; }

        /// <summary>
        /// Gets the default text style with no special formatting.
        /// </summary>
        public static TextStyle Default => new TextStyle(Color.Default, Color.Default, Modifier.None);

        /// <summary>
        /// Initializes a new instance of the <see cref="TextStyle"/> struct.
        /// </summary>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="modifiers">The text modifiers.</param>
        public TextStyle(Color foreground, Color background, Modifier modifiers)
        {
            Foreground = foreground;
            Background = background;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Creates a new style with the specified foreground color.
        /// </summary>
        /// <param name="color">The foreground color.</param>
        /// <returns>A new style with the updated foreground color.</returns>
        public TextStyle WithForeground(Color color) => new TextStyle(color, Background, Modifiers);

        /// <summary>
        /// Creates a new style with the specified background color.
        /// </summary>
        /// <param name="color">The background color.</param>
        /// <returns>A new style with the updated background color.</returns>
        public TextStyle WithBackground(Color color) => new TextStyle(Foreground, color, Modifiers);

        /// <summary>
        /// Creates a new style with the specified modifiers.
        /// </summary>
        /// <param name="modifiers">The modifiers to apply.</param>
        /// <returns>A new style with the updated modifiers.</returns>
        public TextStyle WithModifiers(Modifier modifiers) => new TextStyle(Foreground, Background, modifiers);

        /// <summary>
        /// Creates a new style with additional modifiers added.
        /// </summary>
        /// <param name="modifiers">The modifiers to add.</param>
        /// <returns>A new style with the additional modifiers.</returns>
        public TextStyle AddModifiers(Modifier modifiers) => new TextStyle(Foreground, Background, Modifiers | modifiers);

        /// <summary>
        /// Creates a new style with the specified modifiers removed.
        /// </summary>
        /// <param name="modifiers">The modifiers to remove.</param>
        /// <returns>A new style with the modifiers removed.</returns>
        public TextStyle RemoveModifiers(Modifier modifiers) => new TextStyle(Foreground, Background, Modifiers & ~modifiers);

        /// <summary>
        /// Merges this style with another style, with the other style taking precedence for non-default values.
        /// </summary>
        /// <param name="other">The style to merge with.</param>
        /// <returns>A new style with merged properties.</returns>
        public TextStyle Merge(TextStyle other)
        {
            var foreground = other.Foreground.Kind != ColorKind.Default ? other.Foreground : Foreground;
            var background = other.Background.Kind != ColorKind.Default ? other.Background : Background;
            var modifiers = other.Modifiers != Modifier.None ? other.Modifiers : Modifiers;

            return new TextStyle(foreground, background, modifiers);
        }

        /// <inheritdoc />
        public bool Equals(TextStyle other) =>
            Foreground.Equals(other.Foreground) &&
            Background.Equals(other.Background) &&
            Modifiers == other.Modifiers;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is TextStyle other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(Foreground, Background, Modifiers);
#else
            HashCode.Combine(Foreground, Background, Modifiers);
#endif

        /// <inheritdoc />
        public override string ToString() => $"TextStyle(fg={Foreground}, bg={Background}, mod={Modifiers})";

        /// <summary>
        /// Determines whether two text styles are equal.
        /// </summary>
        public static bool operator ==(TextStyle left, TextStyle right) => left.Equals(right);

        /// <summary>
        /// Determines whether two text styles are not equal.
        /// </summary>
        public static bool operator !=(TextStyle left, TextStyle right) => !(left == right);
    }
}