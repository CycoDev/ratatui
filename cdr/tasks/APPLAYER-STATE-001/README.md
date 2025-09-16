# Application State Management Implementation

## Overview

This task implements the application state management system that coordinates state updates between widgets, handles global application state, and manages stateful widget persistence across rendering cycles. The state management system ensures consistent, thread-safe state operations while providing efficient access patterns for real-time applications.

## Implementation Approach

1. **Create State Management Architecture**:
   - Implement `IStateManager` interface for centralized state coordination
   - Create thread-safe state storage with concurrent access support
   - Build state change notification system for reactive updates
   - Support hierarchical state with parent-child relationships

2. **Implement Global State Management**:
   - Provide key-value global state storage accessible throughout the application
   - Support typed state access with generic methods and type safety
   - Implement state persistence and serialization capabilities
   - Create state validation and constraint mechanisms

3. **Build Widget State Coordination**:
   - Manage stateful widget state persistence across render cycles
   - Implement state lifecycle management (creation, updates, cleanup)
   - Support state sharing between related widgets
   - Handle state migration during widget hierarchy changes

4. **Create State Change Propagation**:
   - Implement observer pattern for state change notifications
   - Build event-driven state update system
   - Support batched state updates for performance
   - Create state rollback mechanisms for error recovery

## Key Challenges

1. **Thread Safety**: Ensuring safe concurrent access to shared state from multiple threads
2. **Performance**: Minimizing overhead of state access and change notifications
3. **Memory Management**: Preventing memory leaks from retained widget references
4. **State Synchronization**: Coordinating state changes with rendering cycles
5. **Type Safety**: Providing strongly-typed state access while maintaining flexibility

## Implementation Notes

### State Manager Interface

```csharp
public interface IStateManager : IDisposable
{
    // Global state management
    void SetGlobalState<T>(string key, T value);
    T GetGlobalState<T>(string key);
    T GetGlobalState<T>(string key, T defaultValue);
    bool TryGetGlobalState<T>(string key, out T value);
    void RemoveGlobalState(string key);
    IEnumerable<string> GetGlobalStateKeys();

    // Widget state management
    void SetWidgetState<TWidget, TState>(TWidget widget, TState state)
        where TWidget : IStatefulWidget<TState>;
    TState GetWidgetState<TWidget, TState>(TWidget widget)
        where TWidget : IStatefulWidget<TState>;
    bool HasWidgetState<TWidget>(TWidget widget)
        where TWidget : IWidget;
    void RemoveWidgetState<TWidget>(TWidget widget)
        where TWidget : IWidget;

    // State change notifications
    event EventHandler<StateChangedEventArgs> GlobalStateChanged;
    event EventHandler<WidgetStateChangedEventArgs> WidgetStateChanged;

    // Batch operations
    IStateTransaction BeginTransaction();
    void CommitTransaction(IStateTransaction transaction);
    void RollbackTransaction(IStateTransaction transaction);

    // State persistence
    Task SaveStateAsync(string filePath);
    Task LoadStateAsync(string filePath);
    byte[] SerializeState();
    void DeserializeState(byte[] data);
}
```

### State Manager Implementation

```csharp
public class StateManager : IStateManager
{
    // Thread-safe storage
    private readonly ConcurrentDictionary<string, object> globalState = new();
    private readonly ConcurrentDictionary<object, object> widgetStates = new();
    private readonly ReaderWriterLockSlim stateLock = new();

    // Change notification
    private readonly ConcurrentDictionary<string, List<WeakReference<IStateChangeHandler>>> stateHandlers = new();
    private readonly Timer cleanupTimer;

    // Transaction support
    private readonly ThreadLocal<StateTransaction> currentTransaction = new();

    public event EventHandler<StateChangedEventArgs> GlobalStateChanged;
    public event EventHandler<WidgetStateChangedEventArgs> WidgetStateChanged;

    public void SetGlobalState<T>(string key, T value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        object oldValue = null;
        var hasOldValue = false;

        stateLock.EnterWriteLock();
        try
        {
            hasOldValue = globalState.TryGetValue(key, out oldValue);
            globalState[key] = value;

            // Record in transaction if active
            if (currentTransaction.IsValueCreated && currentTransaction.Value != null)
            {
                currentTransaction.Value.RecordGlobalStateChange(key, oldValue, value, hasOldValue);
            }
        }
        finally
        {
            stateLock.ExitWriteLock();
        }

        // Notify observers
        OnGlobalStateChanged(new StateChangedEventArgs(key, oldValue, value));

        // Notify specific handlers
        NotifyStateChangeHandlers(key, oldValue, value);
    }

    public T GetGlobalState<T>(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        stateLock.EnterReadLock();
        try
        {
            if (globalState.TryGetValue(key, out var value))
            {
                return value is T typedValue ? typedValue : default(T);
            }
            return default(T);
        }
        finally
        {
            stateLock.ExitReadLock();
        }
    }

    public bool TryGetGlobalState<T>(string key, out T value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        stateLock.EnterReadLock();
        try
        {
            if (globalState.TryGetValue(key, out var objValue) && objValue is T typedValue)
            {
                value = typedValue;
                return true;
            }
            value = default(T);
            return false;
        }
        finally
        {
            stateLock.ExitReadLock();
        }
    }

    public void SetWidgetState<TWidget, TState>(TWidget widget, TState state)
        where TWidget : IStatefulWidget<TState>
    {
        if (widget == null)
            throw new ArgumentNullException(nameof(widget));

        object oldState = null;
        var hasOldState = false;

        stateLock.EnterWriteLock();
        try
        {
            hasOldState = widgetStates.TryGetValue(widget, out oldState);
            widgetStates[widget] = state;

            // Record in transaction if active
            if (currentTransaction.IsValueCreated && currentTransaction.Value != null)
            {
                currentTransaction.Value.RecordWidgetStateChange(widget, oldState, state, hasOldState);
            }
        }
        finally
        {
            stateLock.ExitWriteLock();
        }

        // Notify observers
        OnWidgetStateChanged(new WidgetStateChangedEventArgs(widget, oldState, state));
    }

    public TState GetWidgetState<TWidget, TState>(TWidget widget)
        where TWidget : IStatefulWidget<TState>
    {
        if (widget == null)
            throw new ArgumentNullException(nameof(widget));

        stateLock.EnterReadLock();
        try
        {
            if (widgetStates.TryGetValue(widget, out var state))
            {
                return state is TState typedState ? typedState : widget.CreateDefaultState();
            }
            return widget.CreateDefaultState();
        }
        finally
        {
            stateLock.ExitReadLock();
        }
    }
}
```

### State Transaction System

```csharp
public interface IStateTransaction : IDisposable
{
    string Id { get; }
    DateTime StartTime { get; }
    bool IsCommitted { get; }
    bool IsRolledBack { get; }

    void Commit();
    void Rollback();
}

public class StateTransaction : IStateTransaction
{
    private readonly StateManager stateManager;
    private readonly List<StateChange> changes = new();
    private bool isCommitted;
    private bool isRolledBack;

    public string Id { get; } = Guid.NewGuid().ToString();
    public DateTime StartTime { get; } = DateTime.UtcNow;
    public bool IsCommitted => isCommitted;
    public bool IsRolledBack => isRolledBack;

    internal void RecordGlobalStateChange(string key, object oldValue, object newValue, bool hadOldValue)
    {
        if (isCommitted || isRolledBack)
            throw new InvalidOperationException("Transaction is already completed");

        changes.Add(new GlobalStateChange
        {
            Key = key,
            OldValue = oldValue,
            NewValue = newValue,
            HadOldValue = hadOldValue
        });
    }

    internal void RecordWidgetStateChange(object widget, object oldState, object newState, bool hadOldState)
    {
        if (isCommitted || isRolledBack)
            throw new InvalidOperationException("Transaction is already completed");

        changes.Add(new WidgetStateChange
        {
            Widget = widget,
            OldState = oldState,
            NewState = newState,
            HadOldState = hadOldState
        });
    }

    public void Commit()
    {
        if (isCommitted)
            return;

        if (isRolledBack)
            throw new InvalidOperationException("Cannot commit a rolled back transaction");

        // Transaction changes are already applied, just mark as committed
        isCommitted = true;
    }

    public void Rollback()
    {
        if (isRolledBack)
            return;

        if (isCommitted)
            throw new InvalidOperationException("Cannot rollback a committed transaction");

        // Reverse all changes in reverse order
        for (int i = changes.Count - 1; i >= 0; i--)
        {
            changes[i].Revert(stateManager);
        }

        isRolledBack = true;
    }
}
```

### State Change Notification System

```csharp
public class StateChangedEventArgs : EventArgs
{
    public string Key { get; }
    public object OldValue { get; }
    public object NewValue { get; }
    public DateTime Timestamp { get; }

    public StateChangedEventArgs(string key, object oldValue, object newValue)
    {
        Key = key;
        OldValue = oldValue;
        NewValue = newValue;
        Timestamp = DateTime.UtcNow;
    }
}

public class WidgetStateChangedEventArgs : EventArgs
{
    public object Widget { get; }
    public object OldState { get; }
    public object NewState { get; }
    public DateTime Timestamp { get; }

    public WidgetStateChangedEventArgs(object widget, object oldState, object newState)
    {
        Widget = widget;
        OldState = oldState;
        NewState = newState;
        Timestamp = DateTime.UtcNow;
    }
}

// Reactive state observation
public interface IStateObserver<T>
{
    void OnStateChanged(string key, T oldValue, T newValue);
}

public class StateObserverRegistration : IDisposable
{
    private readonly StateManager stateManager;
    private readonly string key;
    private readonly WeakReference<IStateObserver<object>> observerRef;

    public void Dispose()
    {
        stateManager.UnregisterObserver(key, observerRef);
    }
}

// Extension methods for reactive patterns
public static class StateManagerExtensions
{
    public static StateObserverRegistration Observe<T>(this IStateManager stateManager, string key, IStateObserver<T> observer)
    {
        return stateManager.RegisterObserver(key, observer);
    }

    public static StateObserverRegistration Observe<T>(this IStateManager stateManager, string key, Action<T, T> onChanged)
    {
        var observer = new ActionStateObserver<T>(onChanged);
        return stateManager.RegisterObserver(key, observer);
    }
}
```

### State Persistence and Serialization

```csharp
public class StateSerializer
{
    private readonly JsonSerializerOptions jsonOptions;

    public StateSerializer()
    {
        jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new StateValueConverter() }
        };
    }

    public async Task SaveStateAsync(IStateManager stateManager, string filePath)
    {
        var stateData = new ApplicationStateData
        {
            GlobalState = stateManager.SerializeGlobalState(),
            Timestamp = DateTime.UtcNow,
            Version = GetCurrentStateVersion()
        };

        var json = JsonSerializer.Serialize(stateData, jsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<bool> LoadStateAsync(IStateManager stateManager, string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var stateData = JsonSerializer.Deserialize<ApplicationStateData>(json, jsonOptions);

            if (IsCompatibleVersion(stateData.Version))
            {
                stateManager.DeserializeGlobalState(stateData.GlobalState);
                return true;
            }
            else
            {
                // Handle version migration if needed
                return await MigrateAndLoadStateAsync(stateManager, stateData);
            }
        }
        catch (Exception ex)
        {
            // Log serialization error but don't crash
            await LogStateSerializationErrorAsync(ex);
            return false;
        }
    }
}
```

## Testing Approach

1. **Unit Tests**: Test state operations, thread safety, and data integrity
2. **Concurrency Tests**: Verify thread-safe access with high concurrency scenarios
3. **Performance Tests**: Measure state access latency and memory usage
4. **Transaction Tests**: Test commit/rollback behavior under various conditions
5. **Persistence Tests**: Verify serialization and deserialization accuracy
6. **Memory Leak Tests**: Ensure proper cleanup of widget references
7. **Integration Tests**: Test state coordination with widgets and rendering

## Related Components

- `src/CycoTui.Core/State/IStateManager.cs` - Main state management interface
- `src/CycoTui.Core/State/StateManager.cs` - Core state management implementation
- `src/CycoTui.Core/State/StateTransaction.cs` - Transaction support
- `src/CycoTui.Core/State/StateSerializer.cs` - Persistence and serialization
- `src/CycoTui.Core/State/StateObserver.cs` - Reactive state observation

## Integration Points

- **Application Class**: Uses state manager for global and widget state coordination
- **Widget System**: Stateful widgets integrate with state persistence mechanisms
- **Event System**: State changes can trigger events and vice versa
- **Rendering System**: State changes can trigger render requests
- **Plugin System**: Plugins can access and modify application state

## Performance Considerations

- **Lock Granularity**: Use reader-writer locks to allow concurrent reads
- **Memory Management**: Use weak references to prevent memory leaks
- **Change Batching**: Batch state change notifications to reduce overhead
- **Serialization**: Optimize serialization performance for large state objects
- **Cleanup**: Regular cleanup of orphaned widget state references

## Platform-Specific Details

### Thread Safety Considerations
- **Read-Write Locks**: Efficient concurrent access patterns
- **Atomic Operations**: Use for simple state changes
- **Memory Barriers**: Ensure proper memory visibility
- **Deadlock Prevention**: Careful lock ordering and timeout handling

## Acceptance Criteria

- [ ] `IStateManager` interface implemented with complete functionality
- [ ] Global state management with thread-safe access
- [ ] Widget state persistence across rendering cycles
- [ ] State change notification system with event propagation
- [ ] Transaction support with commit/rollback capabilities
- [ ] State serialization and persistence to storage
- [ ] Observer pattern implementation for reactive state updates
- [ ] Memory leak prevention with proper reference cleanup
- [ ] Performance optimizations for high-frequency state access
- [ ] Integration with existing widget and rendering systems
- [ ] Thread safety verified under high concurrency scenarios
- [ ] State persistence working across application restarts
- [ ] Transaction rollback restoring previous state correctly
- [ ] Memory usage remaining stable during extended operation
- [ ] Performance targets met for state access operations (<1ms)

## See Also

- [SPEC-APPLAYER-001.md](../../specs/SPEC-APPLAYER-001.md): Application framework specification
- [APPLAYER-APP-001](../APPLAYER-APP-001/README.md): Main application class implementation
- [SPEC-WIDGET-STATE-001.md](../../specs/SPEC-WIDGET-STATE-001.md): Widget state specification
- [WIDGET-STATEFUL-INTERFACE-001](../WIDGET-STATEFUL-INTERFACE-001/README.md): Stateful widget interface
- [APPLAYER-M6-001.md](../../roadmap/APPLAYER-M6-001.md): Application layer roadmap