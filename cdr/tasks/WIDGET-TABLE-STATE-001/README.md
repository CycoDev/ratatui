# Table State Management Implementation

## Overview

Implement the TableState class that manages selection, scrolling, and navigation state for Table widgets. This state object enables stateful table rendering with row/column selection, keyboard navigation, and scroll position management.

## Implementation Approach

### Core State Structure
Based on analysis of `ratatui-widgets/src/table/state.rs`:

```csharp
public class TableState : IEquatable<TableState>, INavigableState
{
    public int? Selected { get; set; }           // Selected row index
    public int? SelectedColumn { get; set; }     // Selected column index  
    public int Offset { get; set; }              // First visible row index
    
    // Computed property for cell selection
    public (int row, int column)? SelectedCell { get; }
}
```

### Navigation Methods
Implement comprehensive navigation supporting both keyboard and programmatic control:

- **Row Navigation**: `SelectNext()`, `SelectPrevious()`, `SelectFirst()`, `SelectLast()`
- **Column Navigation**: `SelectNextColumn()`, `SelectPreviousColumn()`, `SelectFirstColumn()`, `SelectLastColumn()`
- **Cell Navigation**: `SelectCell()` for direct cell selection
- **Scrolling**: `ScrollDownBy()`, `ScrollUpBy()`, `ScrollLeftBy()`, `ScrollRightBy()`

### Fluent Builder Pattern
Support both mutable operations and immutable builder patterns:

```csharp
// Fluent builders (immutable)
var state = TableState.Default
    .WithSelectedCell((2, 1))
    .WithOffset(5);

// Mutable operations
state.SelectNext();
state.ScrollDownBy(3);
```

## Key Challenges

### **Deferred Bounds Checking**
- Tables don't know their row/column count until render time
- Use `int.MaxValue` as placeholder for "last" row/column
- Implement bounds correction during widget rendering
- Handle empty tables gracefully

### **Dual-Axis Navigation**
- Support independent row and column selection
- Coordinate cell selection with individual row/column selection
- Maintain consistent state when switching between navigation modes

### **State Synchronization**
- Keep offset synchronized with selection for optimal visibility
- Reset offset appropriately when clearing selection
- Handle edge cases when selection moves outside visible area

## Related Components

### Core Dependencies
- `INavigableState` interface for common navigation patterns
- `SaturatingArithmetic` extension methods for safe arithmetic
- `StateBoundsCorrection` utility for bounds checking

### Widget Integration
- Used by `Table` widget during stateful rendering
- Integrates with `IStatefulWidget<TableState>` interface
- Supports state persistence via serialization

## Integration Points

### Widget System Integration
```csharp
public class Table : IStatefulWidget<TableState>
{
    public void Render(Rect area, Buffer buffer, ref TableState state)
    {
        // Correct state bounds based on actual table dimensions
        StateBoundsCorrection.CorrectTableStateBounds(state, rowCount, columnCount);
        
        // Use state for rendering decisions
        var visibleRows = GetVisibleRows(state.Offset, area.Height);
        RenderRows(buffer, visibleRows, state.Selected, state.SelectedColumn);
    }
}
```

### Serialization Support
```csharp
[System.Text.Json.Serialization.JsonInclude]
public class TableState
{
    [JsonPropertyName("selected")]
    public int? Selected { get; set; }
    
    [JsonPropertyName("selected_column")]  
    public int? SelectedColumn { get; set; }
    
    [JsonPropertyName("offset")]
    public int Offset { get; set; }
    
    // Don't serialize placeholder values
    [JsonIgnore]
    public bool ShouldSerializeSelected() => Selected.HasValue && Selected != int.MaxValue;
    
    [JsonIgnore]  
    public bool ShouldSerializeSelectedColumn() => SelectedColumn.HasValue && SelectedColumn != int.MaxValue;
}
```

## Testing Approach

### Unit Tests
- Test all navigation methods with various initial states
- Verify bounds checking with empty, single-item, and multi-item tables
- Test serialization roundtrip with placeholder values
- Verify fluent builder patterns work correctly

### Navigation Test Cases
```csharp
[Test]
public void SelectNext_HandlesEmptySelection()
{
    var state = new TableState();
    state.SelectNext();
    Assert.AreEqual(0, state.Selected);
}

[Test]
public void SelectPrevious_WithPlaceholderValue()
{
    var state = new TableState();
    state.SelectPrevious(); // Should set to int.MaxValue
    Assert.AreEqual(int.MaxValue, state.Selected);
}

[Test]
public void ScrollRightBy_SaturatingArithmetic()
{
    var state = new TableState { SelectedColumn = int.MaxValue - 1 };
    state.ScrollRightBy(5); // Should saturate at int.MaxValue
    Assert.AreEqual(int.MaxValue, state.SelectedColumn);
}
```

### Integration Tests
- Test state correction during widget rendering
- Verify state persistence across widget render cycles
- Test interaction with keyboard navigation handling

## Performance Considerations

### Memory Efficiency
- Keep state object lightweight (3 integer fields)
- Avoid unnecessary allocations in navigation methods
- Use value types for cell coordinates

### Computational Efficiency
- All navigation operations should be O(1)
- Use efficient saturating arithmetic implementations
- Cache computed properties where beneficial

### Rendering Efficiency
- Minimize state changes during rendering
- Only correct bounds when necessary
- Batch state updates when possible

## Acceptance Criteria

- [ ] TableState class implements all navigation methods from Ratatui equivalent
- [ ] Fluent builder pattern works for all state properties
- [ ] Saturating arithmetic prevents overflow/underflow in navigation
- [ ] Bounds correction handles empty tables and placeholder values correctly
- [ ] Serialization supports both System.Text.Json and Newtonsoft.Json
- [ ] Cell selection correctly manages both row and column indices
- [ ] Integration with IStatefulWidget pattern works seamlessly
- [ ] All navigation edge cases are covered by unit tests
- [ ] Performance is equivalent to simple struct operations

## See Also

- [SPEC-WIDGET-STATE-001](../../specs/SPEC-WIDGET-STATE-001.md): Widget state management specification
- [WIDGET-TABLE-001](../WIDGET-TABLE-001/README.md): Table widget implementation
- [009-TABLE-SYSTEM-001](../../features/009-TABLE-SYSTEM-001.md): Table system features