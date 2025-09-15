using System;

namespace CycoAI.CycoTui.Core.Events
{
    /// <summary>
    /// Base interface for all events in the terminal UI system.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets the timestamp when the event occurred.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Gets the type of event.
        /// </summary>
        EventType Type { get; }

        /// <summary>
        /// Gets a value indicating whether this event has been handled.
        /// When an event is marked as handled, it typically stops propagating through the event system.
        /// </summary>
        bool IsHandled { get; }

        /// <summary>
        /// Marks this event as handled, preventing further propagation.
        /// </summary>
        void Handle();
    }

    /// <summary>
    /// Enumeration of event types supported by the terminal UI system.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Keyboard input event.
        /// </summary>
        Key,

        /// <summary>
        /// Mouse input event.
        /// </summary>
        Mouse,

        /// <summary>
        /// Terminal resize event.
        /// </summary>
        Resize,

        /// <summary>
        /// Focus change event.
        /// </summary>
        Focus,

        /// <summary>
        /// Paste event (clipboard content).
        /// </summary>
        Paste
    }
}