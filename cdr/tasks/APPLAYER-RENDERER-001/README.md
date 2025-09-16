# Rendering Engine Implementation

## Overview

This task implements the rendering engine that connects widget layouts to actual terminal output. The rendering engine coordinates the widget system, buffer management, diffing algorithms, and terminal control to produce efficient, flicker-free updates to the terminal display.

## Implementation Approach

1. **Create Rendering Pipeline Architecture**:
   - Implement `IRenderEngine` interface for coordinating the render process
   - Create render context that provides widgets access to layout areas and styling
   - Build frame-based rendering system with double buffering
   - Integrate with existing buffer diffing algorithms for efficient updates

2. **Implement Widget Render Coordination**:
   - Create widget rendering visitor pattern for traversing widget trees
   - Handle stateful widget rendering with state management integration
   - Implement render area clipping and bounds checking
   - Support nested widget rendering with proper coordinate transformation

3. **Build Buffer Management System**:
   - Integrate with existing Buffer and Cell implementations
   - Implement double-buffering with automatic buffer swapping
   - Create buffer pooling system to minimize allocation overhead
   - Handle buffer resizing during terminal size changes

4. **Create Terminal Output Integration**:
   - Connect buffer diff results to terminal control commands
   - Implement efficient ANSI sequence generation for style changes
   - Handle cursor positioning optimization to minimize terminal I/O
   - Support batch output operations for improved performance

## Key Challenges

1. **Performance Optimization**: Minimizing rendering overhead for smooth 60+ FPS operation
2. **Memory Management**: Efficient buffer allocation and reuse to reduce GC pressure
3. **Coordinate Transformation**: Proper handling of widget coordinates and clipping regions
4. **Style Optimization**: Minimizing redundant style changes in terminal output
5. **Error Recovery**: Graceful handling of widget rendering failures

## Implementation Notes

### Render Engine Interface

```csharp
public interface IRenderEngine : IDisposable
{
    // Core rendering operations
    Task<RenderResult> RenderFrameAsync(IWidget rootWidget, CancellationToken cancellationToken = default);
    Task<RenderResult> RenderFrameAsync<TState>(IStatefulWidget<TState> rootWidget, ref TState state, CancellationToken cancellationToken = default);

    // Frame management
    void RequestRender();
    bool HasPendingRenders { get; }

    // Buffer access for inspection
    Buffer CurrentBuffer { get; }
    Buffer PreviousBuffer { get; }

    // Performance metrics
    RenderMetrics GetMetrics();
    void ResetMetrics();

    // Events
    event EventHandler<FrameRenderedEventArgs> FrameRendered;
    event EventHandler<RenderErrorEventArgs> RenderError;
}

public class RenderResult
{
    public bool Success { get; set; }
    public TimeSpan RenderTime { get; set; }
    public int CellsChanged { get; set; }
    public int BufferOperations { get; set; }
    public Exception Error { get; set; }
}
```

### Widget Render Context

```csharp
public class RenderContext
{
    private readonly Buffer buffer;
    private readonly Rect clippingRect;
    private readonly Stack<Rect> clipStack;

    public Buffer Buffer => buffer;
    public Rect Area => clippingRect;
    public Position Origin { get; private set; }

    // Widget rendering methods
    public void RenderWidget(IWidget widget, Rect area);
    public void RenderStatefulWidget<TState>(IStatefulWidget<TState> widget, Rect area, ref TState state);

    // Buffer operations with clipping
    public void SetCell(Position position, Cell cell);
    public void SetString(Position position, string text, Style style);
    public void FillRect(Rect rect, Cell cell);
    public void ClearRect(Rect rect);

    // Coordinate transformation
    public Position ToAbsolute(Position relative);
    public Rect ToAbsolute(Rect relative);
    public bool IsVisible(Position position);
    public bool IsVisible(Rect rect);

    // Clipping management
    public IDisposable PushClip(Rect rect);
}
```

### Render Pipeline Implementation

```csharp
public class RenderEngine : IRenderEngine
{
    private readonly ITerminalController terminalController;
    private readonly BufferPool bufferPool;
    private Buffer currentBuffer;
    private Buffer previousBuffer;
    private readonly RenderMetrics metrics;
    private bool renderRequested;

    public async Task<RenderResult> RenderFrameAsync(IWidget rootWidget, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // 1. Prepare render buffers
            await PrepareBuffersAsync(cancellationToken);

            // 2. Create render context
            var context = new RenderContext(currentBuffer, currentBuffer.Area);

            // 3. Render widget tree
            await RenderWidgetTreeAsync(rootWidget, context, cancellationToken);

            // 4. Calculate buffer differences
            var diff = previousBuffer.Diff(currentBuffer);

            // 5. Apply changes to terminal
            await ApplyDiffToTerminalAsync(diff, cancellationToken);

            // 6. Swap buffers for next frame
            SwapBuffers();

            // 7. Update metrics and return result
            var result = new RenderResult
            {
                Success = true,
                RenderTime = stopwatch.Elapsed,
                CellsChanged = diff.Count(),
                BufferOperations = context.OperationCount
            };

            UpdateMetrics(result);
            OnFrameRendered(new FrameRenderedEventArgs(result));
            return result;
        }
        catch (Exception ex)
        {
            var errorResult = new RenderResult
            {
                Success = false,
                RenderTime = stopwatch.Elapsed,
                Error = ex
            };

            OnRenderError(new RenderErrorEventArgs(ex));
            return errorResult;
        }
    }

    private async Task RenderWidgetTreeAsync(IWidget widget, RenderContext context, CancellationToken cancellationToken)
    {
        // Handle different widget types
        switch (widget)
        {
            case IStatefulWidget statefulWidget:
                // Need state management integration
                var state = GetWidgetState(statefulWidget);
                statefulWidget.Render(context.Area, context.Buffer, ref state);
                SetWidgetState(statefulWidget, state);
                break;

            case IWidget regularWidget:
                regularWidget.Render(context.Area, context.Buffer);
                break;

            case IContainerWidget containerWidget:
                // Render container and then children
                containerWidget.Render(context.Area, context.Buffer);
                await RenderChildrenAsync(containerWidget, context, cancellationToken);
                break;
        }
    }
}
```

### Buffer Pool for Memory Efficiency

```csharp
public class BufferPool : IDisposable
{
    private readonly ConcurrentQueue<Buffer> availableBuffers;
    private readonly int maxPoolSize;
    private Size lastBufferSize;

    public Buffer Rent(Size size)
    {
        if (availableBuffers.TryDequeue(out var buffer) && buffer.Area.Size == size)
        {
            buffer.Clear();
            return buffer;
        }

        // Create new buffer if none available or size changed
        return Buffer.Empty(new Rect(Position.Origin, size));
    }

    public void Return(Buffer buffer)
    {
        if (availableBuffers.Count < maxPoolSize && buffer.Area.Size == lastBufferSize)
        {
            availableBuffers.Enqueue(buffer);
        }
        // Otherwise let GC handle it
    }
}
```

## Testing Approach

1. **Unit Tests**: Test rendering pipeline components in isolation
2. **Widget Integration Tests**: Verify correct rendering of widget hierarchies
3. **Performance Tests**: Measure rendering performance and memory usage
4. **Buffer Management Tests**: Test buffer pooling and memory efficiency
5. **Error Handling Tests**: Verify graceful handling of rendering failures
6. **Visual Tests**: Compare rendered output against expected results

## Related Components

- `src/CycoTui.Core/Rendering/IRenderEngine.cs` - Main rendering interface
- `src/CycoTui.Core/Rendering/RenderEngine.cs` - Core rendering implementation
- `src/CycoTui.Core/Rendering/RenderContext.cs` - Widget rendering context
- `src/CycoTui.Core/Rendering/BufferPool.cs` - Buffer memory management
- `src/CycoTui.Core/Rendering/RenderMetrics.cs` - Performance monitoring
- Integration with existing `Buffer`, `Cell`, and widget implementations

## Integration Points

- **Widget System**: All widgets must be compatible with the rendering context
- **Buffer System**: Uses existing buffer and diffing implementations
- **Terminal Control**: Coordinates with terminal controller for output
- **Application Framework**: Main Application class orchestrates rendering cycles
- **Layout System**: Receives layout-calculated widget positions and sizes

## Performance Considerations

- **Buffer Pooling**: Reuse buffers to minimize GC allocations
- **Diff Optimization**: Skip unchanged buffer regions to reduce terminal I/O
- **Batch Operations**: Group terminal operations to minimize system calls
- **Asynchronous Rendering**: Support async widget rendering for I/O-bound widgets
- **Metrics Collection**: Track performance data without impacting render speed

## Acceptance Criteria

- [ ] `IRenderEngine` interface implemented with full functionality
- [ ] Widget rendering pipeline supporting both stateful and stateless widgets
- [ ] Double-buffered rendering with efficient buffer management
- [ ] Integration with existing buffer diffing algorithms
- [ ] Terminal output integration with optimized ANSI sequence generation
- [ ] Buffer pooling system reducing memory allocation overhead
- [ ] Error handling with graceful recovery from widget render failures
- [ ] Performance metrics collection and reporting
- [ ] Support for nested widget rendering with proper clipping
- [ ] Coordinate transformation and bounds checking working correctly
- [ ] Frame rate targeting (60+ FPS) achieved in performance tests
- [ ] Memory usage remaining stable during extended rendering sessions
- [ ] Integration tests passing with various widget combinations
- [ ] Visual output matching expected results in test scenarios

## See Also

- [SPEC-APPLAYER-001.md](../../specs/SPEC-APPLAYER-001.md): Application framework specification
- [SPEC-BUFFER-002.md](../../specs/SPEC-BUFFER-002.md): Buffer and rendering model specification
- [SPEC-WIDGET-003.md](../../specs/SPEC-WIDGET-003.md): Widget system specification
- [APPLAYER-TERMINAL-001](../APPLAYER-TERMINAL-001/README.md): Terminal control implementation
- [APPLAYER-M6-001.md](../../roadmap/APPLAYER-M6-001.md): Application layer roadmap