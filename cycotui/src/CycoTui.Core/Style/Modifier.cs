using System;

namespace CycoAI.CycoTui.Core.Style
{
    /// <summary>
    /// Represents text modifiers that can be applied to terminal text.
    /// These are flags that can be combined using bitwise operations.
    /// </summary>
    [Flags]
    public enum Modifier : byte
    {
        /// <summary>
        /// No modifiers applied.
        /// </summary>
        None = 0,

        /// <summary>
        /// Bold or increased intensity text.
        /// </summary>
        Bold = 1 << 0,

        /// <summary>
        /// Dimmed or decreased intensity text.
        /// </summary>
        Dim = 1 << 1,

        /// <summary>
        /// Italic text.
        /// </summary>
        Italic = 1 << 2,

        /// <summary>
        /// Underlined text.
        /// </summary>
        Underlined = 1 << 3,

        /// <summary>
        /// Slow blinking text (less than 150 blinks per minute).
        /// </summary>
        SlowBlink = 1 << 4,

        /// <summary>
        /// Rapid blinking text (150 or more blinks per minute).
        /// </summary>
        RapidBlink = 1 << 5,

        /// <summary>
        /// Reversed video - swaps foreground and background colors.
        /// </summary>
        Reversed = 1 << 6,

        /// <summary>
        /// Hidden or invisible text.
        /// </summary>
        Hidden = 1 << 7
    }

    /// <summary>
    /// Extension methods for working with text modifiers.
    /// </summary>
    public static class ModifierExtensions
    {
        /// <summary>
        /// Checks if the modifier includes bold formatting.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>true if bold is included; otherwise, false.</returns>
        public static bool IsBold(this Modifier modifier) => (modifier & Modifier.Bold) == Modifier.Bold;

        /// <summary>
        /// Checks if the modifier includes dim formatting.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>true if dim is included; otherwise, false.</returns>
        public static bool IsDim(this Modifier modifier) => (modifier & Modifier.Dim) == Modifier.Dim;

        /// <summary>
        /// Checks if the modifier includes italic formatting.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>true if italic is included; otherwise, false.</returns>
        public static bool IsItalic(this Modifier modifier) => (modifier & Modifier.Italic) == Modifier.Italic;

        /// <summary>
        /// Checks if the modifier includes underlined formatting.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>true if underlined is included; otherwise, false.</returns>
        public static bool IsUnderlined(this Modifier modifier) => (modifier & Modifier.Underlined) == Modifier.Underlined;

        /// <summary>
        /// Checks if the modifier includes slow blink formatting.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>true if slow blink is included; otherwise, false.</returns>
        public static bool IsSlowBlink(this Modifier modifier) => (modifier & Modifier.SlowBlink) == Modifier.SlowBlink;

        /// <summary>
        /// Checks if the modifier includes rapid blink formatting.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>true if rapid blink is included; otherwise, false.</returns>
        public static bool IsRapidBlink(this Modifier modifier) => (modifier & Modifier.RapidBlink) == Modifier.RapidBlink;

        /// <summary>
        /// Checks if the modifier includes reversed formatting.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>true if reversed is included; otherwise, false.</returns>
        public static bool IsReversed(this Modifier modifier) => (modifier & Modifier.Reversed) == Modifier.Reversed;

        /// <summary>
        /// Checks if the modifier includes hidden formatting.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>true if hidden is included; otherwise, false.</returns>
        public static bool IsHidden(this Modifier modifier) => (modifier & Modifier.Hidden) == Modifier.Hidden;
    }
}