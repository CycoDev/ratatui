using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Events;

namespace CycoAI.CycoTui
{
    /// <summary>
    /// Interface for application plugins that extend CycoTui functionality.
    /// Plugins can handle events, update application state, and provide additional services.
    /// </summary>
    public interface IApplicationPlugin : IDisposable
    {
        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version of the plugin.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Initializes the plugin with the given application context.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="cancellationToken">Token to cancel the initialization.</param>
        /// <returns>A task representing the initialization operation.</returns>
        Task InitializeAsync(IApplication application, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the plugin state. Called regularly during the application event loop.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="cancellationToken">Token to cancel the update operation.</param>
        /// <returns>A task representing the update operation.</returns>
        Task UpdateAsync(IApplication application, CancellationToken cancellationToken = default);

        /// <summary>
        /// Handles an event and determines if it should continue propagating.
        /// </summary>
        /// <param name="event">The event to handle.</param>
        /// <param name="application">The application instance.</param>
        /// <param name="cancellationToken">Token to cancel the event handling.</param>
        /// <returns>A task that returns true if the event was handled and should stop propagating; otherwise, false.</returns>
        Task<bool> HandleEventAsync(IEvent @event, IApplication application, CancellationToken cancellationToken = default);

        /// <summary>
        /// Shuts down the plugin and releases resources.
        /// </summary>
        /// <param name="application">The application instance.</param>
        /// <param name="cancellationToken">Token to cancel the shutdown operation.</param>
        /// <returns>A task representing the shutdown operation.</returns>
        Task ShutdownAsync(IApplication application, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Interface representing the application from a plugin's perspective.
    /// Provides limited access to application functionality for plugins.
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Gets the current application state.
        /// </summary>
        ApplicationState State { get; }

        /// <summary>
        /// Gets a value indicating whether the application is currently running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Sets a global state value.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The state key.</param>
        /// <param name="value">The state value.</param>
        void SetGlobalState<T>(string key, T value);

        /// <summary>
        /// Gets a global state value.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The state key.</param>
        /// <returns>The state value.</returns>
        T GetGlobalState<T>(string key);

        /// <summary>
        /// Tries to get a global state value.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The state key.</param>
        /// <param name="value">The state value if found.</param>
        /// <returns>true if the value was found; otherwise, false.</returns>
        bool TryGetGlobalState<T>(string key, out T value);

        /// <summary>
        /// Requests a render on the next frame.
        /// </summary>
        void RequestRender();

        /// <summary>
        /// Subscribes to an event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <param name="handler">The event handler.</param>
        void Subscribe<TEvent>(CycoAI.CycoTui.Core.Events.EventHandler<TEvent> handler) where TEvent : IEvent;

        /// <summary>
        /// Unsubscribes from an event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <param name="handler">The event handler.</param>
        void Unsubscribe<TEvent>(CycoAI.CycoTui.Core.Events.EventHandler<TEvent> handler) where TEvent : IEvent;

        /// <summary>
        /// Stops the application.
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// Base class for application plugins that provides default implementations.
    /// </summary>
    public abstract class ApplicationPlugin : IApplicationPlugin
    {
        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public virtual Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

        /// <inheritdoc />
        public virtual Task InitializeAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task UpdateAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual Task<bool> HandleEventAsync(IEvent @event, IApplication application, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public virtual Task ShutdownAsync(IApplication application, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            // Default implementation does nothing
        }
    }
}