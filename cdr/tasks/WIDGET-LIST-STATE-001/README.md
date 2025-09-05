# List State Management Implementation

## Overview

Implement the `ListState` class that manages selection and scrolling state for List widgets, providing a comprehensive navigation API while maintaining safe operations and supporting both immediate and deferred bounds checking.

## Implementation Approach

### Core Structure
- Create a `ListState` class with `Selected` (int?) and `Offset` (int) properties
- Implement both mutable property access and fluent setter methods
- Provide comprehensive navigation methods for list interaction
- Use saturating arithmetic to prevent overflow/underflow in navigation
- Support serialization for state persistence

### Navigation API
- **Selection Methods**: `Select()`, `SelectNext()`, `SelectPrevious()`, `SelectFirst()`, `SelectLast()`
- **Scroll Methods**: `ScrollDownBy(amount)`, `ScrollUpBy(amount)`
- **Fluent Setters**: `WithSelected()`, `WithOffset()` returning new instances
- **Property Access**: Direct property access and mutable property getters

### Safe Operations
- Implement saturating arithmetic using `Math.Min()`/`Math.Max()` with bounds
- Use placeholder values (`int.MaxValue`) for deferred bounds checking
- Handle null selection state gracefully throughout all operations
- Reset offset to 0 when selection is cleared

## Key Challenges

### C# Idiomatic Translation
- Convert Rust's `Option<usize>` to C# `int?` (nullable int)
- Replace `usize` with `int` for selection indices
- Implement fluent interface pattern returning new instances vs. Rust move semantics
- Create extension methods for saturating arithmetic operations

### Deferred Bounds Checking
- Navigation operations must work before list size is known
- Use `int.MaxValue` as placeholder for "last" item until rendering
- Render-time correction of out-of-bounds values
- Balance immediate feedback vs. deferred validation

### Serialization Integration
- Design for both System.Text.Json and Newtonsoft.Json compatibility
- Include appropriate attributes for state persistence
- Consider version compatibility and schema evolution

## Related Components

- **List Widget**: Primary consumer of ListState
- **IStatefulWidget<T>**: Interface that List implements
- **Scrollbar Widget**: May display scroll position from ListState
- **Table Widget**: May use similar state management patterns

## Integration Points

### Widget System Integration
- `ListState` passed by reference to `List.Render()` method
- State modifications during rendering for viewport management
- Integration with event handling for user input processing

### Serialization System
- State persistence for application settings
- Session state management across application restarts
- Network serialization for distributed applications

## Performance Considerations

### Memory Efficiency
- Keep state object minimal (two fields: Selected, Offset)
- Avoid unnecessary allocations in navigation methods
- Use value types where appropriate for performance

### Operation Efficiency
- All navigation operations should be O(1)
- Minimize object creation in fluent setter methods
- Cache commonly used state instances where beneficial

## Acceptance Criteria

### Core Functionality
- [ ] `ListState` class with `Selected` and `Offset` properties
- [ ] Property getters and setters with appropriate access modifiers
- [ ] `Select(int? index)` method that resets offset when null
- [ ] Navigation methods: `SelectNext()`, `SelectPrevious()`, `SelectFirst()`, `SelectLast()`
- [ ] Scroll methods: `ScrollDownBy(int amount)`, `ScrollUpBy(int amount)`
- [ ] Fluent setters: `WithSelected(int? index)`, `WithOffset(int offset)`

### Safety Features  
- [ ] Saturating arithmetic prevents overflow/underflow
- [ ] Graceful handling of null selection in all operations
- [ ] Automatic offset reset when selection is cleared
- [ ] Support for deferred bounds checking using placeholder values

### Integration Features
- [ ] Serialization support with appropriate attributes
- [ ] XML documentation for all public members
- [ ] Unit tests covering all navigation scenarios
- [ ] Integration tests with List widget rendering

### API Design
- [ ] Follows C# naming conventions (PascalCase methods)
- [ ] Implements `IEquatable<ListState>` for value comparison
- [ ] Provides `ToString()` override for debugging
- [ ] Includes static factory methods for common scenarios

## See Also

- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [002-WIDGET-SYSTEM-001](../../features/002-WIDGET-SYSTEM-001.md): Widget system features
- [006-LIST-WIDGET-001](../../features/006-LIST-WIDGET-001.md): List widget functionality