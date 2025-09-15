using System;

namespace CycoAI.CycoTui.Core.Events
{
    /// <summary>
    /// Represents a mouse input event.
    /// </summary>
    public class MouseEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseEvent"/> class.
        /// </summary>
        /// <param name="kind">The kind of mouse event.</param>
        /// <param name="button">The mouse button involved.</param>
        /// <param name="x">The x-coordinate of the mouse.</param>
        /// <param name="y">The y-coordinate of the mouse.</param>
        /// <param name="modifiers">The modifier keys that were active.</param>
        public MouseEvent(MouseEventKind kind, MouseButton button, int x, int y, KeyModifiers modifiers = KeyModifiers.None)
            : base(EventType.Mouse)
        {
            Kind = kind;
            Button = button;
            X = x;
            Y = y;
            Modifiers = modifiers;
        }

        /// <summary>
        /// Gets the kind of mouse event.
        /// </summary>
        public MouseEventKind Kind { get; }

        /// <summary>
        /// Gets the mouse button involved in the event.
        /// </summary>
        public MouseButton Button { get; }

        /// <summary>
        /// Gets the x-coordinate of the mouse position.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the y-coordinate of the mouse position.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets the modifier keys that were active during the mouse event.
        /// </summary>
        public KeyModifiers Modifiers { get; }

        /// <summary>
        /// Gets the mouse position as a tuple.
        /// </summary>
        public (int X, int Y) Position => (X, Y);

        /// <inheritdoc />
        public override string ToString()
        {
            var modStr = Modifiers != KeyModifiers.None ? $" [{Modifiers}]" : "";
            return $"MouseEvent: {Kind} {Button} at ({X}, {Y}){modStr}";
        }
    }

    /// <summary>
    /// Enumeration of mouse event types.
    /// </summary>
    public enum MouseEventKind
    {
        /// <summary>
        /// Mouse button was pressed down.
        /// </summary>
        Down,

        /// <summary>
        /// Mouse button was released.
        /// </summary>
        Up,

        /// <summary>
        /// Mouse was moved.
        /// </summary>
        Moved,

        /// <summary>
        /// Mouse wheel was scrolled.
        /// </summary>
        ScrollUp,

        /// <summary>
        /// Mouse wheel was scrolled down.
        /// </summary>
        ScrollDown,

        /// <summary>
        /// Mouse was dragged (moved while button pressed).
        /// </summary>
        Drag
    }

    /// <summary>
    /// Enumeration of mouse buttons.
    /// </summary>
    public enum MouseButton
    {
        /// <summary>
        /// No button or not applicable.
        /// </summary>
        None,

        /// <summary>
        /// Left mouse button.
        /// </summary>
        Left,

        /// <summary>
        /// Right mouse button.
        /// </summary>
        Right,

        /// <summary>
        /// Middle mouse button (wheel click).
        /// </summary>
        Middle,

        /// <summary>
        /// Additional mouse button 4.
        /// </summary>
        Button4,

        /// <summary>
        /// Additional mouse button 5.
        /// </summary>
        Button5
    }
}