using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CycoAI.CycoTui.Core.Backend;
using CycoAI.CycoTui.Core.Events;
using CycoAI.CycoTui.Core.Input;
using CycoAI.CycoTui.Core.Terminal;
using CycoAI.CycoTui.Core.Widgets;

namespace CycoAI.CycoTui
{
    /// <summary>
    /// The main application class that integrates all CycoTui components into a cohesive terminal application.
    /// Provides lifecycle management, event handling, and high-level API for building terminal applications.
    /// </summary>
    public class Application : IApplication, IDisposable
    {
        private readonly ITerminal _terminal;
        private readonly EventRouter _eventRouter;
        private readonly EventQueue _eventQueue;
        private readonly IInputHandler _inputHandler;
        private readonly ApplicationOptions _options;
        private readonly object _lockObject = new object();

        private bool _isRunning;
        private CancellationTokenSource? _cancellationTokenSource;
        private volatile bool _needsRender = true;
        private volatile bool _disposed;

        // Application state
        private IWidget? _rootWidget;
        private ApplicationState _state = ApplicationState.Created;
        private readonly ConcurrentDictionary<string, object> _globalState = new();

        // Event handling
        private readonly Dictionary<Type, Delegate> _eventHandlers = new();
        private readonly List<IApplicationPlugin> _plugins = new();

        // Lifecycle events
        public event System.EventHandler<ApplicationStartedEventArgs>? Started;
        public event System.EventHandler<ApplicationStoppedEventArgs>? Stopped;
        public event System.EventHandler<RenderFrameEventArgs>? FrameRendered;
        public event System.EventHandler<UnhandledExceptionEventArgs>? UnhandledException;

        /// <summary>
        /// Gets the current application state.
        /// </summary>
        public ApplicationState State => _state;

        /// <summary>
        /// Gets a value indicating whether the application is currently running.
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Gets the current application instance.
        /// </summary>
        public static Application? Current { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Application"/> class.
        /// </summary>
        /// <param name="terminal">The terminal instance.</param>
        /// <param name="eventQueue">The event queue instance.</param>
        /// <param name="eventRouter">The event router instance.</param>
        /// <param name="inputHandler">The input handler instance.</param>
        /// <param name="options">The application options.</param>
        internal Application(
            ITerminal terminal,
            EventQueue eventQueue,
            EventRouter eventRouter,
            IInputHandler inputHandler,
            ApplicationOptions options)
        {
            _terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
            _eventQueue = eventQueue ?? throw new ArgumentNullException(nameof(eventQueue));
            _eventRouter = eventRouter ?? throw new ArgumentNullException(nameof(eventRouter));
            _inputHandler = inputHandler ?? throw new ArgumentNullException(nameof(inputHandler));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            Current = this;
        }

        /// <summary>
        /// Creates a simple application with default settings.
        /// </summary>
        /// <returns>A new application instance.</returns>
        public static Application Create()
        {
            return Create(new ApplicationOptions());
        }

        /// <summary>
        /// Creates an application with the specified options.
        /// </summary>
        /// <param name="options">The application options.</param>
        /// <returns>A new application instance.</returns>
        public static Application Create(ApplicationOptions options)
        {
            var builder = new ApplicationBuilder(options);
            return builder.Build();
        }

        /// <summary>
        /// Creates an application with a root widget and default settings.
        /// </summary>
        /// <typeparam name="TWidget">The type of the root widget.</typeparam>
        /// <param name="rootWidget">The root widget.</param>
        /// <returns>A new application instance.</returns>
        public static Application Create<TWidget>(TWidget rootWidget) where TWidget : IWidget
        {
            var builder = new ApplicationBuilder();
            return builder.WithRootWidget(rootWidget).Build();
        }

        /// <summary>
        /// Creates an application with a root widget and the specified options.
        /// </summary>
        /// <typeparam name="TWidget">The type of the root widget.</typeparam>
        /// <param name="rootWidget">The root widget.</param>
        /// <param name="options">The application options.</param>
        /// <returns>A new application instance.</returns>
        public static Application Create<TWidget>(TWidget rootWidget, ApplicationOptions options) where TWidget : IWidget
        {
            var builder = new ApplicationBuilder(options);
            return builder.WithRootWidget(rootWidget).Build();
        }

        /// <summary>
        /// Creates a new application builder for complex configuration.
        /// </summary>
        /// <returns>A new application builder instance.</returns>
        public static ApplicationBuilder Builder()
        {
            return new ApplicationBuilder();
        }

        /// <summary>
        /// Runs the application synchronously.
        /// </summary>
        public void Run()
        {
            Run(CancellationToken.None);
        }

        /// <summary>
        /// Runs the application synchronously with a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void Run(CancellationToken cancellationToken)
        {
            RunAsync(cancellationToken).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Runs the application asynchronously.
        /// </summary>
        /// <returns>A task representing the application execution.</returns>
        public Task RunAsync()
        {
            return RunAsync(CancellationToken.None);
        }

        /// <summary>
        /// Runs the application asynchronously with a cancellation token.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the application execution.</returns>
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Application));

            if (_isRunning)
                throw new InvalidOperationException("Application is already running.");

            _state = ApplicationState.Initializing;
            _isRunning = true;
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            try
            {
                await InitializeAsync(_cancellationTokenSource.Token);
                _state = ApplicationState.Running;

                Started?.Invoke(this, new ApplicationStartedEventArgs());

                await RunEventLoopAsync(_cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // Expected cancellation
            }
            catch (Exception ex)
            {
                _state = ApplicationState.Error;
                UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
                throw;
            }
            finally
            {
                await ShutdownAsync();
            }
        }

        /// <summary>
        /// Executes a single tick of the application (for integration scenarios).
        /// </summary>
        public void Tick()
        {
            TickAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes a single tick of the application asynchronously.
        /// </summary>
        /// <returns>A task representing the tick operation.</returns>
        public async Task TickAsync()
        {
            if (!_isRunning)
                return;

            try
            {
                await ProcessEventsAsync(CancellationToken.None);
                await UpdateStateAsync(CancellationToken.None);

                if (_needsRender)
                {
                    await RenderFrameAsync(CancellationToken.None);
                    _needsRender = false;
                }
            }
            catch (Exception ex)
            {
                UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public void Stop()
        {
            Stop(TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Stops the application with a timeout.
        /// </summary>
        /// <param name="timeout">The maximum time to wait for shutdown.</param>
        public void Stop(TimeSpan timeout)
        {
            if (!_isRunning)
                return;

            _cancellationTokenSource?.Cancel();

            // Give the application time to shut down gracefully
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (_isRunning && stopwatch.Elapsed < timeout)
            {
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// Sets the root widget for the application.
        /// </summary>
        /// <typeparam name="TWidget">The type of the root widget.</typeparam>
        /// <param name="widget">The root widget.</param>
        public void SetRootWidget<TWidget>(TWidget widget) where TWidget : IWidget
        {
            lock (_lockObject)
            {
                _rootWidget = widget;
                RequestRender();
            }
        }

        /// <summary>
        /// Gets the root widget.
        /// </summary>
        /// <typeparam name="TWidget">The type of the root widget.</typeparam>
        /// <returns>The root widget, or null if not set or not of the specified type.</returns>
        public TWidget? GetRootWidget<TWidget>() where TWidget : class, IWidget
        {
            lock (_lockObject)
            {
                return _rootWidget as TWidget;
            }
        }

        /// <summary>
        /// Sets a global state value.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The state key.</param>
        /// <param name="value">The state value.</param>
        public void SetGlobalState<T>(string key, T value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            _globalState[key] = value!;
        }

        /// <summary>
        /// Gets a global state value.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The state key.</param>
        /// <returns>The state value.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the key is not found.</exception>
        public T GetGlobalState<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.", nameof(key));

            if (_globalState.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;

            throw new KeyNotFoundException($"Global state key '{key}' not found or is not of type {typeof(T).Name}.");
        }

        /// <summary>
        /// Tries to get a global state value.
        /// </summary>
        /// <typeparam name="T">The type of the state value.</typeparam>
        /// <param name="key">The state key.</param>
        /// <param name="value">The state value if found.</param>
        /// <returns>true if the value was found; otherwise, false.</returns>
        public bool TryGetGlobalState<T>(string key, out T value)
        {
            value = default!;

            if (string.IsNullOrEmpty(key))
                return false;

            if (_globalState.TryGetValue(key, out var objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes a global state value.
        /// </summary>
        /// <param name="key">The state key.</param>
        public void RemoveGlobalState(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _globalState.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Requests a render on the next frame.
        /// </summary>
        public void RequestRender()
        {
            _needsRender = true;
        }

        /// <summary>
        /// Subscribes to an event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <param name="handler">The event handler.</param>
        public void Subscribe<TEvent>(CycoAI.CycoTui.Core.Events.EventHandler<TEvent> handler) where TEvent : IEvent
        {
            _eventRouter.RegisterHandler(handler);
        }

        /// <summary>
        /// Unsubscribes from an event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event.</typeparam>
        /// <param name="handler">The event handler.</param>
        public void Unsubscribe<TEvent>(CycoAI.CycoTui.Core.Events.EventHandler<TEvent> handler) where TEvent : IEvent
        {
            _eventRouter.UnregisterHandler(handler);
        }

        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            // Initialize terminal
            await InitializeTerminalAsync();

            // Initialize plugins
            foreach (var plugin in _plugins)
            {
                await plugin.InitializeAsync(this, cancellationToken);
            }

            // Start input handler
            await _inputHandler.StartAsync(_eventQueue, cancellationToken);

            // Initial render
            if (_rootWidget != null)
            {
                await RenderFrameAsync(cancellationToken);
            }
        }

        private async Task InitializeTerminalAsync()
        {
            // Terminal initialization will be handled by the terminal itself
            // when it's created in the ApplicationBuilder
            await Task.CompletedTask;
        }

        private async Task RunEventLoopAsync(CancellationToken cancellationToken)
        {
            var renderTimer = new Timer(RenderCallback, null, TimeSpan.Zero, _options.RenderInterval);
            var inputTimer = new Timer(InputCallback, null, TimeSpan.Zero, _options.InputPollInterval);

            try
            {
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // Process pending events
                        await ProcessEventsAsync(cancellationToken);

                        // Update application state
                        await UpdateStateAsync(cancellationToken);

                        // Render frame if needed
                        if (_needsRender)
                        {
                            await RenderFrameAsync(cancellationToken);
                            _needsRender = false;
                        }

                        // Small delay to prevent CPU spinning
                        await Task.Delay(1, cancellationToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        await HandleUnhandledExceptionAsync(ex);
                    }
                }
            }
            finally
            {
                renderTimer?.Dispose();
                inputTimer?.Dispose();
            }
        }

        private async Task ProcessEventsAsync(CancellationToken cancellationToken)
        {
            // Process all available events
            while (_eventQueue.TryDequeue(out var @event))
            {
                try
                {
                    // Route event through application handlers first
                    var handled = await RouteApplicationEventAsync(@event, cancellationToken);

                    if (!handled)
                    {
                        // Route to widget system
                        await RouteWidgetEventAsync(@event, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    await HandleEventExceptionAsync(@event, ex);
                }
            }
        }

        private async Task<bool> RouteApplicationEventAsync(IEvent @event, CancellationToken cancellationToken)
        {
            // Route through event router
            var handled = _eventRouter.RouteEvent(@event);

            // Route through plugins
            foreach (var plugin in _plugins)
            {
                try
                {
                    if (await plugin.HandleEventAsync(@event, this, cancellationToken))
                    {
                        handled = true;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // Log plugin exception but continue
                    await HandleEventExceptionAsync(@event, ex);
                }
            }

            return handled;
        }

        private async Task RouteWidgetEventAsync(IEvent @event, CancellationToken cancellationToken)
        {
            // Widget event routing will be implemented as the widget system develops
            await Task.CompletedTask;
        }

        private async Task UpdateStateAsync(CancellationToken cancellationToken)
        {
            // Update plugins
            foreach (var plugin in _plugins)
            {
                try
                {
                    await plugin.UpdateAsync(this, cancellationToken);
                }
                catch (Exception ex)
                {
                    await HandleEventExceptionAsync(null, ex);
                }
            }
        }

        private async Task RenderFrameAsync(CancellationToken cancellationToken)
        {
            if (_rootWidget == null)
                return;

            try
            {
                var buffer = _terminal.GetBuffer();
                var area = _terminal.Size;

                _rootWidget.Render(area, buffer);
                _terminal.Draw();

                FrameRendered?.Invoke(this, new RenderFrameEventArgs());
            }
            catch (Exception ex)
            {
                await HandleEventExceptionAsync(null, ex);
            }
        }

        private async Task HandleUnhandledExceptionAsync(Exception ex)
        {
            UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            await Task.CompletedTask;
        }

        private async Task HandleEventExceptionAsync(IEvent? @event, Exception ex)
        {
            // In a real implementation, this would log the exception
            await Task.CompletedTask;
        }

        private void RenderCallback(object? state)
        {
            // Trigger render on timer
            _needsRender = true;
        }

        private void InputCallback(object? state)
        {
            // Input is handled by the input handler itself
        }

        private async Task ShutdownAsync()
        {
            if (_state == ApplicationState.Stopped)
                return;

            _state = ApplicationState.Stopping;

            try
            {
                // Stop input handler
                _inputHandler?.Stop();

                // Shutdown plugins
                foreach (var plugin in _plugins)
                {
                    try
                    {
                        await plugin.ShutdownAsync(this, CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        // Log but don't throw during shutdown
                        await HandleEventExceptionAsync(null, ex);
                    }
                }

                // Dispose resources
                _cancellationTokenSource?.Dispose();

                _state = ApplicationState.Stopped;
                _isRunning = false;

                Stopped?.Invoke(this, new ApplicationStoppedEventArgs());
            }
            catch (Exception ex)
            {
                _state = ApplicationState.Error;
                UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(ex, false));
            }
            finally
            {
                Current = null;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
                return;

            try
            {
                Stop();

                _terminal?.Dispose();
                _eventQueue?.Dispose();
                _inputHandler?.Dispose();

                foreach (var plugin in _plugins)
                {
                    plugin?.Dispose();
                }
                _plugins.Clear();

                _cancellationTokenSource?.Dispose();
            }
            finally
            {
                _disposed = true;
                Current = null;
            }

            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Enumeration of application states.
    /// </summary>
    public enum ApplicationState
    {
        /// <summary>
        /// Application has been created but not yet started.
        /// </summary>
        Created,

        /// <summary>
        /// Application is initializing.
        /// </summary>
        Initializing,

        /// <summary>
        /// Application is running.
        /// </summary>
        Running,

        /// <summary>
        /// Application is stopping.
        /// </summary>
        Stopping,

        /// <summary>
        /// Application has stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// Application encountered an error.
        /// </summary>
        Error
    }
}