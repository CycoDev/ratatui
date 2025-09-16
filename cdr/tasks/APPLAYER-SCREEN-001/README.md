# Screen Buffer Management Implementation

## Overview

This task implements the screen buffer management system that handles double buffering, buffer allocation, resizing operations, and memory optimization for efficient terminal rendering. The screen buffer system provides the foundation for flicker-free updates and optimal memory usage during application execution.

## Implementation Approach

1. **Create Screen Buffer Architecture**:
   - Implement `IScreenBufferManager` interface for buffer lifecycle management
   - Create double-buffered rendering system with automatic buffer swapping
   - Build buffer allocation strategies optimized for different usage patterns
   - Support multiple viewport configurations and buffer sizing

2. **Implement Buffer Pool Management**:
   - Create buffer pool for reusing buffer instances to reduce GC pressure
   - Implement size-based buffer categorization for efficient allocation
   - Build buffer cleanup and recycling mechanisms
   - Support dynamic pool sizing based on application memory constraints

3. **Build Resize and Viewport Handling**:
   - Handle terminal resize events with efficient buffer reallocation
   - Support content preservation during resize operations
   - Implement viewport-specific buffer management (fullscreen, inline, fixed)
   - Create smooth transitions during viewport changes

4. **Create Memory Optimization System**:
   - Implement memory usage monitoring and adaptive allocation strategies
   - Build buffer compression and compaction mechanisms for large applications
   - Create memory pressure response system with intelligent buffer management
   - Support configurable memory limits and cleanup policies

## Key Challenges

1. **Memory Efficiency**: Minimizing memory usage while maintaining performance
2. **Resize Handling**: Efficient buffer reallocation during frequent size changes
3. **Buffer Coordination**: Synchronizing buffer swaps with rendering cycles
4. **Memory Pressure**: Graceful degradation when system memory is constrained
5. **Thread Safety**: Safe concurrent access to buffer management operations

## Implementation Notes

### Screen Buffer Manager Interface

```csharp
public interface IScreenBufferManager : IDisposable
{
    // Buffer lifecycle
    BufferPair GetBuffers(Size size);
    BufferPair GetBuffers(Rect area);
    void SwapBuffers();
    void ClearBuffers();

    // Buffer pool management
    void ReturnBuffers(BufferPair buffers);
    void PreallocateBuffers(Size size, int count);
    int GetPoolSize();
    void TrimPool(int targetSize);

    // Resize operations
    BufferPair ResizeBuffers(Size newSize);
    BufferPair ResizeBuffers(Size newSize, bool preserveContent);
    void HandleTerminalResize(Size oldSize, Size newSize);

    // Memory management
    long GetMemoryUsage();
    void SetMemoryLimit(long maxBytes);
    bool IsMemoryPressureHigh();
    void CompactMemory();

    // Events
    event EventHandler<BufferResizedEventArgs> BufferResized;
    event EventHandler<MemoryPressureEventArgs> MemoryPressureChanged;
}

public struct BufferPair
{
    public Buffer Current { get; }
    public Buffer Previous { get; }
    public bool IsValid => Current != null && Previous != null;

    public BufferPair(Buffer current, Buffer previous)
    {
        Current = current;
        Previous = previous;
    }

    public void Swap() => (Current, Previous) = (Previous, Current);
}
```

### Screen Buffer Manager Implementation

```csharp
public class ScreenBufferManager : IScreenBufferManager
{
    private readonly BufferPool bufferPool;
    private readonly MemoryMonitor memoryMonitor;
    private readonly object bufferLock = new();

    // Current buffer state
    private BufferPair currentBuffers;
    private Size currentSize;
    private bool buffersValid;

    // Configuration
    private long memoryLimit = 100 * 1024 * 1024; // 100MB default
    private int maxPoolSize = 10;
    private readonly ScreenBufferOptions options;

    public event EventHandler<BufferResizedEventArgs> BufferResized;
    public event EventHandler<MemoryPressureEventArgs> MemoryPressureChanged;

    public ScreenBufferManager(ScreenBufferOptions options = null)
    {
        this.options = options ?? new ScreenBufferOptions();
        bufferPool = new BufferPool(this.options.PoolOptions);
        memoryMonitor = new MemoryMonitor(this.options.MemoryOptions);

        memoryMonitor.MemoryPressureChanged += OnMemoryPressureChanged;
    }

    public BufferPair GetBuffers(Size size)
    {
        lock (bufferLock)
        {
            if (buffersValid && currentSize == size)
            {
                return currentBuffers;
            }

            // Need new buffers
            if (buffersValid)
            {
                ReturnBuffersToPool(currentBuffers);
            }

            currentBuffers = AllocateBuffers(size);
            currentSize = size;
            buffersValid = true;

            return currentBuffers;
        }
    }

    public BufferPair ResizeBuffers(Size newSize, bool preserveContent = false)
    {
        lock (bufferLock)
        {
            if (!buffersValid || currentSize == newSize)
            {
                return GetBuffers(newSize);
            }

            var oldBuffers = currentBuffers;
            var oldSize = currentSize;

            // Allocate new buffers
            var newBuffers = AllocateBuffers(newSize);

            // Preserve content if requested
            if (preserveContent && oldBuffers.IsValid)
            {
                CopyBufferContent(oldBuffers.Current, newBuffers.Current);
                CopyBufferContent(oldBuffers.Previous, newBuffers.Previous);
            }

            // Update state
            currentBuffers = newBuffers;
            currentSize = newSize;

            // Return old buffers to pool
            ReturnBuffersToPool(oldBuffers);

            // Notify listeners
            OnBufferResized(new BufferResizedEventArgs(oldSize, newSize, preserveContent));

            return currentBuffers;
        }
    }

    private BufferPair AllocateBuffers(Size size)
    {
        var area = new Rect(Position.Origin, size);

        // Try to get buffers from pool first
        var current = bufferPool.Rent(area);
        var previous = bufferPool.Rent(area);

        if (current == null || previous == null)
        {
            // Pool allocation failed, create new buffers
            current?.Dispose();
            previous?.Dispose();

            // Check memory pressure before allocation
            if (IsMemoryPressureHigh() && !TryReduceMemoryUsage())
            {
                throw new OutOfMemoryException("Cannot allocate buffers due to memory pressure");
            }

            current = Buffer.Empty(area);
            previous = Buffer.Empty(area);
        }

        return new BufferPair(current, previous);
    }

    public void SwapBuffers()
    {
        lock (bufferLock)
        {
            if (buffersValid)
            {
                currentBuffers.Swap();
            }
        }
    }
}
```

### Buffer Pool Implementation

```csharp
public class BufferPool : IDisposable
{
    private readonly Dictionary<Size, Queue<Buffer>> pools = new();
    private readonly ReaderWriterLockSlim poolLock = new();
    private readonly BufferPoolOptions options;
    private volatile int totalBuffersInPool;

    public BufferPool(BufferPoolOptions options = null)
    {
        this.options = options ?? new BufferPoolOptions();
    }

    public Buffer Rent(Rect area)
    {
        var size = area.Size;

        poolLock.EnterReadLock();
        try
        {
            if (pools.TryGetValue(size, out var pool) && pool.Count > 0)
            {
                poolLock.ExitReadLock();
                poolLock.EnterWriteLock();
                try
                {
                    if (pool.Count > 0)
                    {
                        var buffer = pool.Dequeue();
                        totalBuffersInPool--;
                        buffer.Clear(); // Reset buffer content
                        return buffer;
                    }
                }
                finally
                {
                    poolLock.ExitWriteLock();
                }
                poolLock.EnterReadLock();
            }
        }
        finally
        {
            poolLock.ExitReadLock();
        }

        // No buffer available in pool, caller should allocate new one
        return null;
    }

    public bool Return(Buffer buffer)
    {
        if (buffer == null)
            return false;

        var size = buffer.Area.Size;

        poolLock.EnterWriteLock();
        try
        {
            // Check pool size limits
            if (totalBuffersInPool >= options.MaxPoolSize)
            {
                return false; // Pool is full, let buffer be GC'd
            }

            if (!pools.TryGetValue(size, out var pool))
            {
                pool = new Queue<Buffer>();
                pools[size] = pool;
            }

            // Check per-size pool limits
            if (pool.Count >= options.MaxPoolSizePerSize)
            {
                return false;
            }

            pool.Enqueue(buffer);
            totalBuffersInPool++;
            return true;
        }
        finally
        {
            poolLock.ExitWriteLock();
        }
    }

    public void Trim(int targetSize)
    {
        poolLock.EnterWriteLock();
        try
        {
            var toRemove = totalBuffersInPool - targetSize;
            if (toRemove <= 0)
                return;

            // Remove buffers starting with largest sizes (least likely to be reused)
            var sizesByDescending = pools.Keys.OrderByDescending(s => s.Width * s.Height);

            foreach (var size in sizesByDescending)
            {
                var pool = pools[size];
                while (pool.Count > 0 && toRemove > 0)
                {
                    var buffer = pool.Dequeue();
                    buffer.Dispose();
                    totalBuffersInPool--;
                    toRemove--;
                }

                if (pool.Count == 0)
                {
                    pools.Remove(size);
                }

                if (toRemove <= 0)
                    break;
            }
        }
        finally
        {
            poolLock.ExitWriteLock();
        }
    }
}
```

### Memory Monitor and Pressure Management

```csharp
public class MemoryMonitor : IDisposable
{
    private readonly Timer monitorTimer;
    private readonly MemoryMonitorOptions options;
    private volatile MemoryPressureLevel currentPressureLevel;
    private long lastMemoryUsage;

    public event EventHandler<MemoryPressureEventArgs> MemoryPressureChanged;

    public MemoryMonitor(MemoryMonitorOptions options = null)
    {
        this.options = options ?? new MemoryMonitorOptions();
        monitorTimer = new Timer(CheckMemoryPressure, null, TimeSpan.Zero, this.options.CheckInterval);
    }

    private void CheckMemoryPressure(object state)
    {
        try
        {
            var currentUsage = GC.GetTotalMemory(false);
            var pressureLevel = CalculatePressureLevel(currentUsage);

            if (pressureLevel != currentPressureLevel)
            {
                var oldLevel = currentPressureLevel;
                currentPressureLevel = pressureLevel;

                OnMemoryPressureChanged(new MemoryPressureEventArgs(oldLevel, pressureLevel, currentUsage));
            }

            lastMemoryUsage = currentUsage;
        }
        catch (Exception ex)
        {
            // Log memory monitoring error but don't crash
            LogMemoryMonitorError(ex);
        }
    }

    private MemoryPressureLevel CalculatePressureLevel(long currentUsage)
    {
        var availableMemory = GC.GetTotalMemory(false);
        var usageRatio = (double)currentUsage / options.MemoryLimit;

        return usageRatio switch
        {
            < 0.6 => MemoryPressureLevel.Normal,
            < 0.8 => MemoryPressureLevel.Medium,
            < 0.9 => MemoryPressureLevel.High,
            _ => MemoryPressureLevel.Critical
        };
    }
}

public enum MemoryPressureLevel
{
    Normal,
    Medium,
    High,
    Critical
}

public class MemoryPressureEventArgs : EventArgs
{
    public MemoryPressureLevel OldLevel { get; }
    public MemoryPressureLevel NewLevel { get; }
    public long CurrentMemoryUsage { get; }
    public DateTime Timestamp { get; }

    public MemoryPressureEventArgs(MemoryPressureLevel oldLevel, MemoryPressureLevel newLevel, long currentUsage)
    {
        OldLevel = oldLevel;
        NewLevel = newLevel;
        CurrentMemoryUsage = currentUsage;
        Timestamp = DateTime.UtcNow;
    }
}
```

### Configuration Options

```csharp
public class ScreenBufferOptions
{
    public BufferPoolOptions PoolOptions { get; set; } = new();
    public MemoryMonitorOptions MemoryOptions { get; set; } = new();
    public bool EnableContentPreservation { get; set; } = true;
    public bool EnableMemoryPressureHandling { get; set; } = true;
    public int MaxConcurrentResizes { get; set; } = 3;
}

public class BufferPoolOptions
{
    public int MaxPoolSize { get; set; } = 50;
    public int MaxPoolSizePerSize { get; set; } = 10;
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnablePoolStatistics { get; set; } = false;
}

public class MemoryMonitorOptions
{
    public long MemoryLimit { get; set; } = 100 * 1024 * 1024; // 100MB
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(5);
    public bool EnableGcCollection { get; set; } = true;
    public int GcCollectionThreshold { get; set; } = 10; // MB
}
```

## Testing Approach

1. **Unit Tests**: Test buffer allocation, pooling, and memory management
2. **Stress Tests**: Test with frequent resizing and high memory pressure
3. **Performance Tests**: Measure buffer allocation/deallocation overhead
4. **Memory Tests**: Verify no memory leaks during extended operation
5. **Concurrency Tests**: Test thread-safe access to buffer management
6. **Integration Tests**: Test with rendering engine and terminal resize events
7. **Pressure Tests**: Test behavior under various memory pressure scenarios

## Related Components

- `src/CycoTui.Core/Screen/IScreenBufferManager.cs` - Buffer management interface
- `src/CycoTui.Core/Screen/ScreenBufferManager.cs` - Main implementation
- `src/CycoTui.Core/Screen/BufferPool.cs` - Buffer pooling system
- `src/CycoTui.Core/Screen/MemoryMonitor.cs` - Memory pressure monitoring
- Integration with existing `Buffer` and `IRenderEngine` implementations

## Integration Points

- **Rendering Engine**: Provides buffers for widget rendering operations
- **Terminal Controller**: Responds to terminal resize events
- **Application Class**: Coordinates buffer lifecycle with application lifecycle
- **Memory Management**: Integrates with .NET GC and memory monitoring
- **Performance Monitoring**: Provides metrics for application performance analysis

## Performance Considerations

- **Buffer Reuse**: Efficient buffer pooling to minimize allocations
- **Memory Pressure**: Responsive handling of system memory constraints
- **Resize Optimization**: Minimize content copying during buffer resizing
- **Lock Contention**: Minimize lock duration and granularity
- **GC Pressure**: Reduce object allocations in hot paths

## Platform-Specific Details

### Memory Management Strategies
- **Windows**: Integration with Windows memory management APIs
- **Unix/Linux**: Respect system memory limits and OOM killer behavior
- **Mobile/Limited Memory**: Aggressive memory management for constrained environments

## Acceptance Criteria

- [ ] `IScreenBufferManager` interface implemented with full functionality
- [ ] Double-buffered rendering system with automatic buffer swapping
- [ ] Buffer pool implementation reducing allocation overhead
- [ ] Terminal resize handling with efficient buffer reallocation
- [ ] Memory pressure monitoring and responsive management
- [ ] Content preservation during resize operations
- [ ] Thread-safe buffer management operations
- [ ] Memory leak prevention with proper buffer lifecycle management
- [ ] Performance optimization achieving target metrics (allocation <5ms)
- [ ] Integration with rendering engine and terminal controller
- [ ] Stress testing passing with frequent resizing scenarios
- [ ] Memory pressure handling preventing out-of-memory crashes
- [ ] Configuration options providing flexible memory management
- [ ] Monitoring and metrics collection for performance analysis
- [ ] Cross-platform compatibility verified

## See Also

- [SPEC-APPLAYER-001.md](../../specs/SPEC-APPLAYER-001.md): Application framework specification
- [SPEC-BUFFER-002.md](../../specs/SPEC-BUFFER-002.md): Buffer and rendering model specification
- [APPLAYER-RENDERER-001](../APPLAYER-RENDERER-001/README.md): Rendering engine implementation
- [BUFFER-MODEL-001](../BUFFER-MODEL-001/README.md): Buffer model implementation
- [APPLAYER-M6-001.md](../../roadmap/APPLAYER-M6-001.md): Application layer roadmap