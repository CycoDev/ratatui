---
id: SPEC-WIDGET-STATE-001
title: Widget State Management Specification
status: draft
date: 2023-11-28
---

# Widget State Management Specification

## Overview

This specification defines the patterns, interfaces, and implementation requirements for managing widget state in CycoTui. It covers stateful widget implementations, state lifecycle management, navigation patterns, and the relationship between widgets and their associated state objects.

## Scope

This specification covers:
- State object design patterns and interfaces
- Navigation and manipulation methods for widget state
- Serialization and persistence requirements
- Safe arithmetic operations for index management
- Integration patterns with stateful widgets
- Memory management and performance considerations

This specification does not cover:
- Specific widget implementations (covered in widget-specific specifications)
- Layout state management (covered in layout specifications)
- Application-level state management patterns

## Requirements

### State Object Design

#### Core State Pattern
Widget state objects should follow these design principles:
- **External Management**: State is managed outside the widget instance
- **Reference Semantics**: State is passed by reference to widgets for efficient updates
- **Type Safety**: Use strong typing to prevent state/widget mismatches
- **Immutable Options**: Support both mutable and immutable state management patterns
- **Serialization Ready**: All state objects should support serialization for persistence

#### TableState Implementation Requirements
Based on analysis of `ratatui-widgets/src/table/state.rs`:

```csharp
public class TableState : IEquatable<TableState>
{
    // Core state properties
    public int? Selected { get; set; }
    public int? SelectedColumn { get; set; }
    public int Offset { get; set; }
    
    // Static factory methods
    public static TableState Default => new TableState();
    public static TableState WithSelected(int? selected) => new TableState { Selected = selected };
    public static TableState WithOffset(int offset) => new TableState { Offset = offset };
    public static TableState WithSelectedColumn(int? column) => new TableState { SelectedColumn = column };
    public static TableState WithSelectedCell((int row, int column)? cell) => 
        cell.HasValue ? new TableState { Selected = cell.Value.row, SelectedColumn = cell.Value.column } :
                       new TableState();
    
    // Fluent setter methods (immutable pattern)
    public TableState WithSelected(int? selected) => new TableState 
    { 
        Selected = selected, 
        SelectedColumn = this.SelectedColumn, 
        Offset = this.Offset 
    };
    
    public TableState WithOffset(int offset) => new TableState 
    { 
        Selected = this.Selected, 
        SelectedColumn = this.SelectedColumn, 
        Offset = offset 
    };
    
    public TableState WithSelectedColumn(int? column) => new TableState 
    { 
        Selected = this.Selected, 
        SelectedColumn = column, 
        Offset = this.Offset 
    };
    
    public TableState WithSelectedCell((int row, int column)? cell) => 
        cell.HasValue ? new TableState 
        { 
            Selected = cell.Value.row, 
            SelectedColumn = cell.Value.column, 
            Offset = this.Offset 
        } : new TableState { Offset = this.Offset };
    
    // Selection management
    public void Select(int? index)
    {
        Selected = index;
        if (index == null)
            Offset = 0; // Reset scroll position when clearing selection
    }
    
    public void SelectColumn(int? index)
    {
        SelectedColumn = index;
    }
    
    public void SelectCell((int row, int column)? cell)
    {
        if (cell.HasValue)
        {
            Selected = cell.Value.row;
            SelectedColumn = cell.Value.column;
        }
        else
        {
            Selected = null;
            SelectedColumn = null;
            Offset = 0; // Reset offset when clearing selection
        }
    }
    
    // Cell accessor
    public (int row, int column)? SelectedCell => 
        Selected.HasValue && SelectedColumn.HasValue ? 
        (Selected.Value, SelectedColumn.Value) : null;
    
    // Row navigation methods
    public void SelectNext()
    {
        var next = Selected?.SaturatingAdd(1) ?? 0;
        Select(next);
    }
    
    public void SelectPrevious()
    {
        var previous = Selected?.SaturatingSubtract(1) ?? int.MaxValue;
        Select(previous);
    }
    
    public void SelectFirst() => Select(0);
    public void SelectLast() => Select(int.MaxValue); // Corrected during rendering
    
    // Column navigation methods
    public void SelectNextColumn()
    {
        var next = SelectedColumn?.SaturatingAdd(1) ?? 0;
        SelectColumn(next);
    }
    
    public void SelectPreviousColumn()
    {
        var previous = SelectedColumn?.SaturatingSubtract(1) ?? int.MaxValue;
        SelectColumn(previous);
    }
    
    public void SelectFirstColumn() => SelectColumn(0);
    public void SelectLastColumn() => SelectColumn(int.MaxValue); // Corrected during rendering
    
    // Scroll methods for rows
    public void ScrollDownBy(int amount)
    {
        var selected = Selected ?? 0;
        Select(selected.SaturatingAdd(amount));
    }
    
    public void ScrollUpBy(int amount)  
    {
        var selected = Selected ?? 0;
        Select(selected.SaturatingSubtract(amount));
    }
    
    // Scroll methods for columns
    public void ScrollRightBy(int amount)
    {
        var selected = SelectedColumn ?? 0;
        SelectColumn(selected.SaturatingAdd(amount));
    }
    
    public void ScrollLeftBy(int amount)
    {
        var selected = SelectedColumn ?? 0;
        SelectColumn(selected.SaturatingSubtract(amount));
    }
}
```

#### ListState Implementation Requirements
Based on analysis of `ratatui-widgets/src/list/state.rs`:

```csharp
public class ListState : IEquatable<ListState>
{
    // Core state properties
    public int? Selected { get; set; }
    public int Offset { get; set; }
    
    // Static factory methods
    public static ListState Default => new ListState();
    public static ListState WithSelected(int? selected) => new ListState { Selected = selected };
    public static ListState WithOffset(int offset) => new ListState { Offset = offset };
    
    // Fluent setter methods (immutable pattern)
    public ListState WithSelected(int? selected) => new ListState { Selected = selected, Offset = this.Offset };
    public ListState WithOffset(int offset) => new ListState { Selected = this.Selected, Offset = offset };
    
    // Selection management
    public void Select(int? index)
    {
        Selected = index;
        if (index == null)
            Offset = 0; // Reset scroll position when clearing selection
    }
    
    // Navigation methods
    public void SelectNext()
    {
        var next = Selected?.SaturatingAdd(1) ?? 0;
        Select(next);
    }
    
    public void SelectPrevious()
    {
        var previous = Selected?.SaturatingSubtract(1) ?? int.MaxValue;
        Select(previous);
    }
    
    public void SelectFirst() => Select(0);
    public void SelectLast() => Select(int.MaxValue); // Corrected during rendering
    
    // Scroll methods
    public void ScrollDownBy(int amount)
    {
        var selected = Selected ?? 0;
        Select(selected.SaturatingAdd(amount));
    }
    
    public void ScrollUpBy(int amount)  
    {
        var selected = Selected ?? 0;
        Select(selected.SaturatingSubtract(amount));
    }
}
```

### Safe Arithmetic Operations

#### Saturating Arithmetic Extensions
Implement extension methods for safe arithmetic that prevents overflow/underflow:

```csharp
public static class SaturatingArithmetic
{
    public static int SaturatingAdd(this int value, int amount)
    {
        if (amount > 0 && value > int.MaxValue - amount)
            return int.MaxValue;
        if (amount < 0 && value < int.MinValue - amount)
            return int.MinValue;
        return value + amount;
    }
    
    public static int SaturatingSubtract(this int value, int amount)
    {
        if (amount > 0 && value < int.MinValue + amount)
            return int.MinValue;
        if (amount < 0 && value > int.MaxValue + amount)
            return int.MaxValue;
        return value - amount;
    }
    
    public static int? SaturatingAdd(this int? value, int amount)
    {
        return value?.SaturatingAdd(amount);
    }
    
    public static int? SaturatingSubtract(this int? value, int amount)
    {
        return value?.SaturatingSubtract(amount);
    }
}
```

### Navigation Patterns

#### Standard Navigation Interface
Define common navigation patterns for list-like widgets:

```csharp
public interface INavigableState
{
    int? Selected { get; }
    void SelectNext();
    void SelectPrevious(); 
    void SelectFirst();
    void SelectLast();
    void Select(int? index);
}

public interface IScrollableState
{
    int Offset { get; }
    void ScrollDownBy(int amount);
    void ScrollUpBy(int amount);
}
```

### Deferred Bounds Checking

#### Placeholder Values Pattern
Support navigation operations before widget bounds are known:
- Use `int.MaxValue` as placeholder for "last" item
- Use `0` as safe default for empty or unknown collections
- Correct placeholder values during widget rendering
- Provide bounds checking during render phase

#### Bounds Correction Algorithm
```csharp
public static class StateBoundsCorrection
{
    public static void CorrectListStateBounds(ListState state, int itemCount)
    {
        if (itemCount == 0)
        {
            state.Selected = null;
            state.Offset = 0;
            return;
        }
        
        // Correct selection bounds
        if (state.Selected.HasValue)
        {
            if (state.Selected.Value >= itemCount)
                state.Selected = itemCount - 1;
            else if (state.Selected.Value < 0)
                state.Selected = 0;
            
            // Handle placeholder value
            if (state.Selected.Value == int.MaxValue)
                state.Selected = itemCount - 1;
        }
        
        // Correct offset bounds
        if (state.Offset >= itemCount)
            state.Offset = Math.Max(0, itemCount - 1);
        else if (state.Offset < 0)
            state.Offset = 0;
    }
    
    public static void CorrectTableStateBounds(TableState state, int rowCount, int columnCount)
    {
        if (rowCount == 0 || columnCount == 0)
        {
            state.Selected = null;
            state.SelectedColumn = null;
            state.Offset = 0;
            return;
        }
        
        // Correct row selection bounds
        if (state.Selected.HasValue)
        {
            if (state.Selected.Value >= rowCount)
                state.Selected = rowCount - 1;
            else if (state.Selected.Value < 0)
                state.Selected = 0;
            
            // Handle placeholder value
            if (state.Selected.Value == int.MaxValue)
                state.Selected = rowCount - 1;
        }
        
        // Correct column selection bounds
        if (state.SelectedColumn.HasValue)
        {
            if (state.SelectedColumn.Value >= columnCount)
                state.SelectedColumn = columnCount - 1;
            else if (state.SelectedColumn.Value < 0)
                state.SelectedColumn = 0;
            
            // Handle placeholder value
            if (state.SelectedColumn.Value == int.MaxValue)
                state.SelectedColumn = columnCount - 1;
        }
        
        // Correct offset bounds
        if (state.Offset >= rowCount)
            state.Offset = Math.Max(0, rowCount - 1);
        else if (state.Offset < 0)
            state.Offset = 0;
    }
}
```

### Serialization Support

#### JSON Serialization Requirements
State objects must support both System.Text.Json and Newtonsoft.Json:

```csharp
[System.Text.Json.Serialization.JsonInclude]
[Newtonsoft.Json.JsonProperty]
public class ListState
{
    [System.Text.Json.Serialization.JsonPropertyName("selected")]
    [Newtonsoft.Json.JsonProperty("selected")]
    public int? Selected { get; set; }
    
    [System.Text.Json.Serialization.JsonPropertyName("offset")]
    [Newtonsoft.Json.JsonProperty("offset")]
    public int Offset { get; set; }
    
    // Custom serialization for handling placeholder values
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public bool ShouldSerializeSelected() => Selected.HasValue && Selected.Value != int.MaxValue;
}
```

### State Lifecycle Management

#### State Initialization
- Provide static factory methods for common state configurations
- Support default state values that work with empty widgets
- Enable copy construction for state duplication

#### State Validation
- Validate state consistency during critical operations
- Provide debug assertions for development-time validation
- Log warnings for potentially problematic state transitions

#### Memory Management
- Keep state objects lightweight with minimal memory footprint
- Avoid unnecessary allocations in frequently called methods
- Support object pooling where beneficial for performance

## Technical Approach

### Implementation Strategy
1. **Define base interfaces** for common state patterns (`INavigableState`, `IScrollableState`)
2. **Implement concrete state classes** for specific widgets (ListState, TableState, etc.)
3. **Create extension methods** for safe arithmetic operations
4. **Provide utilities** for bounds checking and state correction
5. **Include serialization support** with appropriate attributes
6. **Add comprehensive tests** covering edge cases and navigation scenarios

### Integration with Widget System
State objects should integrate seamlessly with the widget rendering system:
- Pass state by reference to `IStatefulWidget<TState>.Render()` methods
- Allow state modifications during rendering for scroll position management
- Support both mutable and immutable state management patterns
- Provide clear documentation on when state modifications occur

### Performance Considerations
- All navigation operations should be O(1) complexity
- Minimize memory allocations in frequently called methods
- Use value types for simple state objects where appropriate
- Cache commonly used state instances to reduce GC pressure

## Examples

### Basic State Usage
```csharp
// Create and configure list state
var listState = ListState.Default
    .WithSelected(2)
    .WithOffset(0);

// Use with widget
var list = new List<string>(items);
list.Render(area, buffer, ref listState);

// Navigate programmatically
listState.SelectNext();
listState.ScrollDownBy(3);
```

### State Persistence
```csharp
// Serialize state for persistence
var json = JsonSerializer.Serialize(listState);

// Restore state from persistence
var restoredState = JsonSerializer.Deserialize<ListState>(json);

// Correct bounds after restoration
StateBoundsCorrection.CorrectListStateBounds(restoredState, items.Count);
```

### Safe Navigation
```csharp
// Navigation handles edge cases safely
var state = new ListState();
state.SelectNext();     // Handles null selection -> 0
state.SelectPrevious(); // Uses saturating arithmetic
state.SelectLast();     // Uses placeholder value
```

## See Also

- [SPEC-WIDGET-003](SPEC-WIDGET-003.md): Widget implementation specification
- [002-WIDGET-SYSTEM-001](../features/002-WIDGET-SYSTEM-001.md): Widget system features
- [006-LIST-WIDGET-001](../features/006-LIST-WIDGET-001.md): List widget functionality