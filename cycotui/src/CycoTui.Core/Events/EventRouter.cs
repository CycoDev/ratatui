using System;
using System.Collections.Generic;
using System.Linq;

namespace CycoAI.CycoTui.Core.Events
{
    /// <summary>
    /// Interface for event routing and filtering.
    /// </summary>
    public interface IEventRouter
    {
        /// <summary>
        /// Registers an event handler for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The event handler.</param>
        void RegisterHandler<T>(EventHandler<T> handler) where T : IEvent;

        /// <summary>
        /// Unregisters an event handler for a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to handle.</typeparam>
        /// <param name="handler">The event handler to remove.</param>
        void UnregisterHandler<T>(EventHandler<T> handler) where T : IEvent;

        /// <summary>
        /// Routes an event to the appropriate handlers.
        /// </summary>
        /// <param name="event">The event to route.</param>
        /// <returns>true if the event was handled by any handler; otherwise, false.</returns>
        bool RouteEvent(IEvent @event);

        /// <summary>
        /// Adds an event filter that can modify or block events before they reach handlers.
        /// </summary>
        /// <param name="filter">The event filter.</param>
        void AddFilter(IEventFilter filter);

        /// <summary>
        /// Removes an event filter.
        /// </summary>
        /// <param name="filter">The event filter to remove.</param>
        void RemoveFilter(IEventFilter filter);

        /// <summary>
        /// Clears all registered handlers and filters.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Interface for event filters that can modify or block events.
    /// </summary>
    public interface IEventFilter
    {
        /// <summary>
        /// Filters an event, potentially modifying it or blocking it from further processing.
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
    /// Event router implementation that manages event handlers and filters.
    /// </summary>
    public class EventRouter : IEventRouter
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers;
        private readonly List<IEventFilter> _filters;
        private readonly object _lockObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventRouter"/> class.
        /// </summary>
        public EventRouter()
        {
            _handlers = new Dictionary<Type, List<Delegate>>();
            _filters = new List<IEventFilter>();
            _lockObject = new object();
        }

        /// <inheritdoc />
        public void RegisterHandler<T>(EventHandler<T> handler) where T : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lockObject)
            {
                var eventType = typeof(T);
                if (!_handlers.ContainsKey(eventType))
                {
                    _handlers[eventType] = new List<Delegate>();
                }

                _handlers[eventType].Add(handler);
            }
        }

        /// <inheritdoc />
        public void UnregisterHandler<T>(EventHandler<T> handler) where T : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lockObject)
            {
                var eventType = typeof(T);
                if (_handlers.ContainsKey(eventType))
                {
                    _handlers[eventType].Remove(handler);
                    if (_handlers[eventType].Count == 0)
                    {
                        _handlers.Remove(eventType);
                    }
                }
            }
        }

        /// <inheritdoc />
        public bool RouteEvent(IEvent @event)
        {
            if (@event == null)
                return false;

            // Apply filters first
            var filteredEvent = ApplyFilters(@event);
            if (filteredEvent == null)
                return true; // Event was filtered out (considered handled)

            // Route to handlers
            return InvokeHandlers(filteredEvent);
        }

        /// <inheritdoc />
        public void AddFilter(IEventFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            lock (_lockObject)
            {
                _filters.Add(filter);
                _filters.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            }
        }

        /// <inheritdoc />
        public void RemoveFilter(IEventFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            lock (_lockObject)
            {
                _filters.Remove(filter);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (_lockObject)
            {
                _handlers.Clear();
                _filters.Clear();
            }
        }

        private IEvent? ApplyFilters(IEvent @event)
        {
            var currentEvent = @event;

            lock (_lockObject)
            {
                foreach (var filter in _filters)
                {
                    try
                    {
                        currentEvent = filter.FilterEvent(currentEvent);
                        if (currentEvent == null)
                            break;
                    }
                    catch (Exception)
                    {
                        // Log the exception in a real implementation
                        // For now, continue with the original event
                        continue;
                    }
                }
            }

            return currentEvent;
        }

        private bool InvokeHandlers(IEvent @event)
        {
            var eventType = @event.GetType();
            var handled = false;

            lock (_lockObject)
            {
                // Look for handlers for the exact type and base types
                var typesToCheck = GetTypeHierarchy(eventType);

                foreach (var type in typesToCheck)
                {
                    if (_handlers.ContainsKey(type))
                    {
                        var handlers = _handlers[type].ToList(); // Copy to avoid modification during iteration

                        foreach (var handler in handlers)
                        {
                            try
                            {
                                if (handler.DynamicInvoke(@event) is bool result && result)
                                {
                                    handled = true;
                                    @event.Handle();
                                    break; // Stop processing if event is handled
                                }
                            }
                            catch (Exception)
                            {
                                // Log the exception in a real implementation
                                // Continue with other handlers
                                continue;
                            }

                            if (@event.IsHandled)
                                break;
                        }

                        if (@event.IsHandled)
                            break;
                    }
                }
            }

            return handled;
        }

        private static IEnumerable<Type> GetTypeHierarchy(Type type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;

                // Also check interfaces
                foreach (var @interface in current?.GetInterfaces() ?? Array.Empty<Type>())
                {
                    yield return @interface;
                }
            }
        }
    }

    /// <summary>
    /// A simple event filter that blocks events based on a predicate.
    /// </summary>
    public class PredicateEventFilter : IEventFilter
    {
        private readonly Func<IEvent, bool> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateEventFilter"/> class.
        /// </summary>
        /// <param name="predicate">The predicate to determine if an event should be allowed through.</param>
        /// <param name="priority">The priority of this filter.</param>
        public PredicateEventFilter(Func<IEvent, bool> predicate, int priority = 0)
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
    /// An event filter that transforms events using a provided function.
    /// </summary>
    public class TransformEventFilter : IEventFilter
    {
        private readonly Func<IEvent, IEvent?> _transform;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformEventFilter"/> class.
        /// </summary>
        /// <param name="transform">The transformation function.</param>
        /// <param name="priority">The priority of this filter.</param>
        public TransformEventFilter(Func<IEvent, IEvent?> transform, int priority = 0)
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