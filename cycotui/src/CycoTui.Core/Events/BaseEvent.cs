using System;

namespace CycoAI.CycoTui.Core.Events
{
    /// <summary>
    /// Base implementation of <see cref="IEvent"/> providing common functionality.
    /// </summary>
    public abstract class BaseEvent : IEvent
    {
        private bool _isHandled;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseEvent"/> class.
        /// </summary>
        /// <param name="type">The type of event.</param>
        protected BaseEvent(EventType type)
        {
            Type = type;
            Timestamp = DateTime.UtcNow;
        }

        /// <inheritdoc />
        public DateTime Timestamp { get; }

        /// <inheritdoc />
        public EventType Type { get; }

        /// <inheritdoc />
        public bool IsHandled => _isHandled;

        /// <inheritdoc />
        public void Handle()
        {
            _isHandled = true;
        }
    }
}