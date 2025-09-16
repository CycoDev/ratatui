# Main Application Class Implementation

## Overview

This task implements the main `Application` class that serves as the primary entry point for CycoTui applications. The Application class coordinates all major systems (terminal control, rendering, events, input) and provides the high-level developer API for building terminal applications.

## Implementation Approach

1. **Create Application Class Architecture**:
   - Implement main `Application` class with lifecycle management
   - Create `ApplicationBuilder` for flexible application configuration
   - Implement `ApplicationOptions` for customizing application behavior
   - Support both simple factory methods and complex builder patterns

2. **Implement Lifecycle Management**:
   - Handle application initialization, running, and shutdown phases
   - Support both synchronous (`Run()`) and asynchronous (`RunAsync()`) execution
   - Implement graceful shutdown with configurable timeout
   - Provide single-frame execution for integration scenarios

3. **Integrate Core Systems**:
   - Coordinate terminal controller for low-level terminal operations
   - Integrate rendering engine for widget display
   - Connect event system for user input processing
   - Manage application state and global data storage

4. **Build High-Level Developer API**:
   - Provide simple application creation patterns
   - Support root widget management and replacement
   - Implement global state management accessible to all components
   - Create event subscription and handling mechanisms

## Key Challenges

1. **System Coordination**: Ensuring proper initialization and shutdown order of all subsystems
2. **Error Handling**: Graceful recovery from component failures without crashing
3. **Thread Safety**: Coordinating potentially multi-threaded operations safely
4. **Resource Management**: Proper disposal of all resources during shutdown
5. **API Design**: Balancing simplicity with flexibility and power

## Implementation Notes

### Application Class Structure

```csharp
public class Application : IDisposable
{
    // Core subsystems
    private readonly ITerminalController terminalController;
    private readonly IRenderEngine renderEngine;
    private readonly EventQueue eventQueue;
    private readonly EventRouter eventRouter;
    private readonly IInputHandler inputHandler;

    // Application state
    private IWidget rootWidget;
    private readonly Dictionary<string, object> globalState;
    private readonly Dictionary<object, object> widgetStates;
    private ApplicationState currentState;
    private CancellationTokenSource cancellationTokenSource;

    // Configuration
    private readonly ApplicationOptions options;

    // Events
    public event EventHandler<ApplicationStartedEventArgs> Started;
    public event EventHandler<ApplicationStoppedEventArgs> Stopped;
    public event EventHandler<FrameRenderedEventArgs> FrameRendered;
    public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

    // Core API
    public void Run() => RunAsync().GetAwaiter().GetResult();
    public void Run(CancellationToken cancellationToken) => RunAsync(cancellationToken).GetAwaiter().GetResult();
    public async Task RunAsync(CancellationToken cancellationToken = default);

    public void Stop();
    public void Stop(TimeSpan timeout);

    public bool IsRunning => currentState == ApplicationState.Running;
    public ApplicationState State => currentState;
}

public enum ApplicationState
{
    Created,
    Initializing,
    Running,
    Stopping,
    Stopped,
    Error
}
```

### Factory Methods and Builder

```csharp
public static class Application
{
    // Simple factory methods
    public static Application Create()
        => Create(new Paragraph("Hello, CycoTui!"));

    public static Application Create<TWidget>(TWidget rootWidget) where TWidget : IWidget
        => Create(rootWidget, new ApplicationOptions());

    public static Application Create<TWidget>(TWidget rootWidget, ApplicationOptions options) where TWidget : IWidget
        => new Application(rootWidget, options);

    // Builder pattern for complex scenarios
    public static ApplicationBuilder Builder()
        => new ApplicationBuilder();
}

public class ApplicationBuilder
{
    private readonly ApplicationOptions options = new();
    private IWidget rootWidget;
    private readonly List<IApplicationPlugin> plugins = new();
    private readonly Dictionary<Type, Delegate> eventHandlers = new();

    public ApplicationBuilder WithRootWidget<TWidget>(TWidget widget) where TWidget : IWidget
    {
        rootWidget = widget;
        return this;
    }

    public ApplicationBuilder WithBackend(TerminalBackendType backend)
    {
        options.Backend = backend;
        return this;
    }

    public ApplicationBuilder EnableMouse(bool enable = true)
    {
        options.EnableMouse = enable;
        return this;
    }

    public ApplicationBuilder WithRenderRate(int fps)
    {
        options.RenderInterval = TimeSpan.FromMilliseconds(1000.0 / fps);
        return this;
    }

    public ApplicationBuilder OnEvent<TEvent>(EventHandler<TEvent> handler) where TEvent : IEvent
    {
        eventHandlers[typeof(TEvent)] = handler;
        return this;
    }

    public ApplicationBuilder AddPlugin<TPlugin>(TPlugin plugin) where TPlugin : IApplicationPlugin
    {
        plugins.Add(plugin);
        return this;
    }

    public Application Build()
    {
        var app = new Application(rootWidget ?? new Paragraph("Empty Application"), options);

        // Register event handlers
        foreach (var (eventType, handler) in eventHandlers)
        {
            app.RegisterEventHandler(eventType, handler);
        }

        // Add plugins
        foreach (var plugin in plugins)
        {
            app.AddPlugin(plugin);
        }

        return app;
    }
}
```

### Application Lifecycle Implementation

```csharp
public async Task RunAsync(CancellationToken cancellationToken = default)
{
    using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationTokenSource.Token);
    var token = combinedCts.Token;

    try
    {
        // Initialize all subsystems
        await InitializeAsync(token);

        // Raise Started event
        OnStarted(new ApplicationStartedEventArgs());

        // Run main application loop
        await RunMainLoopAsync(token);
    }
    catch (OperationCanceledException) when (token.IsCancellationRequested)
    {
        // Expected during shutdown
    }
    catch (Exception ex)
    {
        currentState = ApplicationState.Error;
        OnUnhandledException(new UnhandledExceptionEventArgs(ex));
        throw;
    }
    finally
    {
        // Always attempt cleanup
        await ShutdownAsync();
        OnStopped(new ApplicationStoppedEventArgs());
    }
}

private async Task InitializeAsync(CancellationToken cancellationToken)
{
    currentState = ApplicationState.Initializing;

    try
    {
        // 1. Initialize terminal controller
        await terminalController.EnableRawModeAsync();
        if (options.EnableAlternateScreen)
            await terminalController.EnterAlternateScreenAsync();
        if (options.EnableMouse)
            await terminalController.EnableMouseCaptureAsync();
        await terminalController.HideCursorAsync();

        // 2. Initialize rendering engine
        renderEngine.FrameRendered += OnFrameRendered;

        // 3. Initialize event system
        eventRouter.Subscribe<KeyEvent>(HandleKeyEvent);
        eventRouter.Subscribe<ResizeEvent>(HandleResizeEvent);
        eventRouter.Subscribe<MouseEvent>(HandleMouseEvent);

        // 4. Initialize input handler
        inputHandler.EventReceived += OnInputEventReceived;
        inputHandler.Start();

        // 5. Initialize plugins
        foreach (var plugin in plugins)
        {
            await plugin.InitializeAsync(this, cancellationToken);
        }

        // 6. Perform initial render
        if (rootWidget != null)
        {
            await renderEngine.RenderFrameAsync(rootWidget, cancellationToken);
        }

        currentState = ApplicationState.Running;
    }
    catch (Exception ex)
    {
        currentState = ApplicationState.Error;
        throw new ApplicationInitializationException("Failed to initialize application", ex);
    }
}

private async Task ShutdownAsync()
{
    if (currentState == ApplicationState.Stopped)
        return;

    currentState = ApplicationState.Stopping;

    try
    {
        // 1. Stop input handler
        inputHandler?.Stop();

        // 2. Shutdown plugins
        if (plugins != null)
        {
            foreach (var plugin in plugins.Reverse())
            {
                try
                {
                    await plugin.ShutdownAsync(this);
                }
                catch (Exception ex)
                {
                    // Log plugin shutdown errors but continue
                    await LogExceptionAsync($"Plugin {plugin.Name} shutdown failed", ex);
                }
            }
        }

        // 3. Restore terminal state
        if (terminalController != null)
        {
            await terminalController.ShowCursorAsync();
            if (options.EnableMouse)
                await terminalController.DisableMouseCaptureAsync();
            if (options.EnableAlternateScreen)
                await terminalController.ExitAlternateScreenAsync();
            await terminalController.DisableRawModeAsync();
        }

        // 4. Dispose resources
        renderEngine?.Dispose();
        terminalController?.Dispose();
        inputHandler?.Dispose();
        eventQueue?.Dispose();

        currentState = ApplicationState.Stopped;
    }
    catch (Exception ex)
    {
        // Log shutdown errors but don't throw
        await LogExceptionAsync("Application shutdown error", ex);
        currentState = ApplicationState.Error;
    }
}
```

### State and Widget Management

```csharp
public class Application
{
    // Root widget management
    public void SetRootWidget<TWidget>(TWidget widget) where TWidget : IWidget
    {
        rootWidget = widget ?? throw new ArgumentNullException(nameof(widget));
        RequestRender();
    }

    public TWidget GetRootWidget<TWidget>() where TWidget : class, IWidget
        => rootWidget as TWidget;

    // Global state management
    private readonly ConcurrentDictionary<string, object> globalState = new();

    public void SetGlobalState<T>(string key, T value)
    {
        globalState.AddOrUpdate(key, value, (_, _) => value);
        OnGlobalStateChanged(new GlobalStateChangedEventArgs(key, value));
    }

    public T GetGlobalState<T>(string key)
        => globalState.TryGetValue(key, out var value) ? (T)value : default(T);

    public bool TryGetGlobalState<T>(string key, out T value)
    {
        if (globalState.TryGetValue(key, out var objValue) && objValue is T typedValue)
        {
            value = typedValue;
            return true;
        }
        value = default(T);
        return false;
    }

    // Widget state management for stateful widgets
    private readonly ConcurrentDictionary<object, object> widgetStates = new();

    public void SetWidgetState<TWidget, TState>(TWidget widget, TState state)
        where TWidget : IStatefulWidget<TState>
    {
        widgetStates.AddOrUpdate(widget, state, (_, _) => state);
    }

    public TState GetWidgetState<TWidget, TState>(TWidget widget)
        where TWidget : IStatefulWidget<TState>
    {
        return widgetStates.TryGetValue(widget, out var state)
            ? (TState)state
            : widget.CreateDefaultState();
    }

    // Render management
    public void RequestRender()
    {
        renderEngine?.RequestRender();
    }
}
```

## Testing Approach

1. **Unit Tests**: Test application lifecycle, state management, and API methods
2. **Integration Tests**: Test coordination between all major subsystems
3. **Error Handling Tests**: Verify graceful handling of initialization and runtime failures
4. **Performance Tests**: Measure application startup time and resource usage
5. **Thread Safety Tests**: Verify safe concurrent access to application state
6. **Plugin System Tests**: Test plugin loading, initialization, and shutdown
7. **Builder Pattern Tests**: Verify correct application configuration through builder

## Related Components

- `src/CycoTui/Application.cs` - Main application class
- `src/CycoTui/ApplicationBuilder.cs` - Fluent configuration API
- `src/CycoTui/ApplicationOptions.cs` - Configuration options
- `src/CycoTui/ApplicationState.cs` - Application lifecycle state
- `src/CycoTui/Plugins/IApplicationPlugin.cs` - Plugin system interface

## Integration Points

- **Terminal Controller**: Manages low-level terminal operations
- **Render Engine**: Coordinates widget rendering and display updates
- **Event System**: Processes and routes user input events
- **Widget System**: Hosts and manages the root widget and its children
- **Plugin System**: Extends application functionality through plugins

## Performance Considerations

- **Startup Time**: Minimize initialization overhead for fast application startup
- **Memory Usage**: Efficient state management to avoid memory leaks
- **Thread Safety**: Use concurrent collections for safe multi-threaded access
- **Resource Cleanup**: Ensure proper disposal of all resources during shutdown
- **Event Processing**: Efficient event routing and handling without blocking

## Platform-Specific Details

### Error Recovery Strategies
- **Terminal Restoration**: Always attempt to restore terminal state, even after crashes
- **Plugin Isolation**: Plugin failures should not crash the entire application
- **Resource Leaks**: Prevent resource leaks through proper dispose patterns
- **Signal Handling**: Handle process termination signals gracefully

## Acceptance Criteria

- [ ] `Application` class implemented with complete lifecycle management
- [ ] Factory methods providing simple application creation patterns
- [ ] `ApplicationBuilder` supporting complex application configuration
- [ ] Synchronous and asynchronous execution methods working correctly
- [ ] Graceful shutdown with configurable timeout implemented
- [ ] Root widget management with dynamic widget replacement
- [ ] Global state management with thread-safe access
- [ ] Widget state management for stateful widgets
- [ ] Event subscription and handling mechanisms working
- [ ] Plugin system integration with proper lifecycle management
- [ ] Error handling with graceful recovery and logging
- [ ] Resource cleanup ensuring no leaks during shutdown
- [ ] Cross-platform compatibility verified on Windows, macOS, and Linux
- [ ] Performance targets met (startup <100ms, minimal memory usage)
- [ ] Integration tests passing with all major subsystems
- [ ] Documentation and examples for common usage patterns

## See Also

- [SPEC-APPLAYER-001.md](../../specs/SPEC-APPLAYER-001.md): Application framework specification
- [APPLAYER-TERMINAL-001](../APPLAYER-TERMINAL-001/README.md): Terminal control implementation
- [APPLAYER-RENDERER-001](../APPLAYER-RENDERER-001/README.md): Rendering engine implementation
- [APPLAYER-EVENTLOOP-001](../APPLAYER-EVENTLOOP-001/README.md): Event loop implementation
- [APPLAYER-M6-001.md](../../roadmap/APPLAYER-M6-001.md): Application layer roadmap