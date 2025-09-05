# Frame Abstraction Implementation

## Overview

Implement the Frame abstraction that provides controlled access to the rendering buffer and manages the frame-based rendering lifecycle. This task involves creating Frame and CompletedFrame classes that match Ratatui's frame model while adapting to C# idioms.

## Implementation Approach

Based on analysis of `ratatui-core/src/terminal/frame.rs`, implement:

### Core Frame Class
```csharp
public class Frame : IDisposable
{
    private readonly Buffer _buffer;
    private readonly Rect _viewportArea;
    private readonly ulong _count;
    private Position? _cursorPosition;
    
    // Properties
    public Rect Area => _viewportArea;
    public ulong Count => _count;
    public Buffer Buffer => _buffer;
    
    // Widget rendering methods
    public void RenderWidget<T>(T widget, Rect area) where T : IWidget;
    public void RenderStatefulWidget<T, TState>(T widget, Rect area, ref TState state) 
        where T : IStatefulWidget<TState>;
    
    // Cursor management
    public void SetCursorPosition(Position position);
    
    // IDisposable implementation
    public void Dispose();
}
```

### CompletedFrame Class
```csharp
public class CompletedFrame
{
    public Buffer Buffer { get; }
    public Rect Area { get; }
    public ulong Count { get; }
    
    internal CompletedFrame(Buffer buffer, Rect area, ulong count);
}
```

## Key Challenges

1. **Lifetime Management**: Replace Rust's lifetime system with IDisposable pattern
2. **Generic Constraints**: Map Rust trait constraints to C# interface constraints
3. **Mutable References**: Handle Rust's `&mut` pattern with C# reference semantics
4. **Cursor API Design**: Resolve frame-level vs terminal-level cursor control

## Related Components

- `Buffer` class for rendering operations
- `IWidget` and `IStatefulWidget` interfaces for rendering
- `Terminal` class for frame creation and management
- `Position` and `Rect` types for layout

## Integration Points

- **Terminal Class**: Must create and manage Frame instances
- **Widget System**: Must implement rendering through Frame interface
- **Buffer Diffing**: Frame completion triggers buffer comparison
- **Cursor Management**: Frame cursor position applied after rendering

## Testing Approach

- Unit tests for Frame creation and property access
- Tests for widget rendering delegation
- Tests for cursor position management
- Integration tests with Terminal and Buffer classes
- Test frame disposal and resource cleanup

## Acceptance Criteria

- [ ] Frame class provides controlled access to rendering buffer
- [ ] Widget rendering delegates correctly to widget render methods
- [ ] Cursor position management works as expected
- [ ] Frame disposal triggers proper cleanup
- [ ] CompletedFrame provides immutable access to completed state
- [ ] Frame count increments correctly for animation support
- [ ] Area property remains stable during frame rendering
- [ ] Integration with Terminal class works correctly
- [ ] Memory management follows C# best practices
- [ ] API feels natural to C# developers

## See Also

- `SPEC-BACKEND-001.md` - Backend specification including frame model
- `SPEC-BUFFER-002.md` - Buffer specification for frame-buffer relationship  
- `004-BACKEND-ABSTRACTION-001.md` - Backend abstraction feature
- `ratatui-core/src/terminal/frame.rs` - Original Rust implementation