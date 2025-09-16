using System;
using System.Threading;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Events;

namespace CycoAI.CycoTui.Plugins
{
    /// <summary>
    /// Plugin that handles terminal resize events and manages application layout updates.
    /// </summary>
    public class ResizeHandlerPlugin : ApplicationPlugin
    {
        private (int Width, int Height) _lastSize;
        private DateTime _lastResizeTime = DateTime.MinValue;
        private readonly TimeSpan _debounceInterval = TimeSpan.FromMilliseconds(100);

        /// <inheritdoc />
        public override string Name => "Resize Handler";

        /// <summary>
        /// Gets or sets the resize debounce interval to prevent excessive redraws during resize operations.
        /// </summary>
        public TimeSpan DebounceInterval { get; set; } = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// Gets or sets a value indicating whether to automatically save and restore window size.
        /// </summary>
        public bool SaveWindowSize { get; set; } = true;

        /// <inheritdoc />
        public override Task InitializeAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            // Store initial size
            if (TryGetCurrentSize(out var size))
            {
                _lastSize = size;
                application.SetGlobalState("TerminalSize", size);

                // Restore saved size if available
                if (SaveWindowSize && application.TryGetGlobalState<(int, int)>("SavedTerminalSize", out var savedSize))
                {
                    // In a real implementation, we might attempt to resize the terminal
                    // if the platform supports it
                }
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override async Task<bool> HandleEventAsync(IEvent @event, IApplication application, CancellationToken cancellationToken = default)
        {
            if (@event.Type == EventType.Resize && @event is ResizeEvent resizeEvent)
            {
                return await HandleResizeEvent(resizeEvent, application);
            }

            return false;
        }

        /// <inheritdoc />
        public override Task UpdateAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            // Check for size changes that might not have generated events
            if (TryGetCurrentSize(out var currentSize) && currentSize != _lastSize)
            {
                var resizeEvent = new ResizeEvent
                {
                    Width = currentSize.Width,
                    Height = currentSize.Height
                };

                // Handle the resize
                _ = Task.Run(async () => await HandleResizeEvent(resizeEvent, application));
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task ShutdownAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            // Save current size for next session
            if (SaveWindowSize && TryGetCurrentSize(out var size))
            {
                application.SetGlobalState("SavedTerminalSize", size);
            }

            return Task.CompletedTask;
        }

        private async Task<bool> HandleResizeEvent(ResizeEvent resizeEvent, IApplication application)
        {
            var newSize = (resizeEvent.Width, resizeEvent.Height);
            var now = DateTime.UtcNow;

            // Debounce rapid resize events
            if (now - _lastResizeTime < DebounceInterval)
            {
                // Schedule a delayed update
                _ = Task.Delay(DebounceInterval).ContinueWith(async _ =>
                {
                    if (TryGetCurrentSize(out var latestSize))
                    {
                        await ProcessResize(latestSize, application);
                    }
                });

                return true;
            }

            _lastResizeTime = now;
            await ProcessResize(newSize, application);
            return true;
        }

        private async Task ProcessResize((int Width, int Height) newSize, IApplication application)
        {
            var oldSize = _lastSize;
            _lastSize = newSize;

            // Update global state
            application.SetGlobalState("TerminalSize", newSize);
            application.SetGlobalState("PreviousTerminalSize", oldSize);

            // Calculate size change
            var widthChange = newSize.Width - oldSize.Width;
            var heightChange = newSize.Height - oldSize.Height;

            application.SetGlobalState("ResizeInfo", new ResizeInfo
            {
                OldSize = oldSize,
                NewSize = newSize,
                WidthChange = widthChange,
                HeightChange = heightChange,
                Timestamp = DateTime.UtcNow
            });

            // Handle minimum size constraints
            if (newSize.Width < 20 || newSize.Height < 5)
            {
                application.SetGlobalState("ShowSizeWarning", true);
            }
            else
            {
                application.SetGlobalState("ShowSizeWarning", false);
            }

            // Handle very large sizes that might cause performance issues
            if (newSize.Width > 200 || newSize.Height > 60)
            {
                application.SetGlobalState("ShowPerformanceWarning", true);
            }
            else
            {
                application.SetGlobalState("ShowPerformanceWarning", false);
            }

            // Trigger re-layout and render
            application.RequestRender();

            // Notify about the resize
            OnResizeProcessed(new ResizeProcessedEventArgs
            {
                OldSize = oldSize,
                NewSize = newSize,
                Application = application
            });

            await Task.CompletedTask;
        }

        private bool TryGetCurrentSize(out (int Width, int Height) size)
        {
            try
            {
                // In a real implementation, this would get the actual terminal size
                size = (Console.WindowWidth, Console.WindowHeight);
                return true;
            }
            catch
            {
                size = (80, 24); // Default fallback
                return false;
            }
        }

        /// <summary>
        /// Event raised when a resize has been processed.
        /// </summary>
        public event System.EventHandler<ResizeProcessedEventArgs>? ResizeProcessed;

        private void OnResizeProcessed(ResizeProcessedEventArgs e)
        {
            ResizeProcessed?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Information about a resize operation.
    /// </summary>
    public class ResizeInfo
    {
        /// <summary>
        /// Gets or sets the old terminal size.
        /// </summary>
        public (int Width, int Height) OldSize { get; set; }

        /// <summary>
        /// Gets or sets the new terminal size.
        /// </summary>
        public (int Width, int Height) NewSize { get; set; }

        /// <summary>
        /// Gets or sets the width change (positive = larger, negative = smaller).
        /// </summary>
        public int WidthChange { get; set; }

        /// <summary>
        /// Gets or sets the height change (positive = larger, negative = smaller).
        /// </summary>
        public int HeightChange { get; set; }

        /// <summary>
        /// Gets or sets the timestamp when the resize occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Event arguments for resize processed events.
    /// </summary>
    public class ResizeProcessedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the old terminal size.
        /// </summary>
        public (int Width, int Height) OldSize { get; set; }

        /// <summary>
        /// Gets or sets the new terminal size.
        /// </summary>
        public (int Width, int Height) NewSize { get; set; }

        /// <summary>
        /// Gets or sets the application instance.
        /// </summary>
        public IApplication? Application { get; set; }
    }

    // Placeholder resize event type
    internal class ResizeEvent : IEvent
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public EventType Type => EventType.Resize;
        public bool IsHandled { get; private set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void Handle() => IsHandled = true;
    }
}