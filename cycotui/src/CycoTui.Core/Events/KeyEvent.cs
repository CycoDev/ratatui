using System;

namespace CycoAI.CycoTui.Core.Events
{
    /// <summary>
    /// Represents a keyboard input event.
    /// </summary>
    public class KeyEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEvent"/> class.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        /// <param name="modifiers">The modifier keys that were active.</param>
        public KeyEvent(Key key, KeyModifiers modifiers = KeyModifiers.None)
            : base(EventType.Key)
        {
            Key = key;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Gets the key that was pressed.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        /// Gets the modifier keys that were active when the key was pressed.
        /// </summary>
        public KeyModifiers Modifiers { get; }

        /// <summary>
        /// Gets the character representation of the key, if applicable.
        /// </summary>
        public char? Character => GetCharacter();

        /// <summary>
        /// Gets a value indicating whether this is a printable character.
        /// </summary>
        public bool IsPrintable => Character.HasValue && !char.IsControl(Character.Value);

        private char? GetCharacter()
        {
            // Handle basic alphanumeric keys
            if (Key >= Key.A && Key <= Key.Z)
            {
                var baseChar = (char)('a' + ((int)Key - (int)Key.A));
                return Modifiers.HasFlag(KeyModifiers.Shift) ? char.ToUpper(baseChar) : baseChar;
            }

            if (Key >= Key.D0 && Key <= Key.D9)
            {
                if (Modifiers.HasFlag(KeyModifiers.Shift))
                {
                    // Number row shift characters
                    return Key switch
                    {
                        Key.D1 => '!',
                        Key.D2 => '@',
                        Key.D3 => '#',
                        Key.D4 => '$',
                        Key.D5 => '%',
                        Key.D6 => '^',
                        Key.D7 => '&',
                        Key.D8 => '*',
                        Key.D9 => '(',
                        Key.D0 => ')',
                        _ => null
                    };
                }
                return (char)('0' + ((int)Key - (int)Key.D0));
            }

            // Handle special characters
            return Key switch
            {
                Key.Space => ' ',
                Key.Tab => '\t',
                Key.Enter => '\n',
                Key.Period => Modifiers.HasFlag(KeyModifiers.Shift) ? '>' : '.',
                Key.Comma => Modifiers.HasFlag(KeyModifiers.Shift) ? '<' : ',',
                Key.Semicolon => Modifiers.HasFlag(KeyModifiers.Shift) ? ':' : ';',
                Key.Quote => Modifiers.HasFlag(KeyModifiers.Shift) ? '"' : '\'',
                Key.LeftBracket => Modifiers.HasFlag(KeyModifiers.Shift) ? '{' : '[',
                Key.RightBracket => Modifiers.HasFlag(KeyModifiers.Shift) ? '}' : ']',
                Key.Backslash => Modifiers.HasFlag(KeyModifiers.Shift) ? '|' : '\\',
                Key.Slash => Modifiers.HasFlag(KeyModifiers.Shift) ? '?' : '/',
                Key.Minus => Modifiers.HasFlag(KeyModifiers.Shift) ? '_' : '-',
                Key.Equal => Modifiers.HasFlag(KeyModifiers.Shift) ? '+' : '=',
                Key.Grave => Modifiers.HasFlag(KeyModifiers.Shift) ? '~' : '`',
                _ => null
            };
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var modStr = Modifiers != KeyModifiers.None ? $"{Modifiers}+" : "";
            var charStr = Character.HasValue ? $" ('{Character}')" : "";
            return $"KeyEvent: {modStr}{Key}{charStr}";
        }
    }

    /// <summary>
    /// Enumeration of keyboard keys.
    /// </summary>
    public enum Key
    {
        // Alphanumeric keys
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        D0, D1, D2, D3, D4, D5, D6, D7, D8, D9,

        // Function keys
        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,

        // Navigation keys
        Up, Down, Left, Right,
        Home, End, PageUp, PageDown,

        // Special keys
        Enter, Escape, Tab, Space, Backspace, Delete, Insert,

        // Punctuation and symbols
        Period, Comma, Semicolon, Quote, LeftBracket, RightBracket,
        Backslash, Slash, Minus, Equal, Grave,

        // Modifier keys (when pressed alone)
        LeftShift, RightShift, LeftControl, RightControl,
        LeftAlt, RightAlt, LeftSuper, RightSuper,

        // Lock keys
        CapsLock, NumLock, ScrollLock,

        // Unknown or unsupported key
        Unknown
    }

    /// <summary>
    /// Enumeration of keyboard modifier keys.
    /// </summary>
    [Flags]
    public enum KeyModifiers
    {
        /// <summary>
        /// No modifier keys.
        /// </summary>
        None = 0,

        /// <summary>
        /// Shift key modifier.
        /// </summary>
        Shift = 1 << 0,

        /// <summary>
        /// Control key modifier.
        /// </summary>
        Control = 1 << 1,

        /// <summary>
        /// Alt key modifier.
        /// </summary>
        Alt = 1 << 2,

        /// <summary>
        /// Super/Windows/Cmd key modifier.
        /// </summary>
        Super = 1 << 3
    }
}