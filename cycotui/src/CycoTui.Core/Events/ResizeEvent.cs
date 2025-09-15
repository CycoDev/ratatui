using System;

namespace CycoAI.CycoTui.Core.Events
{
    /// <summary>
    /// Represents a terminal resize event.
    /// </summary>
    public class ResizeEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeEvent"/> class.
        /// </summary>
        /// <param name="width">The new width of the terminal.</param>
        /// <param name="height">The new height of the terminal.</param>
        public ResizeEvent(int width, int height)
            : base(EventType.Resize)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the new width of the terminal.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the new height of the terminal.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Gets the new size as a tuple.
        /// </summary>
        public (int Width, int Height) Size => (Width, Height);

        /// <inheritdoc />
        public override string ToString()
        {
            return $"ResizeEvent: {Width}x{Height}";
        }
    }

    /// <summary>
    /// Represents a focus change event.
    /// </summary>
    public class FocusEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FocusEvent"/> class.
        /// </summary>
        /// <param name="gained">Whether focus was gained (true) or lost (false).</param>
        public FocusEvent(bool gained)
            : base(EventType.Focus)
        {
            Gained = gained;
        }

        /// <summary>
        /// Gets a value indicating whether focus was gained (true) or lost (false).
        /// </summary>
        public bool Gained { get; }

        /// <summary>
        /// Gets a value indicating whether focus was lost.
        /// </summary>
        public bool Lost => !Gained;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"FocusEvent: {(Gained ? "Gained" : "Lost")}";
        }
    }

    /// <summary>
    /// Represents a paste event containing clipboard content.
    /// </summary>
    public class PasteEvent : BaseEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasteEvent"/> class.
        /// </summary>
        /// <param name="content">The pasted content.</param>
        public PasteEvent(string content)
            : base(EventType.Paste)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        /// <summary>
        /// Gets the pasted content.
        /// </summary>
        public string Content { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var preview = Content.Length > 50 ? Content.Substring(0, 47) + "..." : Content;
            return $"PasteEvent: \"{preview}\"";
        }
    }
}