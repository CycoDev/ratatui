# List Widget Implementation

## Overview

Implement the core List widget structure providing scrollable, selectable list display functionality. This task covers the basic widget implementation including the List structure, ListDirection enum, fluent builder API, and integration with the widget system interfaces.

## Implementation Approach

Create a generic `List<TItem>` widget that implements both `IWidget` and `IStatefulWidget<ListState>` interfaces. The widget should support flexible item types through generic constraints and provide a comprehensive fluent configuration API following C# conventions.

### Rendering Implementation Strategy

The rendering implementation should follow these key patterns:

1. **Stateless Delegation**: `IWidget.Render()` creates default state and delegates to `IStatefulWidget.Render()`
2. **Viewport Calculation**: Implement `GetItemsBounds()` method to efficiently determine visible items
3. **Scroll Padding**: Implement `ApplyScrollPaddingToSelectedIndex()` for maintaining context around selection
4. **Direction Handling**: Support both TopToBottom and BottomToTop rendering with different coordinate calculations

### Core Rendering Algorithm
```csharp
public void Render(Rect area, Buffer buffer, ref ListState state)
{
    // 1. Apply base style to entire area
    buffer.SetStyle(area, Style);
    
    // 2. Render block (border/title) if present and get inner area
    Block?.Render(area, buffer);
    var listArea = Block?.InnerArea(area) ?? area;
    
    // 3. Handle empty area or empty items
    if (listArea.IsEmpty || Items.Count == 0)
    {
        if (Items.Count == 0) state.Select(null);
        return;
    }
    
    // 4. Clamp selected index to valid range
    if (state.Selected >= Items.Count)
        state.Select(Items.Count - 1);
    
    // 5. Calculate visible item bounds and update state offset
    var (firstVisible, lastVisible) = GetItemsBounds(
        state.Selected, state.Offset, listArea.Height);
    state.Offset = firstVisible;
    
    // 6. Render visible items with highlighting
    RenderVisibleItems(listArea, buffer, state, firstVisible, lastVisible);
}
```

### Key Implementation Details
- Use `Math.Max(value - subtract, 0)` for saturating subtraction equivalent
- Handle highlight symbol overflow gracefully by truncating content area
- Support multi-line items with optional repeated highlight symbols
- Update state during rendering to maintain consistency

### Core Structure Design
```csharp
public struct List<TItem> : IWidget, IStatefulWidget<ListState>
{
    private readonly IList<ListItem> _items;
    private readonly Style _style;
    private readonly ListDirection _direction;
    private readonly Style _highlightStyle;
    private readonly Line? _highlightSymbol;
    private readonly bool _repeatHighlightSymbol;
    private readonly HighlightSpacing _highlightSpacing;
    private readonly ushort _scrollPadding;
    private readonly Block? _block;
}
```

### Builder Pattern Implementation
- All configuration methods return new instances (immutable updates)
- Use `[Pure]` attribute to indicate non-mutating methods
- Support generic type constraints for flexible parameter types
- Method chaining for fluent configuration

### Rendering Strategy
- Implement dual rendering support (stateless and stateful)
- Stateless rendering uses default ListState
- Stateful rendering manages selection and scrolling
- Proper integration with Block containers for borders/titles

## Key Challenges

### Generic Type Constraints
- Design flexible constraints for item types: `where TItem : IConvertible<ListItem>`
- Support common scenarios: string arrays, ListItem collections, custom objects
- Provide convenient implicit conversions and extension methods

### Immutable Builder Pattern
- Ensure all builder methods return new instances
- Maintain performance with struct semantics
- Handle complex nested structures (item collections, optional blocks)

### State Management Integration
- External state pattern with ListState class
- Reference passing for efficient state updates
- Support both mutable and immutable state approaches

### Performance Considerations
- Efficient item collection handling
- Minimal allocations during rendering
- Optimize for large lists with scrolling

## Related Components

### Core Framework Dependencies
- `Buffer` class for cell-based rendering
- `Rect` structure for area management
- `Style` system for appearance customization
- `Line` and `Text` for content rendering

### Widget System Integration
- `IWidget` interface for stateless rendering
- `IStatefulWidget<ListState>` interface for stateful rendering
- Widget composition patterns for Block integration

### Supporting Types
- `ListItem` structure (separate implementation task)
- `ListState` class (separate implementation task)
- `ListDirection` enum
- `HighlightSpacing` enum (from table widget)

## Integration Points

### Layout System
- Proper area subdivision for item placement
- Clipping and boundary checking
- Block container integration for borders/titles

### Style System
- Hierarchical style application
- Style inheritance from List to items
- Integration with highlight styling

### Text System
- Item content rendering through ListItem/Text
- Multi-line item support
- Text alignment and wrapping

## Testing Approach

### Unit Tests
- Basic widget creation and configuration
- Builder pattern method chaining
- Empty list handling
- Item collection conversion
- Style application and inheritance

### Integration Tests
- Rendering to buffer with various configurations
- Block container integration
- Area clipping and boundary handling
- State management with ListState

### Edge Case Tests
- Minimal buffer areas (1x1, zero-size)
- Very large item collections
- Complex item types and conversions
- Invalid configuration combinations

## Acceptance Criteria

### Core Implementation
- [ ] List<TItem> struct implements IWidget and IStatefulWidget<ListState>
- [ ] Generic type constraint supports common item types (string, ListItem, etc.)
- [ ] Constructor accepts IEnumerable<TItem> and converts to ListItem collection
- [ ] All properties have appropriate default values

### Builder Pattern
- [ ] All configuration methods use fluent builder pattern
- [ ] Methods marked with [Pure] attribute for immutability
- [ ] Method chaining works correctly for complex configurations
- [ ] Generic type constraints allow flexible parameter types

### Rendering Implementation
- [ ] Stateless Render(Rect, Buffer) method works correctly
- [ ] Stateful Render(Rect, Buffer, ref ListState) method works correctly
- [ ] Area clipping and boundary checking implemented
- [ ] Integration with Block containers for borders/titles

### Collection Interface
- [ ] Count property returns correct item count
- [ ] IsEmpty property indicates whether list has items
- [ ] Item collection can be updated through builder methods
- [ ] Support for LINQ operations on underlying collection

### Error Handling
- [ ] Null item collections handled gracefully
- [ ] Invalid area dimensions don't cause crashes
- [ ] Empty lists render without errors
- [ ] Large item collections handled efficiently

## See Also

- [006-LIST-WIDGET-001](../../features/006-LIST-WIDGET-001.md): List widget feature specification
- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [WIDGET-LIST-ITEM-001](../WIDGET-LIST-ITEM-001/README.md): ListItem implementation task
- [WIDGET-LIST-STATE-001](../WIDGET-LIST-STATE-001/README.md): ListState implementation task