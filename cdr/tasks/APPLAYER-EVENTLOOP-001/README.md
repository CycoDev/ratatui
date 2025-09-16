# Integrated Event Loop Implementation

## Overview

This task implements the integrated event loop that coordinates input processing, event routing, application state updates, and frame rendering within the main application lifecycle. The event loop serves as the heartbeat of the application, orchestrating all real-time operations with precise timing and error resilience.

## Implementation Approach

1. **Create Event Loop Architecture**:
   - Implement main event loop with configurable timing and frame rate control
   - Create separate input polling and rendering cycles for optimal performance
   - Build event processing pipeline with priority queuing and batching
   - Support both blocking and non-blocking operation modes

2. **Implement Input Processing Integration**:
   - Coordinate with input handlers to collect keyboard, mouse, and resize events
   - Process input events asynchronously to prevent UI blocking
   - Implement event batching to handle high-frequency input efficiently
   - Support input throttling and debouncing for smooth interaction

3. **Build State Update Coordination**:
   - Manage application and widget state updates triggered by events
   - Coordinate state changes with rendering cycles to prevent conflicts
   - Implement state change batching for efficient updates
   - Support rollback mechanisms for failed state transitions

4. **Create Rendering Cycle Management**:
   - Integrate with rendering engine for frame-based display updates
   - Implement adaptive frame rate based on activity and performance
   - Support render-on-demand for idle applications to reduce CPU usage
   - Handle terminal resize events with buffer reallocation

## Key Challenges

1. **Timing Coordination**: Balancing input responsiveness with rendering performance
2. **Thread Safety**: Coordinating potentially multi-threaded event processing
3. **Error Recovery**: Maintaining stable operation when individual components fail
4. **Resource Management**: Preventing memory leaks and resource exhaustion
5. **Performance Optimization**: Achieving 60+ FPS while maintaining low CPU usage

## Implementation Notes

### Event Loop Architecture

```csharp
public class ApplicationEventLoop : IDisposable
{
    private readonly Application application;
    private readonly IInputHandler inputHandler;
    private readonly IRenderEngine renderEngine;
    private readonly EventQueue eventQueue;
    private readonly EventRouter eventRouter;

    // Timing control
    private readonly Timer renderTimer;
    private readonly Timer inputTimer;
    private readonly ApplicationOptions options;

    // State management
    private volatile bool isRunning;
    private CancellationTokenSource cancellationTokenSource;
    private Task mainLoopTask;

    // Performance tracking
    private readonly EventLoopMetrics metrics;
    private DateTime lastFrameTime;
    private readonly Queue<TimeSpan> frameTimes;

    public bool IsRunning => isRunning;
    public EventLoopMetrics Metrics => metrics;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (isRunning)
            throw new InvalidOperationException("Event loop is already running");

        isRunning = true;
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        try
        {
            await InitializeAsync(cancellationTokenSource.Token);
            mainLoopTask = RunMainLoopAsync(cancellationTokenSource.Token);
            await mainLoopTask;
        }
        finally
        {
            isRunning = false;
            await ShutdownAsync();
        }
    }

    public async Task StopAsync(TimeSpan timeout = default)
    {
        if (!isRunning)
            return;

        cancellationTokenSource.Cancel();

        var timeoutTask = timeout == default
            ? Task.Delay(Timeout.Infinite)
            : Task.Delay(timeout);

        var completed = await Task.WhenAny(mainLoopTask, timeoutTask);
        if (completed == timeoutTask)
        {
            throw new TimeoutException("Event loop did not stop within the specified timeout");
        }
    }
}
```

### Main Event Loop Implementation

```csharp
private async Task RunMainLoopAsync(CancellationToken cancellationToken)
{
    var frameStopwatch = Stopwatch.StartNew();
    var nextRenderTime = DateTime.UtcNow;
    var renderInterval = options.RenderInterval;

    while (!cancellationToken.IsCancellationRequested)
    {
        var frameStart = DateTime.UtcNow;

        try
        {
            // 1. Process input events (non-blocking)
            await ProcessInputEventsAsync(cancellationToken);

            // 2. Route and handle application events
            await ProcessApplicationEventsAsync(cancellationToken);

            // 3. Update plugin systems
            await UpdatePluginsAsync(cancellationToken);

            // 4. Render frame if needed or scheduled
            if (ShouldRender(frameStart, nextRenderTime))
            {
                await RenderFrameAsync(cancellationToken);
                nextRenderTime = frameStart.Add(renderInterval);
                UpdateFrameMetrics(frameStart);
            }

            // 5. Sleep until next cycle (adaptive timing)
            await SleepUntilNextCycleAsync(frameStart, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Expected during shutdown
            break;
        }
        catch (Exception ex)
        {
            // Handle unexpected errors without crashing
            await HandleEventLoopExceptionAsync(ex);

            // Brief pause to prevent error storms
            await Task.Delay(10, cancellationToken);
        }
    }
}

private async Task ProcessInputEventsAsync(CancellationToken cancellationToken)
{
    // Process all available input events without blocking
    var processedCount = 0;
    var maxBatchSize = options.MaxInputBatchSize;

    while (inputHandler.HasPendingEvents && processedCount < maxBatchSize)
    {
        var inputEvent = await inputHandler.GetNextEventAsync(TimeSpan.Zero, cancellationToken);
        if (inputEvent == null)
            break;

        // Add to event queue for processing
        eventQueue.Enqueue(inputEvent);
        processedCount++;
    }

    metrics.InputEventsProcessed += processedCount;
}

private async Task ProcessApplicationEventsAsync(CancellationToken cancellationToken)
{
    var processedCount = 0;
    var maxBatchSize = options.MaxEventBatchSize;

    // Process events in priority order
    while (eventQueue.TryDequeue(out var @event) && processedCount < maxBatchSize)
    {
        try
        {
            // Route event through application-level handlers first
            var handled = await RouteApplicationEventAsync(@event, cancellationToken);

            // If not handled at application level, route to widgets
            if (!handled && !@event.IsHandled)
            {
                await RouteWidgetEventAsync(@event, cancellationToken);
            }

            processedCount++;
        }
        catch (Exception ex)
        {
            // Log event processing error but continue
            await LogEventProcessingErrorAsync(@event, ex);
        }
    }

    metrics.ApplicationEventsProcessed += processedCount;
}
```

### Adaptive Rendering and Timing

```csharp
private bool ShouldRender(DateTime currentTime, DateTime nextScheduledRender)
{
    // Always render if explicitly requested
    if (renderEngine.HasPendingRenders)
        return true;

    // Render if we've reached the scheduled time
    if (currentTime >= nextScheduledRender)
        return true;

    // Render if significant state changes have occurred
    if (HasSignificantStateChanges())
        return true;

    // Skip rendering for idle applications
    return false;
}

private async Task SleepUntilNextCycleAsync(DateTime frameStart, CancellationToken cancellationToken)
{
    var frameTime = DateTime.UtcNow - frameStart;
    var targetCycleTime = options.MinCycleTime;

    if (frameTime < targetCycleTime)
    {
        var sleepTime = targetCycleTime - frameTime;
        await Task.Delay(sleepTime, cancellationToken);
    }
    else if (frameTime > options.MaxFrameTime)
    {
        // Frame took too long, consider reducing quality or increasing intervals
        await AdaptPerformanceSettingsAsync(frameTime);
    }
}

private async Task AdaptPerformanceSettingsAsync(TimeSpan frameTime)
{
    // Adaptive performance tuning
    if (frameTime > TimeSpan.FromMilliseconds(33)) // >30 FPS
    {
        // Reduce render rate temporarily
        options.RenderInterval = TimeSpan.FromMilliseconds(Math.Min(50, options.RenderInterval.TotalMilliseconds * 1.1));

        // Reduce input polling frequency
        options.InputPollInterval = TimeSpan.FromMilliseconds(Math.Min(20, options.InputPollInterval.TotalMilliseconds * 1.1));

        metrics.PerformanceAdaptations++;
    }
    else if (frameTime < TimeSpan.FromMilliseconds(10)) // Very fast frames
    {
        // Can increase quality/frequency
        options.RenderInterval = TimeSpan.FromMilliseconds(Math.Max(16, options.RenderInterval.TotalMilliseconds * 0.9));
        options.InputPollInterval = TimeSpan.FromMilliseconds(Math.Max(5, options.InputPollInterval.TotalMilliseconds * 0.9));
    }
}
```

### Error Handling and Recovery

```csharp
private async Task HandleEventLoopExceptionAsync(Exception exception)
{
    metrics.ErrorsEncountered++;

    // Different strategies based on exception type
    switch (exception)
    {
        case RenderingException renderEx:
            // Try to reset rendering system
            await RecoverFromRenderingErrorAsync(renderEx);
            break;

        case InputException inputEx:
            // Reset input handler
            await RecoverFromInputErrorAsync(inputEx);
            break;

        case TerminalException terminalEx:
            // May need to reinitialize terminal
            await RecoverFromTerminalErrorAsync(terminalEx);
            break;

        default:
            // General error handling
            await application.NotifyUnhandledExceptionAsync(exception);
            break;
    }
}

private async Task RecoverFromRenderingErrorAsync(RenderingException exception)
{
    try
    {
        // Clear render queues
        renderEngine.ClearPendingRenders();

        // Force buffer recreation
        await renderEngine.RecreateBuffersAsync();

        // Request full screen refresh
        renderEngine.RequestFullRedraw();

        metrics.RenderingRecoveries++;
    }
    catch (Exception recoveryEx)
    {
        // Recovery failed, escalate
        await application.NotifyUnhandledExceptionAsync(new AggregateException(exception, recoveryEx));
    }
}
```

### Performance Metrics and Monitoring

```csharp
public class EventLoopMetrics
{
    public int FramesRendered { get; set; }
    public int InputEventsProcessed { get; set; }
    public int ApplicationEventsProcessed { get; set; }
    public int ErrorsEncountered { get; set; }
    public int PerformanceAdaptations { get; set; }
    public int RenderingRecoveries { get; set; }

    public double AverageFrameTime => frameTimes.Count > 0 ? frameTimes.Average() : 0;
    public double CurrentFPS => AverageFrameTime > 0 ? 1000.0 / AverageFrameTime : 0;
    public TimeSpan TotalRuntime { get; set; }

    private readonly Queue<double> frameTimes = new(capacity: 60); // Last 60 frames

    public void UpdateFrameTime(TimeSpan frameTime)
    {
        if (frameTimes.Count >= 60)
            frameTimes.Dequeue();

        frameTimes.Enqueue(frameTime.TotalMilliseconds);
        FramesRendered++;
    }

    public void Reset()
    {
        FramesRendered = 0;
        InputEventsProcessed = 0;
        ApplicationEventsProcessed = 0;
        ErrorsEncountered = 0;
        PerformanceAdaptations = 0;
        RenderingRecoveries = 0;
        frameTimes.Clear();
        TotalRuntime = TimeSpan.Zero;
    }
}
```

## Testing Approach

1. **Unit Tests**: Test event loop components and timing logic in isolation
2. **Integration Tests**: Test coordination between input, events, and rendering
3. **Performance Tests**: Measure frame rates, latency, and resource usage
4. **Stress Tests**: Test with high-frequency input and complex rendering scenarios
5. **Error Recovery Tests**: Verify graceful handling of component failures
6. **Timing Tests**: Verify frame rate targeting and adaptive timing behavior
7. **Long-running Tests**: Test stability and memory usage over extended periods

## Related Components

- `src/CycoTui.Core/EventLoop/ApplicationEventLoop.cs` - Main event loop implementation
- `src/CycoTui.Core/EventLoop/EventLoopMetrics.cs` - Performance monitoring
- `src/CycoTui.Core/EventLoop/EventLoopOptions.cs` - Configuration options
- Integration with `Application`, `IRenderEngine`, `EventQueue`, and `IInputHandler`

## Integration Points

- **Application Class**: Main Application uses event loop for its Run/RunAsync methods
- **Input System**: Coordinates with input handlers for event collection
- **Event System**: Processes events through the routing and handling pipeline
- **Rendering System**: Triggers frame renders based on timing and state changes
- **Plugin System**: Coordinates plugin update cycles within the event loop

## Performance Considerations

- **Adaptive Timing**: Automatically adjusts timing based on system performance
- **Event Batching**: Process multiple events per cycle to reduce overhead
- **Non-blocking Operations**: Prevent any single operation from blocking the loop
- **Memory Management**: Minimize allocations in the hot path
- **CPU Usage**: Implement idle detection to reduce CPU usage when inactive

## Acceptance Criteria

- [ ] Event loop architecture implemented with main loop coordination
- [ ] Input processing integration with non-blocking event collection
- [ ] Event routing and handling pipeline with error isolation
- [ ] Frame rendering coordination with adaptive timing
- [ ] Error handling and recovery mechanisms for all major components
- [ ] Performance metrics collection and reporting
- [ ] Adaptive performance tuning based on system performance
- [ ] Support for both high-performance and power-efficient operation modes
- [ ] Thread-safe coordination of all subsystem interactions
- [ ] Graceful startup and shutdown with proper resource cleanup
- [ ] Frame rate targeting (60+ FPS) achieved in performance tests
- [ ] Input latency under 10ms in responsive scenarios
- [ ] Stable operation during stress tests and long-running scenarios
- [ ] Integration tests passing with all major application components
- [ ] Error recovery tests demonstrating system resilience

## See Also

- [SPEC-APPLAYER-001.md](../../specs/SPEC-APPLAYER-001.md): Application framework specification
- [APPLAYER-APP-001](../APPLAYER-APP-001/README.md): Main application class implementation
- [APPLAYER-RENDERER-001](../APPLAYER-RENDERER-001/README.md): Rendering engine implementation
- [INPUT-M5-001.md](../../roadmap/INPUT-M5-001.md): Input handling system roadmap
- [APPLAYER-M6-001.md](../../roadmap/APPLAYER-M6-001.md): Application layer roadmap