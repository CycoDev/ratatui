using System;
using System.Threading;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Events;

namespace CycoAI.CycoTui.Core.Input
{
    /// <summary>
    /// Interface for platform-specific input handling.
    /// </summary>
    public interface IInputHandler : IDisposable
    {
        /// <summary>
        /// Starts the input handler and begins processing input events.
        /// </summary>
        /// <param name="eventQueue">The event queue to send events to.</param>
        /// <param name="cancellationToken">Token to cancel the input processing.</param>
        /// <returns>A task representing the input processing operation.</returns>
        Task StartAsync(IEventQueue eventQueue, CancellationToken cancellationToken = default);

        /// <summary>
        /// Stops the input handler and releases resources.
        /// </summary>
        void Stop();

        /// <summary>
        /// Polls for a single input event synchronously.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for an event.</param>
        /// <returns>The next input event, or null if timeout occurred.</returns>
        IEvent? PollEvent(TimeSpan timeout);

        /// <summary>
        /// Reads a single input event asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the read operation.</param>
        /// <returns>The next input event.</returns>
        Task<IEvent> ReadEventAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a value indicating whether the input handler is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets a value indicating whether mouse events are supported.
        /// </summary>
        bool SupportsMouseEvents { get; }

        /// <summary>
        /// Gets a value indicating whether focus events are supported.
        /// </summary>
        bool SupportsFocusEvents { get; }

        /// <summary>
        /// Gets a value indicating whether paste events are supported.
        /// </summary>
        bool SupportsPasteEvents { get; }

        /// <summary>
        /// Enables or disables mouse event capture.
        /// </summary>
        /// <param name="enable">Whether to enable mouse events.</param>
        void SetMouseEnabled(bool enable);

        /// <summary>
        /// Enables or disables focus event capture.
        /// </summary>
        /// <param name="enable">Whether to enable focus events.</param>
        void SetFocusEnabled(bool enable);

        /// <summary>
        /// Enables or disables paste event capture.
        /// </summary>
        /// <param name="enable">Whether to enable paste events.</param>
        void SetPasteEnabled(bool enable);

        /// <summary>
        /// Event raised when an input error occurs.
        /// </summary>
        event System.EventHandler<InputErrorEventArgs>? InputError;
    }

    /// <summary>
    /// Event arguments for input errors.
    /// </summary>
    public class InputErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputErrorEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="context">Additional context about the error.</param>
        public InputErrorEventArgs(Exception exception, string? context = null)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            Context = context;
        }

        /// <summary>
        /// Gets the exception that occurred.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets additional context about the error.
        /// </summary>
        public string? Context { get; }
    }

    /// <summary>
    /// Interface for input configuration.
    /// </summary>
    public interface IInputConfiguration
    {
        /// <summary>
        /// Gets or sets whether mouse events should be captured.
        /// </summary>
        bool MouseEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether focus events should be captured.
        /// </summary>
        bool FocusEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether paste events should be captured.
        /// </summary>
        bool PasteEnabled { get; set; }

        /// <summary>
        /// Gets or sets the input buffer size.
        /// </summary>
        int BufferSize { get; set; }

        /// <summary>
        /// Gets or sets the polling interval for input events.
        /// </summary>
        TimeSpan PollingInterval { get; set; }
    }

    /// <summary>
    /// Default implementation of input configuration.
    /// </summary>
    public class InputConfiguration : IInputConfiguration
    {
        /// <inheritdoc />
        public bool MouseEnabled { get; set; } = true;

        /// <inheritdoc />
        public bool FocusEnabled { get; set; } = true;

        /// <inheritdoc />
        public bool PasteEnabled { get; set; } = true;

        /// <inheritdoc />
        public int BufferSize { get; set; } = 1024;

        /// <inheritdoc />
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMilliseconds(16); // ~60 FPS
    }
}