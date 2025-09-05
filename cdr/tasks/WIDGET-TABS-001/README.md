# Tabs Widget Implementation

## Overview

Implement the Tabs widget that displays a horizontal set of tabs with a single tab selected. The widget should provide a fluent API for configuration, support Unicode text properly, and handle various styling options including selection highlighting, custom dividers, and padding.

## Implementation Approach

### Core Structure
Create a `Tabs` class with the following key properties:
- `Block` - Optional block wrapper for borders and titles
- `IList<Line>` - Collection of tab titles as styled lines
- `int?` - Selected tab index (nullable for no selection)
- `Style` - Overall widget style
- `Style` - Highlight style for selected tab
- `Span` - Custom divider between tabs (defaults to '|')
- `Line` - Left padding for each tab
- `Line` - Right padding for each tab

### Fluent API Design
Implement method chaining pattern:
```csharp
var tabs = new Tabs(new[] { "Tab1", "Tab2", "Tab3" })
    .Select(1)
    .Style(Style.Default.Foreground(Color.White))
    .HighlightStyle(Style.Default.Background(Color.Blue))
    .Divider("•")
    .Padding("[ ", " ]")
    .Block(Block.Bordered().Title("Navigation"));
```

### Generic Construction
Support flexible title input:
- Constructor accepting `IEnumerable<T>` where `T` can be converted to `Line`
- Implicit conversion operators for common types (`string`, `Line`, `Span`)
- Extension methods for collection-to-tabs conversion

### Rendering Algorithm
1. Apply overall style to the entire area
2. Render optional block wrapper
3. Calculate inner area for tabs
4. Iterate through tabs, rendering padding + title + padding + divider
5. Apply highlight style to selected tab area
6. Handle width overflow by early termination

## Key Challenges

### Unicode Width Calculation
- Need accurate Unicode width calculation for proper tab spacing
- Must handle CJK characters, combining marks, and emoji properly
- Consider both standard width and CJK-aware width calculations
- **Solution**: Implement or integrate Unicode width calculation library

### Generic Type Handling
- Support flexible input types while maintaining type safety
- Provide ergonomic API without excessive method overloads
- **Solution**: Use generic constraints with `IConvertible` interfaces and extension methods

### Width-Aware Rendering
- Must handle cases where tabs don't fit in available space
- Need efficient width calculation without full rendering
- **Solution**: Implement separate width calculation method and use it for overflow detection

## Related Components

### Dependencies
- `Buffer` - For rendering target
- `Rect` - For area definitions
- `Style` - For styling system
- `Line`, `Span` - For text components
- `Block` - For optional wrapper
- Unicode width calculation library

### Integration Points
- `IWidget` interface implementation
- Style system integration
- Block wrapper integration
- Text component system

## Testing Approach

### Unit Tests
- Test tab construction with various input types
- Verify selection bounds checking
- Test width calculation accuracy
- Validate style application
- Test overflow handling

### Rendering Tests
- Test basic tab rendering
- Test selection highlighting
- Test custom dividers and padding
- Test block integration
- Test minimal and zero-size buffer handling

### Unicode Tests
- Test CJK character handling
- Test emoji and combining character support
- Test width calculation accuracy
- Test mixed character type scenarios

## Performance Considerations

### Efficient Width Calculation
- Cache width calculations when possible
- Avoid unnecessary string operations during rendering
- Use efficient Unicode width lookup

### Memory Management
- Reuse buffer operations where possible
- Minimize allocations during rendering
- Consider object pooling for frequently created instances

### Rendering Optimization
- Early termination on width overflow
- Efficient style application
- Minimize buffer writes

## Acceptance Criteria

- [ ] Tabs widget class implemented with all core properties
- [ ] Fluent API supports method chaining for all configuration options
- [ ] Generic constructors accept various collection and item types
- [ ] Selection management with bounds checking and null support
- [ ] Custom dividers and padding support
- [ ] Block wrapper integration
- [ ] Width calculation methods (standard and CJK)
- [ ] Proper Unicode character width handling
- [ ] Rendering handles width overflow gracefully
- [ ] Style system integration (overall, highlight, individual titles)
- [ ] Comprehensive test suite covering all functionality
- [ ] Documentation with usage examples
- [ ] Performance benchmarks for large tab sets

## See Also

- [SPEC-WIDGET-003.md](../../specs/SPEC-WIDGET-003.md) - Widget implementation specification
- [SPEC-TEXT-001.md](../../specs/SPEC-TEXT-001.md) - Text handling specification
- [UNICODE-WIDTH-001](../UNICODE-WIDTH-001/README.md) - Unicode width calculation task
- [002-WIDGET-SYSTEM-001.md](../../features/002-WIDGET-SYSTEM-001.md) - Widget system feature