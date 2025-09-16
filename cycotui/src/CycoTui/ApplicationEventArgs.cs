using System;

namespace CycoAI.CycoTui
{
    /// <summary>
    /// Event arguments for application started events.
    /// </summary>
    public class ApplicationStartedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the timestamp when the application started.
        /// </summary>
        public DateTime StartTime { get; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event arguments for application stopped events.
    /// </summary>
    public class ApplicationStoppedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the timestamp when the application stopped.
        /// </summary>
        public DateTime StopTime { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the reason for stopping, if any.
        /// </summary>
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Event arguments for render frame events.
    /// </summary>
    public class RenderFrameEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the timestamp when the frame was rendered.
        /// </summary>
        public DateTime FrameTime { get; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the frame number (incremental counter).
        /// </summary>
        public long FrameNumber { get; set; }

        /// <summary>
        /// Gets the time taken to render the frame.
        /// </summary>
        public TimeSpan RenderDuration { get; set; }
    }

    /// <summary>
    /// Event arguments for unhandled exception events.
    /// </summary>
    public class UnhandledExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The unhandled exception.</param>
        /// <param name="isTerminating">Whether the application is terminating due to this exception.</param>
        public UnhandledExceptionEventArgs(Exception exception, bool isTerminating)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            IsTerminating = isTerminating;
        }

        /// <summary>
        /// Gets the unhandled exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets a value indicating whether the application is terminating due to this exception.
        /// </summary>
        public bool IsTerminating { get; }

        /// <summary>
        /// Gets the timestamp when the exception occurred.
        /// </summary>
        public DateTime ExceptionTime { get; } = DateTime.UtcNow;
    }
}