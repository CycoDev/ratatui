# Stateful Widget Interface Implementation

## Overview

Implement the `IStatefulWidget<TState>` interface and related infrastructure for widgets that maintain state between render cycles. This interface enables widgets to have persistent state while allowing the widget instances themselves to be recreated each frame.

## Implementation Approach

### Core Interface Design
```csharp
public interface IStatefulWidget<TState> : IWidgetBase
{
    void Render(Rect area, Buffer buffer, ref TState state);
}

// Experimental: Reference-based stateful widget interface
#if EXPERIMENTAL_WIDGET_REF
public interface IStatefulWidgetRef<TState> : IWidgetBase
{
    void RenderRef(Rect area, Buffer buffer, ref TState state);
}
#endif
```

### Experimental StatefulWidgetRef Pattern
- **Purpose**: Enable widgets to be rendered by reference instead of consumption
- **Use Cases**: Widget reuse, heterogeneous collections, boxed widgets
- **Implementation**: Extension methods provide automatic StatefulWidgetRef support
- **Blanket Implementation**: Any `IStatefulWidget<TState>` can be used as `IStatefulWidgetRef<TState>`
- **Unsized State Support**: Design patterns for handling state types without size constraints

### Key Design Decisions
- **Generic State Type**: Use `TState` generic parameter for compile-time type safety
- **Reference Parameter**: Pass state by reference for efficiency and in-place updates
- **Widget Immutability**: Widget instances should be immutable during rendering
- **State Separation**: State is managed externally, not within widget instances

### State Management Patterns
- State objects should be created and managed by the application
- Widgets can modify state during rendering (e.g., scroll offset calculations)
- State persists between render cycles while widgets can be recreated
- Support both value types and reference types as state

## Key Challenges

### C# vs Rust Semantics
- **Associated Types**: Rust uses associated types, C# uses generic type parameters
- **Ownership**: Rust's ownership model vs C# reference semantics require different patterns
- **Unsized Types**: Need to handle Rust's `?Sized` equivalent in C# context
- **Higher-Ranked Trait Bounds**: Rust's HRTB pattern (`for<'a>`) doesn't exist in C#
- **Blanket Implementations**: Need C# equivalent for automatic trait implementations

### StatefulWidgetRef Pattern Challenges
- **Automatic Implementation**: Design extension method pattern for blanket StatefulWidgetRef support
- **Unsized State Types**: Handle equivalent of Rust's `?Sized` for state like byte arrays
- **Feature Gating**: Implement experimental feature pattern for unstable interfaces
- **Boxed Widgets**: Support collections of different widget types through interface objects

### State Reference Handling
- Decide between `ref TState`, `in TState`, or reference type patterns
- Consider performance implications of different parameter passing strategies
- Ensure state mutation is safe and predictable

### Polymorphism Support
- Enable collections of mixed stateful and stateless widgets
- Consider how to handle different state types in widget collections
- Design for runtime type checking when needed

## Related Components

- `IWidget` interface for stateless widgets
- `IWidgetBase` common base interface  
- Buffer system for rendering
- Rect system for area management
- Specific widget implementations (List, Table, etc.)

## Integration Points

### With Buffer System
- Widgets render directly to buffer within specified area
- Must respect buffer boundaries and clipping
- Should only modify cells that need updating

### With Layout System
- Widgets receive area from layout calculations
- May need to subdivide area for child widgets
- Must work within constraint-based layout

### With Application Framework
- Applications create and manage state objects
- Integration with event handling for state updates
- Support for state serialization/persistence scenarios

## Testing Approach

### Unit Tests
- Test interface implementation with mock state objects
- Verify state mutation behavior during rendering
- Test with both value types and reference types as state
- Validate boundary conditions and edge cases

### Integration Tests
- Test with real widget implementations
- Verify state persistence across multiple render cycles
- Test widget composition with stateful widgets
- Performance testing for state passing efficiency

## Acceptance Criteria

- [ ] `IStatefulWidget<TState>` interface is defined and documented
- [ ] Interface supports both value types and reference types as state
- [ ] State can be modified during rendering (mutable reference semantics)
- [ ] Widget instances remain immutable during rendering
- [ ] Interface integrates properly with `IWidgetBase` for polymorphism
- [ ] Experimental `IStatefulWidgetRef<TState>` interface is defined and gated
- [ ] Extension methods provide automatic StatefulWidgetRef support for existing StatefulWidgets
- [ ] Support for unsized state types is designed and documented
- [ ] Boxed widget scenarios work correctly with interface collections
- [ ] Comprehensive unit tests cover interface contract
- [ ] Example implementation demonstrates usage patterns
- [ ] Documentation includes usage guidelines and best practices
- [ ] Performance is comparable to Rust implementation

## See Also

- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [002-WIDGET-SYSTEM-001](../../features/002-WIDGET-SYSTEM-001.md): Widget system requirements
- [WIDGET-BASE-001](../WIDGET-BASE-001/README.md): Base widget interface implementation