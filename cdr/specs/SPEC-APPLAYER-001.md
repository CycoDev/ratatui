---
id: SPEC-APPLAYER-001
title: Application Framework and Terminal Integration Specification
status: draft
date: 2025-01-16
---

# Application Framework and Terminal Integration Specification

## Overview

The Application Framework specification defines the high-level API that integrates all CycoTui components (layouts, widgets, events, input) into a cohesive terminal application development experience. This specification covers the Application class, terminal lifecycle management, integrated event loop, and developer-facing API design.

This framework serves as the "glue" that connects the lower-level components and provides the primary entry point for developers building terminal applications with CycoTui.

## Scope

This specification covers:

- Application class structure and lifecycle management
- Terminal initialization and cleanup integration
- Integrated event loop coordinating input, updates, and rendering
- Application state management and widget orchestration
- High-level developer API design
- Cross-platform application deployment considerations
- Integration patterns for existing .NET CLI projects
- Performance optimization for application-level operations

## Requirements

### Application Class Structure

**Core Application Framework:**

```csharp
public class Application : IDisposable
{
    // Core state
    private readonly ITerminal terminal;
    private readonly EventRouter eventRouter;
    private readonly EventQueue eventQueue;
    private readonly IInputHandler inputHandler;
    private bool isRunning;
    private CancellationTokenSource cancellationTokenSource;

    // Application state
    private IWidget rootWidget;
    private ApplicationState state;
    private readonly Dictionary<string, object> globalState;

    // Event handling
    private readonly Dictionary<Type, Delegate> eventHandlers;
    private readonly List<IApplicationPlugin> plugins;

    // Lifecycle management
    public event EventHandler<ApplicationStartedEventArgs> Started;
    public event EventHandler<ApplicationStoppedEventArgs> Stopped;
    public event EventHandler<RenderFrameEventArgs> FrameRendered;
    public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;
}

public class ApplicationOptions
{
    public TerminalBackendType Backend { get; set; } = TerminalBackendType.Crossterm;
    public Viewport Viewport { get; set; } = Viewport.Fullscreen;
    public bool EnableMouse { get; set; } = true;
    public bool EnableResize { get; set; } = true;
    public bool EnableRawMode { get; set; } = true;
    public bool EnableAlternateScreen { get; set; } = true;
    public TimeSpan RenderInterval { get; set; } = TimeSpan.FromMilliseconds(16); // ~60 FPS
    public TimeSpan InputPollInterval { get; set; } = TimeSpan.FromMilliseconds(10);
}
```

**Application Factory Methods:**

```csharp
public static class Application
{
    // Simple application creation
    public static Application Create();
    public static Application Create(ApplicationOptions options);
    public static Application Create<TWidget>(TWidget rootWidget) where TWidget : IWidget;
    public static Application Create<TWidget>(TWidget rootWidget, ApplicationOptions options) where TWidget : IWidget;

    // Builder pattern for complex configuration
    public static ApplicationBuilder Builder();
}

public class ApplicationBuilder
{
    public ApplicationBuilder WithBackend(TerminalBackendType backend);
    public ApplicationBuilder WithBackend<TBackend>(TBackend backend) where TBackend : ITerminalBackend;
    public ApplicationBuilder WithRootWidget<TWidget>(TWidget widget) where TWidget : IWidget;
    public ApplicationBuilder WithViewport(Viewport viewport);
    public ApplicationBuilder EnableMouse(bool enable = true);
    public ApplicationBuilder EnableResize(bool enable = true);
    public ApplicationBuilder WithRenderRate(int fps);
    public ApplicationBuilder WithInputPollRate(TimeSpan interval);
    public ApplicationBuilder AddPlugin<TPlugin>(TPlugin plugin) where TPlugin : IApplicationPlugin;
    public ApplicationBuilder OnEvent<TEvent>(EventHandler<TEvent> handler) where TEvent : IEvent;
    public ApplicationBuilder OnException(EventHandler<UnhandledExceptionEventArgs> handler);

    public Application Build();
}
```

### Application Lifecycle Management

**Main Run Methods:**

```csharp
public class Application : IDisposable
{
    // Synchronous execution
    public void Run();
    public void Run(CancellationToken cancellationToken);

    // Asynchronous execution
    public Task RunAsync();
    public Task RunAsync(CancellationToken cancellationToken);

    // Single frame execution (for integration scenarios)
    public void Tick();
    public Task TickAsync();

    // Controlled shutdown
    public void Stop();
    public void Stop(TimeSpan timeout);

    // Lifecycle state
    public bool IsRunning { get; }
    public ApplicationState State { get; }
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

**Lifecycle Phases:**

1. **Initialization Phase:**
   - Terminal backend initialization
   - Event system setup (queue, router, handlers)
   - Input handler configuration
   - Plugin initialization
   - Root widget setup
   - Initial render

2. **Run Phase:**
   - Main event loop execution
   - Input processing
   - Event routing and handling
   - Widget updates
   - Frame rendering
   - Plugin update cycle

3. **Shutdown Phase:**
   - Event loop termination
   - Plugin cleanup
   - Terminal restoration
   - Resource disposal

### Integrated Event Loop

**Core Event Loop Implementation:**

```csharp
private async Task RunEventLoopAsync(CancellationToken cancellationToken)
{
    using var renderTimer = new Timer(RenderCallback, null, TimeSpan.Zero, options.RenderInterval);
    using var inputTimer = new Timer(InputCallback, null, TimeSpan.Zero, options.InputPollInterval);

    while (isRunning && !cancellationToken.IsCancellationRequested)
    {
        try
        {
            // Process pending events
            await ProcessEventsAsync(cancellationToken);

            // Update application state
            await UpdateStateAsync(cancellationToken);

            // Render frame if needed
            if (needsRender)
            {
                await RenderFrameAsync(cancellationToken);
                needsRender = false;
            }

            // Small delay to prevent CPU spinning
            await Task.Delay(1, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
            break;
        }
        catch (Exception ex)
        {
            await HandleUnhandledExceptionAsync(ex);
        }
    }
}

private async Task ProcessEventsAsync(CancellationToken cancellationToken)
{
    // Process all available events
    while (eventQueue.TryDequeue(out var @event))
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
```

**Event Loop Requirements:**

1. **Non-blocking Input Processing**: Input polling on separate thread/timer
2. **Frame Rate Control**: Configurable render rate with consistent timing
3. **Event Priority**: System events (resize, quit) get priority over user events
4. **Exception Resilience**: Event processing exceptions don't crash the application
5. **Graceful Shutdown**: Proper cleanup on cancellation or stop requests
6. **Plugin Integration**: Plugin lifecycle methods called at appropriate times

### Widget Integration and State Management

**Widget Management:**

```csharp
public class Application
{
    // Root widget management
    public void SetRootWidget<TWidget>(TWidget widget) where TWidget : IWidget;
    public TWidget GetRootWidget<TWidget>() where TWidget : class, IWidget;

    // Global state management
    public void SetGlobalState<T>(string key, T value);
    public T GetGlobalState<T>(string key);
    public bool TryGetGlobalState<T>(string key, out T value);
    public void RemoveGlobalState(string key);

    // Widget state management for stateful widgets
    public void SetWidgetState<TWidget, TState>(TWidget widget, TState state)
        where TWidget : IStatefulWidget<TState>;
    public TState GetWidgetState<TWidget, TState>(TWidget widget)
        where TWidget : IStatefulWidget<TState>;

    // Layout and focus management
    public void RequestRender();
    public void SetFocus<TWidget>(TWidget widget) where TWidget : IWidget;
    public IWidget GetFocusedWidget();

    // Event subscription at application level
    public void Subscribe<TEvent>(EventHandler<TEvent> handler) where TEvent : IEvent;
    public void Unsubscribe<TEvent>(EventHandler<TEvent> handler) where TEvent : IEvent;
}
```

**State Synchronization:**

- Thread-safe state access using concurrent collections
- Automatic render triggers on state changes
- Widget state persistence across renders
- Global state accessible to all widgets and plugins
- Event-driven state updates with proper synchronization

### Terminal Integration

**Terminal Lifecycle Integration:**

```csharp
private async Task InitializeTerminalAsync()
{
    try
    {
        // Create and configure backend
        var backend = BackendFactory.Create(options.Backend);
        terminal = Terminal.New(backend, new TerminalOptions
        {
            Viewport = options.Viewport
        });

        // Configure terminal modes
        if (options.EnableRawMode)
            await terminal.EnableRawModeAsync();

        if (options.EnableAlternateScreen)
            await terminal.EnterAlternateScreenAsync();

        if (options.EnableMouse)
            await terminal.EnableMouseCaptureAsync();

        // Hide cursor initially
        await terminal.HideCursorAsync();

        // Clear screen
        await terminal.ClearAsync();

        // Perform initial render
        await RenderFrameAsync(CancellationToken.None);
    }
    catch (Exception ex)
    {
        throw new ApplicationInitializationException("Failed to initialize terminal", ex);
    }
}

private async Task RestoreTerminalAsync()
{
    try
    {
        if (terminal != null)
        {
            // Restore cursor
            await terminal.ShowCursorAsync();

            // Exit alternate screen
            if (options.EnableAlternateScreen)
                await terminal.ExitAlternateScreenAsync();

            // Disable raw mode
            if (options.EnableRawMode)
                await terminal.DisableRawModeAsync();

            // Disable mouse capture
            if (options.EnableMouse)
                await terminal.DisableMouseCaptureAsync();

            terminal.Dispose();
        }
    }
    catch (Exception ex)
    {
        // Log but don't throw during cleanup
        await LogExceptionAsync("Terminal restore failed", ex);
    }
}
```

### Plugin System

**Plugin Interface:**

```csharp
public interface IApplicationPlugin : IDisposable
{
    string Name { get; }
    Version Version { get; }

    Task InitializeAsync(IApplication application, CancellationToken cancellationToken = default);
    Task UpdateAsync(IApplication application, CancellationToken cancellationToken = default);
    Task<bool> HandleEventAsync(IEvent @event, IApplication application, CancellationToken cancellationToken = default);
    Task ShutdownAsync(IApplication application, CancellationToken cancellationToken = default);
}

// Common plugin base class
public abstract class ApplicationPlugin : IApplicationPlugin
{
    public abstract string Name { get; }
    public virtual Version Version => Assembly.GetExecutingAssembly().GetName().Version;

    public virtual Task InitializeAsync(IApplication application, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public virtual Task UpdateAsync(IApplication application, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public virtual Task<bool> HandleEventAsync(IEvent @event, IApplication application, CancellationToken cancellationToken = default) => Task.FromResult(false);
    public virtual Task ShutdownAsync(IApplication application, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public virtual void Dispose() { }
}
```

**Built-in Plugins:**

1. **KeyboardShortcutsPlugin**: Global keyboard shortcut handling
2. **ResizeHandlerPlugin**: Terminal resize event management
3. **MouseHandlerPlugin**: Mouse event processing and widget targeting
4. **FocusManagerPlugin**: Widget focus management and navigation
5. **PerformanceMonitorPlugin**: Frame rate and performance monitoring
6. **LoggingPlugin**: Application event logging and debugging

### High-Level Developer API

**Simple Application Patterns:**

```csharp
// Example 1: Hello World application
public static async Task Main(string[] args)
{
    var app = Application.Create(new Paragraph("Hello, World!"));
    await app.RunAsync();
}

// Example 2: Interactive dashboard
public static async Task Main(string[] args)
{
    var dashboard = new Dashboard();

    var app = Application.Builder()
        .WithRootWidget(dashboard)
        .EnableMouse()
        .OnEvent<KeyEvent>(OnKeyPressed)
        .OnEvent<ResizeEvent>(OnResize)
        .Build();

    await app.RunAsync();
}

// Example 3: Integration with existing CLI app
public static async Task ShowInteractiveReport(ReportData data)
{
    var reportWidget = new InteractiveReport(data);

    using var app = Application.Create(reportWidget, new ApplicationOptions
    {
        Viewport = Viewport.Inline(25), // 25 lines high
        EnableAlternateScreen = false   // Don't take over entire terminal
    });

    await app.RunAsync();

    // Terminal automatically restored when app is disposed
    Console.WriteLine("Report viewing completed.");
}
```

### Cross-Platform Deployment

**Platform-Specific Considerations:**

```csharp
public static class ApplicationPlatform
{
    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    public static bool IsMacOS => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    public static ApplicationOptions GetPlatformDefaults()
    {
        var options = new ApplicationOptions();

        if (IsWindows)
        {
            // Windows-specific defaults
            options.Backend = TerminalBackendType.Crossterm; // Best Windows compatibility
            options.InputPollInterval = TimeSpan.FromMilliseconds(15); // Slightly slower on Windows
        }
        else if (IsMacOS)
        {
            // macOS-specific defaults
            options.Backend = TerminalBackendType.Unix;
            options.EnableMouse = true; // iTerm2/Terminal.app have good mouse support
        }
        else
        {
            // Linux/Unix defaults
            options.Backend = TerminalBackendType.Unix;
        }

        return options;
    }
}
```

### Integration Patterns

**Existing .NET CLI Project Integration:**

```csharp
// Pattern 1: Command-line switch for interactive mode
public static async Task Main(string[] args)
{
    if (args.Contains("--interactive"))
    {
        await RunInteractiveMode();
    }
    else
    {
        await RunCommandLineMode(args);
    }
}

// Pattern 2: Optional interactive fallback
public static async Task Main(string[] args)
{
    try
    {
        await RunCommandLineMode(args);
    }
    catch (InvalidOperationException) when (IsInteractiveTerminal())
    {
        Console.WriteLine("Falling back to interactive mode...");
        await RunInteractiveMode();
    }
}

// Pattern 3: Sub-command architecture
[Verb("interactive", HelpText = "Run in interactive terminal mode")]
public class InteractiveOptions
{
    [Option('m', "mouse", Default = true, HelpText = "Enable mouse support")]
    public bool EnableMouse { get; set; }
}

public static async Task RunInteractive(InteractiveOptions opts)
{
    var app = Application.Create(new MainMenuWidget(), new ApplicationOptions
    {
        EnableMouse = opts.EnableMouse
    });

    await app.RunAsync();
}
```

## Technical Approach

### Architecture Pattern

The Application Framework follows a layered architecture:

1. **Application Layer** (This specification)
   - High-level API and developer experience
   - Application lifecycle management
   - Plugin system coordination

2. **Integration Layer**
   - Event system integration
   - Widget system coordination
   - Terminal lifecycle management

3. **Core Services Layer**
   - Terminal, Buffer, Layout, Widget, Events
   - Cross-cutting concerns (logging, performance)

4. **Platform Layer**
   - Backend implementations
   - Platform-specific optimizations

### Event Flow Architecture

```
Input Events → Event Queue → Event Router → Application Handlers → Widget Handlers
                                    ↓
               Application State ← State Manager ← Event Handlers
                                    ↓
               Render Trigger → Frame Render → Terminal Output
```

### Performance Optimization Strategy

1. **Event Batching**: Process multiple events per frame to reduce latency
2. **Selective Rendering**: Only render frames when state changes occur
3. **Widget Caching**: Cache widget render results when possible
4. **Memory Pooling**: Reuse event objects and buffers
5. **Background Processing**: Use background threads for non-critical operations

### Error Handling Strategy

1. **Exception Isolation**: Prevent single widget/plugin errors from crashing app
2. **Graceful Degradation**: Continue operation when non-critical components fail
3. **Recovery Mechanisms**: Attempt to restore terminal state even after crashes
4. **Diagnostic Information**: Provide detailed error information for debugging

## Platform-Specific Details

### Windows Considerations

- **Console Mode Handling**: Proper setup of Windows Console modes for VT processing
- **ConPTY Integration**: Support for Windows Pseudo Console when available
- **Legacy Console**: Graceful fallback for older Windows versions
- **Unicode Support**: Ensure proper UTF-8/UTF-16 handling

### Unix/Linux Considerations

- **Signal Handling**: Proper handling of SIGWINCH (resize) and SIGTERM/SIGINT
- **Terminal Database**: Integration with terminfo for feature detection
- **Process Groups**: Proper terminal process group management
- **File Descriptor Management**: Efficient handling of stdin/stdout/stderr

### macOS Considerations

- **Terminal.app Integration**: Specific optimizations for macOS Terminal
- **iTerm2 Features**: Support for iTerm2-specific capabilities
- **Metal Performance**: Consider Metal-based rendering optimizations
- **Accessibility**: VoiceOver and accessibility framework integration

## Performance Targets

- **Application Startup**: <100ms from Main() to first render
- **Event Processing**: <1ms latency for keyboard events
- **Frame Rate**: Maintain 60 FPS during active interaction
- **Memory Usage**: <50MB for typical applications
- **CPU Usage**: <5% during idle, <25% during active rendering

## Examples

### Basic Hello World

```csharp
using CycoTui;

var app = Application.Create(new Text("Hello, World!"));
await app.RunAsync();
```

### Interactive Dashboard

```csharp
using CycoTui;
using CycoTui.Widgets;

public class DashboardApp
{
    public static async Task Main(string[] args)
    {
        var dashboard = new Layout(Direction.Horizontal)
        {
            new Panel("System Info")
            {
                new SystemInfoWidget()
            },
            new Panel("Metrics")
            {
                new Chart(GetMetricsData())
            }
        };

        var app = Application.Builder()
            .WithRootWidget(dashboard)
            .EnableMouse()
            .OnEvent<KeyEvent>(HandleKey)
            .OnEvent<ResizeEvent>(HandleResize)
            .Build();

        await app.RunAsync();
    }

    private static Task HandleKey(KeyEvent keyEvent)
    {
        if (keyEvent.Key == Key.Q)
        {
            Application.Current.Stop();
        }
        return Task.CompletedTask;
    }

    private static Task HandleResize(ResizeEvent resizeEvent)
    {
        Application.Current.RequestRender();
        return Task.CompletedTask;
    }
}
```

## See Also

- `SPEC-TERMINAL-007.md` - Terminal implementation specification
- `SPEC-BACKEND-001.md` - Backend interface specification
- `INPUT-M5-001.md` - Input handling and event system roadmap
- `SPEC-WIDGET-003.md` - Widget system specification
- `SPEC-LAYOUT-004.md` - Layout engine specification