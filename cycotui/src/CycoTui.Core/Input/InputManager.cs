using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Events;

namespace CycoAI.CycoTui.Core.Input
{
    /// <summary>
    /// Manages input handling and event processing for the terminal UI.
    /// </summary>
    public class InputManager : IDisposable
    {
        private readonly IInputHandler _inputHandler;
        private readonly IEventQueue _eventQueue;
        private readonly IEventRouter _eventRouter;
        private readonly List<IInputFilter> _inputFilters;
        private readonly object _lockObject;

        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputManager"/> class.
        /// </summary>
        /// <param name="inputHandler">The input handler to use.</param>
        /// <param name="eventQueue">The event queue for processing events.</param>
        /// <param name="eventRouter">The event router for dispatching events.</param>
        public InputManager(IInputHandler inputHandler, IEventQueue eventQueue, IEventRouter eventRouter)
        {
            _inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));
            _eventQueue = eventQueue ?? throw new ArgumentNullException(nameof(eventQueue));
            _eventRouter = eventRouter ?? throw new ArgumentNullException(nameof(eventRouter));
            _inputFilters = new List<IInputFilter>();
            _lockObject = new object();

            _inputHandler.InputError += OnInputError;
        }

        /// <summary>
        /// Event raised when an input error occurs.
        /// </summary>
        public event System.EventHandler<InputErrorEventArgs>? InputError;

        /// <summary>
        /// Gets a value indicating whether the input manager is currently running.
        /// </summary>
        public bool IsRunning => _inputHandler.IsRunning;

        /// <summary>
        /// Gets the input handler being used.
        /// </summary>
        public IInputHandler InputHandler => _inputHandler;

        /// <summary>
        /// Gets the event queue being used.
        /// </summary>
        public IEventQueue EventQueue => _eventQueue;

        /// <summary>
        /// Gets the event router being used.
        /// </summary>
        public IEventRouter EventRouter => _eventRouter;

        /// <summary>
        /// Starts the input manager and begins processing input events.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the input processing.</param>
        /// <returns>A task representing the input processing operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InputManager));

            // Start the input handler with event processing
            var inputTask = _inputHandler.StartAsync(_eventQueue, cancellationToken);
            var processingTask = StartEventProcessingAsync(cancellationToken);

            // Wait for both tasks to complete
            await Task.WhenAll(inputTask, processingTask).ConfigureAwait(false);
        }

        /// <summary>
        /// Stops the input manager and releases resources.
        /// </summary>
        public void Stop()
        {
            _inputHandler.Stop();
        }

        /// <summary>
        /// Polls for a single input event synchronously.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for an event.</param>
        /// <returns>The next input event, or null if timeout occurred.</returns>
        public IEvent? PollEvent(TimeSpan timeout)
        {
            if (_disposed)
                return null;

            var @event = _inputHandler.PollEvent(timeout);
            return @event != null ? ApplyInputFilters(@event) : null;
        }

        /// <summary>
        /// Reads a single input event asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the read operation.</param>
        /// <returns>The next input event.</returns>
        public async Task<IEvent> ReadEventAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(InputManager));

            var @event = await _inputHandler.ReadEventAsync(cancellationToken).ConfigureAwait(false);
            return ApplyInputFilters(@event) ?? @event;
        }

        /// <summary>
        /// Adds an input filter to the processing pipeline.
        /// </summary>
        /// <param name="filter">The input filter to add.</param>
        public void AddInputFilter(IInputFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            lock (_lockObject)
            {
                _inputFilters.Add(filter);
                _inputFilters.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }
        }

        /// <summary>
        /// Removes an input filter from the processing pipeline.
        /// </summary>
        /// <param name="filter">The input filter to remove.</param>
        /// <returns>true if the filter was removed; otherwise, false.</returns>
        public bool RemoveInputFilter(IInputFilter filter)
        {
            if (filter == null)
                return false;

            lock (_lockObject)
            {
                return _inputFilters.Remove(filter);
            }
        }

        /// <summary>
        /// Clears all input filters.
        /// </summary>
        public void ClearInputFilters()
        {
            lock (_lockObject)
            {
                _inputFilters.Clear();
            }
        }

        /// <summary>
        /// Registers an event handler for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The event handler.</param>
        public void RegisterHandler<T>(Events.EventHandler<T> handler) where T : IEvent
        {
            _eventRouter.RegisterHandler(handler);
        }

        /// <summary>
        /// Unregisters an event handler for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The event handler to remove.</param>
        public void UnregisterHandler<T>(Events.EventHandler<T> handler) where T : IEvent
        {
            _eventRouter.UnregisterHandler(handler);
        }

        /// <summary>
        /// Adds an event filter to the router.
        /// </summary>
        /// <param name="filter">The event filter to add.</param>
        public void AddEventFilter(IEventFilter filter)
        {
            _eventRouter.AddFilter(filter);
        }

        /// <summary>
        /// Removes an event filter from the router.
        /// </summary>
        /// <param name="filter">The event filter to remove.</param>
        public void RemoveEventFilter(IEventFilter filter)
        {
            _eventRouter.RemoveFilter(filter);
        }

        /// <summary>
        /// Enables or disables mouse event capture.
        /// </summary>
        /// <param name="enable">Whether to enable mouse events.</param>
        public void SetMouseEnabled(bool enable)
        {
            _inputHandler.SetMouseEnabled(enable);
        }

        /// <summary>
        /// Enables or disables focus event capture.
        /// </summary>
        /// <param name="enable">Whether to enable focus events.</param>
        public void SetFocusEnabled(bool enable)
        {
            _inputHandler.SetFocusEnabled(enable);
        }

        /// <summary>
        /// Enables or disables paste event capture.
        /// </summary>
        /// <param name="enable">Whether to enable paste events.</param>
        public void SetPasteEnabled(bool enable)
        {
            _inputHandler.SetPasteEnabled(enable);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="InputManager"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Stop();
                    _inputHandler?.Dispose();
                    if (_eventQueue is IDisposable disposableQueue)
                        disposableQueue.Dispose();
                }

                _disposed = true;
            }
        }

        private async Task StartEventProcessingAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested && _inputHandler.IsRunning)
                {
                    try
                    {
                        // Wait for events in the queue and process them
                        var @event = await _eventQueue.WaitForEventAsync(cancellationToken).ConfigureAwait(false);

                        // Apply input filters
                        var filteredEvent = ApplyInputFilters(@event);
                        if (filteredEvent != null)
                        {
                            // Route the event through the event router
                            _eventRouter.RouteEvent(filteredEvent);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when cancellation is requested
                        break;
                    }
                    catch (Exception ex)
                    {
                        OnInputError(null, new InputErrorEventArgs(ex, "Error during event processing"));
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
        }

        private IEvent? ApplyInputFilters(IEvent @event)
        {
            var currentEvent = @event;

            lock (_lockObject)
            {
                foreach (var filter in _inputFilters)
                {
                    try
                    {
                        currentEvent = filter.FilterEvent(currentEvent);
                        if (currentEvent == null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        OnInputError(null, new InputErrorEventArgs(ex, $"Error in input filter {filter.GetType().Name}"));
                        continue;
                    }
                }
            }

            return currentEvent;
        }

        private void OnInputError(object? sender, InputErrorEventArgs e)
        {
            InputError?.Invoke(this, e);
        }
    }

    /// <summary>
    /// Interface for input filters that can modify or block input events before processing.
    /// </summary>
    public interface IInputFilter
    {
        /// <summary>
        /// Filters an input event, potentially modifying it or blocking it from further processing.
        /// </summary>
        /// <param name="event">The event to filter.</param>
        /// <returns>The filtered event, or null to block the event.</returns>
        IEvent? FilterEvent(IEvent @event);

        /// <summary>
        /// Gets the priority of this filter. Lower values have higher priority.
        /// </summary>
        int Priority { get; }
    }

    /// <summary>
    /// A simple input filter that blocks events based on a predicate.
    /// </summary>
    public class PredicateInputFilter : IInputFilter
    {
        private readonly Func<IEvent, bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateInputFilter"/> class.
        /// </summary>
        /// <param name="predicate">The predicate to determine if an event should be allowed through.</param>
        /// <param name="priority">The priority of this filter.</param>
        public PredicateInputFilter(Func<IEvent, bool> predicate, int priority = 0)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            Priority = priority;
        }

        /// <inheritdoc />
        public int Priority { get; }

        /// <inheritdoc />
        public IEvent? FilterEvent(IEvent @event)
        {
            return _predicate(@event) ? @event : null;
        }
    }

    /// <summary>
    /// An input filter that transforms events using a provided function.
    /// </summary>
    public class TransformInputFilter : IInputFilter
    {
        private readonly Func<IEvent, IEvent?> _transform;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformInputFilter"/> class.
        /// </summary>
        /// <param name="transform">The transformation function.</param>
        /// <param name="priority">The priority of this filter.</param>
        public TransformInputFilter(Func<IEvent, IEvent?> transform, int priority = 0)
        {
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            Priority = priority;
        }

        /// <inheritdoc />
        public int Priority { get; }

        /// <inheritdoc />
        public IEvent? FilterEvent(IEvent @event)
        {
            return _transform(@event);
        }
    }
}