using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CycoAI.CycoTui.Core.Events
{
    /// <summary>
    /// Interface for event queue operations.
    /// </summary>
    public interface IEventQueue
    {
        /// <summary>
        /// Enqueues an event for processing.
        /// </summary>
        /// <param name="event">The event to enqueue.</param>
        void Enqueue(IEvent @event);

        /// <summary>
        /// Attempts to dequeue an event.
        /// </summary>
        /// <param name="event">The dequeued event, if any.</param>
        /// <returns>true if an event was dequeued; otherwise, false.</returns>
        bool TryDequeue(out IEvent? @event);

        /// <summary>
        /// Gets all currently queued events and clears the queue.
        /// </summary>
        /// <returns>A collection of all queued events.</returns>
        IReadOnlyList<IEvent> DrainAll();

        /// <summary>
        /// Gets the number of events currently in the queue.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Clears all events from the queue.
        /// </summary>
        void Clear();

        /// <summary>
        /// Waits for an event to become available asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the wait operation.</param>
        /// <returns>The next available event.</returns>
        Task<IEvent> WaitForEventAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Filters events based on the provided predicate.
        /// </summary>
        /// <param name="predicate">The predicate to filter events.</param>
        /// <returns>A collection of events matching the predicate.</returns>
        IReadOnlyList<IEvent> FilterEvents(Func<IEvent, bool> predicate);
    }

    /// <summary>
    /// Thread-safe event queue implementation for managing terminal UI events.
    /// </summary>
    public class EventQueue : IEventQueue, IDisposable
    {
        private readonly ConcurrentQueue<IEvent> _queue;
        private readonly SemaphoreSlim _semaphore;
        private readonly object _lockObject;
        private volatile bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventQueue"/> class.
        /// </summary>
        public EventQueue()
        {
            _queue = new ConcurrentQueue<IEvent>();
            _semaphore = new SemaphoreSlim(0);
            _lockObject = new object();
        }

        /// <inheritdoc />
        public int Count => _queue.Count;

        /// <inheritdoc />
        public void Enqueue(IEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            if (_disposed)
                return;

            _queue.Enqueue(@event);
            _semaphore.Release();
        }

        /// <inheritdoc />
        public bool TryDequeue(out IEvent? @event)
        {
            @event = null;

            if (_disposed)
                return false;

            if (_queue.TryDequeue(out var result))
            {
                @event = result;
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public IReadOnlyList<IEvent> DrainAll()
        {
            if (_disposed)
                return Array.Empty<IEvent>();

            var events = new List<IEvent>();

            lock (_lockObject)
            {
                while (_queue.TryDequeue(out var @event))
                {
                    events.Add(@event);
                }
            }

            return events;
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (_disposed)
                return;

            lock (_lockObject)
            {
                while (_queue.TryDequeue(out _))
                {
                    // Clear all events
                }
            }
        }

        /// <inheritdoc />
        public async Task<IEvent> WaitForEventAsync(CancellationToken cancellationToken = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(EventQueue));

            // First check if there's already an event available
            if (TryDequeue(out var existingEvent))
                return existingEvent!;

            // Wait for a new event to be enqueued
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            // Try to dequeue the event
            if (TryDequeue(out var @event))
                return @event!;

            throw new InvalidOperationException("Event queue is in an inconsistent state.");
        }

        /// <inheritdoc />
        public IReadOnlyList<IEvent> FilterEvents(Func<IEvent, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            if (_disposed)
                return Array.Empty<IEvent>();

            lock (_lockObject)
            {
                var allEvents = new List<IEvent>();
                var filteredEvents = new List<IEvent>();

                // Drain all events
                while (_queue.TryDequeue(out var @event))
                {
                    allEvents.Add(@event);
                    if (predicate(@event))
                    {
                        filteredEvents.Add(@event);
                    }
                }

                // Re-enqueue non-matching events
                foreach (var @event in allEvents.Where(e => !predicate(e)))
                {
                    _queue.Enqueue(@event);
                }

                return filteredEvents;
            }
        }

        /// <summary>
        /// Releases all resources used by the <see cref="EventQueue"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="EventQueue"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Clear();
                    _semaphore?.Dispose();
                }

                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Event handler delegate for processing events.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <returns>true if the event was handled; false to continue propagation.</returns>
    public delegate bool EventHandler(IEvent @event);

    /// <summary>
    /// Generic event handler delegate for processing specific event types.
    /// </summary>
    /// <typeparam name="T">The type of event to handle.</typeparam>
    /// <param name="event">The event to handle.</param>
    /// <returns>true if the event was handled; false to continue propagation.</returns>
    public delegate bool EventHandler<in T>(T @event) where T : IEvent;
}