using System;
using System.Collections.Generic;
using CycoAI.CycoTui.Core.Backend;
using CycoAI.CycoTui.Core.Events;
using CycoAI.CycoTui.Core.Input;
using CycoAI.CycoTui.Core.Terminal;
using CycoAI.CycoTui.Core.Widgets;

namespace CycoAI.CycoTui
{
    /// <summary>
    /// Builder class for creating and configuring CycoTui applications.
    /// Provides a fluent API for complex application setup.
    /// </summary>
    public class ApplicationBuilder
    {
        private readonly ApplicationOptions _options;
        private readonly List<IApplicationPlugin> _plugins = new();
        private readonly Dictionary<Type, Delegate> _eventHandlers = new();
        private System.EventHandler<UnhandledExceptionEventArgs>? _exceptionHandler;
        private IWidget? _rootWidget;
        private IBackend? _customBackend;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBuilder"/> class.
        /// </summary>
        public ApplicationBuilder() : this(new ApplicationOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationBuilder"/> class with the specified options.
        /// </summary>
        /// <param name="options">The initial application options.</param>
        public ApplicationBuilder(ApplicationOptions options)
        {
            _options = options?.Clone() ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Sets the terminal backend type.
        /// </summary>
        /// <param name="backend">The backend type to use.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder WithBackend(TerminalBackendType backend)
        {
            _options.Backend = backend;
            return this;
        }

        /// <summary>
        /// Sets a custom terminal backend implementation.
        /// </summary>
        /// <typeparam name="TBackend">The type of the backend.</typeparam>
        /// <param name="backend">The backend instance to use.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder WithBackend<TBackend>(TBackend backend) where TBackend : IBackend
        {
            _customBackend = backend ?? throw new ArgumentNullException(nameof(backend));
            return this;
        }

        /// <summary>
        /// Sets the root widget for the application.
        /// </summary>
        /// <typeparam name="TWidget">The type of the widget.</typeparam>
        /// <param name="widget">The root widget instance.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder WithRootWidget<TWidget>(TWidget widget) where TWidget : IWidget
        {
            _rootWidget = widget ?? throw new ArgumentNullException(nameof(widget));
            return this;
        }

        /// <summary>
        /// Sets the viewport configuration.
        /// </summary>
        /// <param name="viewport">The viewport to use.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder WithViewport(Viewport viewport)
        {
            _options.Viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));
            return this;
        }

        /// <summary>
        /// Enables or disables mouse input.
        /// </summary>
        /// <param name="enable">Whether to enable mouse input.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder EnableMouse(bool enable = true)
        {
            _options.EnableMouse = enable;
            return this;
        }

        /// <summary>
        /// Enables or disables resize events.
        /// </summary>
        /// <param name="enable">Whether to enable resize events.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder EnableResize(bool enable = true)
        {
            _options.EnableResize = enable;
            return this;
        }

        /// <summary>
        /// Sets the target frame rate.
        /// </summary>
        /// <param name="fps">The target frames per second.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder WithRenderRate(int fps)
        {
            if (fps <= 0)
                throw new ArgumentException("FPS must be positive.", nameof(fps));

            _options.RenderInterval = TimeSpan.FromMilliseconds(1000.0 / fps);
            return this;
        }

        /// <summary>
        /// Sets the input polling interval.
        /// </summary>
        /// <param name="interval">The polling interval.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder WithInputPollRate(TimeSpan interval)
        {
            if (interval <= TimeSpan.Zero)
                throw new ArgumentException("Interval must be positive.", nameof(interval));

            _options.InputPollInterval = interval;
            return this;
        }

        /// <summary>
        /// Adds a plugin to the application.
        /// </summary>
        /// <typeparam name="TPlugin">The type of the plugin.</typeparam>
        /// <param name="plugin">The plugin instance.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder AddPlugin<TPlugin>(TPlugin plugin) where TPlugin : IApplicationPlugin
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            _plugins.Add(plugin);
            return this;
        }

        /// <summary>
        /// Registers an event handler for a specific event type.
        /// </summary>
        /// <typeparam name="TEvent">The type of event to handle.</typeparam>
        /// <param name="handler">The event handler.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder OnEvent<TEvent>(CycoAI.CycoTui.Core.Events.EventHandler<TEvent> handler) where TEvent : IEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(TEvent);
            if (!_eventHandlers.ContainsKey(eventType))
            {
                _eventHandlers[eventType] = handler;
            }
            else
            {
                // Combine handlers
                var existing = _eventHandlers[eventType];
                _eventHandlers[eventType] = Delegate.Combine(existing, handler);
            }

            return this;
        }

        /// <summary>
        /// Sets the unhandled exception handler.
        /// </summary>
        /// <param name="handler">The exception handler.</param>
        /// <returns>The builder instance for method chaining.</returns>
        public ApplicationBuilder OnException(System.EventHandler<UnhandledExceptionEventArgs> handler)
        {
            _exceptionHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            return this;
        }

        /// <summary>
        /// Builds and returns the configured application.
        /// </summary>
        /// <returns>A new application instance.</returns>
        public Application Build()
        {
            // Create backend
            var backend = _customBackend ?? CreateBackend(_options.Backend);

            // Create terminal
            var terminal = CreateTerminal(backend);

            // Create event system
            var eventQueue = new EventQueue();
            var eventRouter = new EventRouter();

            // Register event handlers
            foreach (var kvp in _eventHandlers)
            {
                // This is a simplified registration - in reality we'd need to properly handle
                // the generic RegisterHandler method with reflection or a more sophisticated approach
                var method = eventRouter.GetType().GetMethod("RegisterHandler");
                if (method != null)
                {
                    var genericMethod = method.MakeGenericMethod(kvp.Key);
                    genericMethod.Invoke(eventRouter, new object[] { kvp.Value });
                }
            }

            // Create input handler
            var inputHandler = CreateInputHandler(_options);

            // Create application
            var application = new Application(terminal, eventQueue, eventRouter, inputHandler, _options);

            // Add plugins
            foreach (var plugin in _plugins)
            {
                application.GetType()
                    .GetField("_plugins", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                    .GetValue(application)
                    ?.GetType()
                    .GetMethod("Add")?
                    .Invoke(application.GetType()
                        .GetField("_plugins", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                        .GetValue(application), new object[] { plugin });
            }

            // Set root widget if provided
            if (_rootWidget != null)
            {
                application.SetRootWidget(_rootWidget);
            }

            // Set exception handler if provided
            if (_exceptionHandler != null)
            {
                application.UnhandledException += _exceptionHandler;
            }

            return application;
        }

        private IBackend CreateBackend(TerminalBackendType backendType)
        {
            return backendType switch
            {
                TerminalBackendType.Auto => CreateAutoBackend(),
                TerminalBackendType.Crossterm => new CrosstermBackend(),
                TerminalBackendType.Unix => CreateUnixBackend(),
                TerminalBackendType.Windows => CreateWindowsBackend(),
                _ => throw new NotSupportedException($"Backend type {backendType} is not supported.")
            };
        }

        private IBackend CreateAutoBackend()
        {
            // Automatically select the best backend for the current platform
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return CreateWindowsBackend();
            }
            else
            {
                return CreateUnixBackend();
            }
        }

        private IBackend CreateUnixBackend()
        {
            // For now, return a placeholder or existing implementation
            return new UnixBackend();
        }

        private IBackend CreateWindowsBackend()
        {
            // For now, fall back to crossterm-style backend
            return new CrosstermBackend();
        }

        private ITerminal CreateTerminal(IBackend backend)
        {
            return new Terminal(backend);
        }

        private IInputHandler CreateInputHandler(ApplicationOptions options)
        {
            // Create input handler based on options
            // For now, use a console input handler
            return new ConsoleInputHandler();
        }
    }

    // Placeholder implementations for backends that don't exist yet
    internal class CrosstermBackend : IBackend
    {
        public (int Width, int Height) GetSize() => (80, 24);
        public void Clear() { }
        public void HideCursor() { }
        public void ShowCursor() { }
        public (int X, int Y) GetCursor() => (0, 0);
        public void SetCursor(int x, int y) { }
        public void Flush() { }
        public void EnterAlternateScreen() { }
        public void LeaveAlternateScreen() { }
        public void EnableRawMode() { }
        public void DisableRawMode() { }
        public void Write(string text) { }
        public System.IO.TextWriter Output => Console.Out;
        public bool SupportsColor => true;
        public bool SupportsAlternateScreen => true;
        public bool SupportsMouse => true;
        public void Dispose() { }
    }

    // Interface placeholder for ITerminal
    internal interface ITerminal : IDisposable
    {
        CycoAI.CycoTui.Core.Layout.Rect Size { get; }
        CycoAI.CycoTui.Core.Buffer.IBuffer GetBuffer();
        void Draw();
    }

    // Wrapper around the existing Terminal class to implement ITerminal
    internal class Terminal : ITerminal
    {
        private readonly CycoAI.CycoTui.Core.Terminal.Terminal _terminal;

        public Terminal(IBackend backend)
        {
            _terminal = new CycoAI.CycoTui.Core.Terminal.Terminal(backend);
        }

        public CycoAI.CycoTui.Core.Layout.Rect Size => _terminal.Size;

        public CycoAI.CycoTui.Core.Buffer.IBuffer GetBuffer() => _terminal.GetBuffer();

        public void Draw() => _terminal.Draw();

        public void Dispose() => _terminal.Dispose();
    }
}