using System;
using CycoAI.CycoTui.Core.Style;
#if NETSTANDARD2_0 || NETSTANDARD2_1
using CycoAI.CycoTui.Core.Internal;
#endif

namespace CycoAI.CycoTui.Core.Buffer
{
    /// <summary>
    /// Represents a single character cell in the terminal buffer.
    /// Contains the character, foreground color, background color, and text modifiers.
    /// </summary>
    public readonly struct Cell : IEquatable<Cell>
    {
        /// <summary>
        /// Gets the character displayed in this cell.
        /// </summary>
        public char Character { get; }

        /// <summary>
        /// Gets the foreground color of this cell.
        /// </summary>
        public Color Foreground { get; }

        /// <summary>
        /// Gets the background color of this cell.
        /// </summary>
        public Color Background { get; }

        /// <summary>
        /// Gets the text modifiers applied to this cell.
        /// </summary>
        public Modifier Modifiers { get; }

        /// <summary>
        /// Gets an empty cell with default styling.
        /// </summary>
        public static Cell Empty => new Cell(' ', Color.Default, Color.Default, Modifier.None);

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> struct.
        /// </summary>
        /// <param name="character">The character to display.</param>
        public Cell(char character) : this(character, Color.Default, Color.Default, Modifier.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> struct.
        /// </summary>
        /// <param name="character">The character to display.</param>
        /// <param name="foreground">The foreground color.</param>
        /// <param name="background">The background color.</param>
        /// <param name="modifiers">The text modifiers.</param>
        public Cell(char character, Color foreground, Color background, Modifier modifiers)
        {
            Character = character;
            Foreground = foreground;
            Background = background;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Creates a new cell with the specified character, keeping the current styling.
        /// </summary>
        /// <param name="character">The new character.</param>
        /// <returns>A new cell with the updated character.</returns>
        public Cell WithCharacter(char character) => new Cell(character, Foreground, Background, Modifiers);

        /// <summary>
        /// Creates a new cell with the specified foreground color.
        /// </summary>
        /// <param name="color">The new foreground color.</param>
        /// <returns>A new cell with the updated foreground color.</returns>
        public Cell WithForeground(Color color) => new Cell(Character, color, Background, Modifiers);

        /// <summary>
        /// Creates a new cell with the specified background color.
        /// </summary>
        /// <param name="color">The new background color.</param>
        /// <returns>A new cell with the updated background color.</returns>
        public Cell WithBackground(Color color) => new Cell(Character, Foreground, color, Modifiers);

        /// <summary>
        /// Creates a new cell with the specified modifiers.
        /// </summary>
        /// <param name="modifiers">The new modifiers.</param>
        /// <returns>A new cell with the updated modifiers.</returns>
        public Cell WithModifiers(Modifier modifiers) => new Cell(Character, Foreground, Background, modifiers);

        /// <summary>
        /// Creates a new cell with additional modifiers added.
        /// </summary>
        /// <param name="modifiers">The modifiers to add.</param>
        /// <returns>A new cell with the additional modifiers.</returns>
        public Cell AddModifiers(Modifier modifiers) => new Cell(Character, Foreground, Background, Modifiers | modifiers);

        /// <summary>
        /// Creates a new cell with the specified modifiers removed.
        /// </summary>
        /// <param name="modifiers">The modifiers to remove.</param>
        /// <returns>A new cell with the modifiers removed.</returns>
        public Cell RemoveModifiers(Modifier modifiers) => new Cell(Character, Foreground, Background, Modifiers & ~modifiers);

        /// <summary>
        /// Determines whether this cell is visually identical to another cell.
        /// This compares character, colors, and modifiers.
        /// </summary>
        /// <param name="other">The cell to compare with.</param>
        /// <returns>true if the cells are visually identical; otherwise, false.</returns>
        public bool Equals(Cell other) =>
            Character == other.Character &&
            Foreground.Equals(other.Foreground) &&
            Background.Equals(other.Background) &&
            Modifiers == other.Modifiers;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Cell other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() =>
#if NETSTANDARD2_0 || NETSTANDARD2_1
            Compat.CombineHashCodes(Character, Foreground, Background, Modifiers);
#else
            HashCode.Combine(Character, Foreground, Background, Modifiers);
#endif

        /// <inheritdoc />
        public override string ToString() => $"Cell('{Character}', fg={Foreground}, bg={Background}, mod={Modifiers})";

        /// <summary>
        /// Determines whether two cells are equal.
        /// </summary>
        public static bool operator ==(Cell left, Cell right) => left.Equals(right);

        /// <summary>
        /// Determines whether two cells are not equal.
        /// </summary>
        public static bool operator !=(Cell left, Cell right) => !(left == right);

        /// <summary>
        /// Implicitly converts a character to a cell with default styling.
        /// </summary>
        /// <param name="character">The character to convert.</param>
        public static implicit operator Cell(char character) => new Cell(character);
    }
}