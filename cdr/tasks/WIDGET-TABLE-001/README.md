# Table Widget Implementation

## Overview

Implement the Table widget for CycoTui, providing a powerful and flexible data display component that supports rows, columns, headers, footers, multi-level selection, and scrolling. The Table widget is one of the most complex interactive widgets in the system.

## Implementation Approach

### Core Data Structure
```csharp
public struct Table<TRow> : IWidget, IStatefulWidget<TableState> where TRow : IConvertible<Row>
{
    private readonly IList<Row> _rows;
    private readonly Row? _header;
    private readonly Row? _footer;
    private readonly IList<Constraint> _widths;
    private readonly ushort _columnSpacing;
    private readonly Block? _block;
    private readonly Style _style;
    private readonly Style _rowHighlightStyle;

    // Core dependencies on Cell implementation
    // Rows contain Cell instances that handle individual cell styling and content
}

// Cell implementation requirements (dependency)
public struct Cell : IStylable
{
    private readonly Text _content;
    private readonly Style _style;

    public Cell(Text content) => (_content, _style) = (content, Style.Default);

    // Fluent builder pattern - consuming methods return new instances
    [Pure] public Cell Content<T>(T content) where T : IConvertible<Text> => 
        this with { _content = content.ToText() };
    
    [Pure] public Cell Style<S>(S style) where S : IConvertible<Style> => 
        this with { _style = style.ToStyle() };

    // Implicit conversions for ease of use
    public static implicit operator Cell(string content) => new Cell(content);
    public static implicit operator Cell(Text content) => new Cell(content);

    // Internal rendering applies cell style to area, then renders text content
    internal void Render(Rect area, Buffer buffer)
    {
        buffer.SetStyle(area, _style);  // Cell background/area style
        _content.Render(area, buffer);  // Text content with own styles
    }
}
    private readonly Style _columnHighlightStyle;
    private readonly Style _cellHighlightStyle;
    private readonly Text _highlightSymbol;
    private readonly HighlightSpacing _highlightSpacing;
    private readonly Flex _flex;
}
```

### Builder Pattern Implementation
- All configuration methods consume `self` and return new Table instance
- Support generic input types through constraints and conversion operators
- Validation during construction (e.g., percentage constraints <100%)
- Fluent method chaining with immutable updates

### State Management
```csharp
public class TableState
{
    public int? Selected { get; set; }           // Selected row index
    public int? SelectedColumn { get; set; }     // Selected column index  
    public int Offset { get; set; }              // First visible row for scrolling
    
    // Navigation methods with bounds checking
    public void SelectNext(int maxRows);
    public void SelectPrevious();
    public void SelectColumn(int? column);
    public void ScrollTo(int row);
}
```

## Key Challenges

### Column Width Distribution
- **Challenge**: Efficient layout calculation with various constraint types
- **Approach**: Integrate with existing Layout system using Flex for extra space distribution
- **Considerations**: Handle edge cases like missing widths, constraint validation, resize behavior

### Multi-Level Highlighting
- **Challenge**: Row, column, and cell highlighting with proper style precedence
- **Approach**: Calculate separate areas for each selection type, apply styles in order (Row → Column → Cell)
- **Considerations**: Area intersection calculations, performance during selection changes

### Viewport Management
- **Challenge**: Efficient scrolling for large datasets
- **Approach**: Implement `visible_rows()` algorithm that calculates renderable range based on area
- **Considerations**: Handle partial rows, ensure selected row visibility, scroll state management

### Generic Row Conversion
- **Challenge**: Accept various input types while maintaining type safety
- **Approach**: Use generic constraints with conversion operators and factory methods
- **Considerations**: Performance of conversions, memory allocations during construction

## Integration Points

### Dependencies
- **Row Component**: Individual table rows with cells, styling, and height management
- **Cell Component**: Table cell content with text, styling, and alignment
- **TableState**: External state management for selection and scrolling
- **HighlightSpacing**: Configuration for highlight symbol spacing behavior
- **Layout System**: Constraint-based column width calculation
- **Style System**: Multi-level style composition and inheritance

### Related Components
- **Block Widget**: Optional container for borders and titles
- **Text System**: Cell content rendering and text measurement
- **Buffer System**: Direct rendering to cell buffer with style application

## Testing Approach

### Unit Tests
- Column width calculation with various constraint combinations
- Viewport calculation with different area sizes and row counts
- Multi-level highlighting area calculations and style precedence
- State management navigation and bounds checking
- Builder pattern method chaining and immutability

### Integration Tests
- Rendering with various data sizes and configurations
- Scrolling behavior with state management
- Selection highlighting across row/column/cell combinations
- Header/footer rendering with different content types
- Performance testing with large datasets

### Visual Tests
- Rendering output verification for different table configurations
- Style application verification for highlighting
- Layout verification with constraint combinations
- Responsive behavior with area resizing

## Performance Considerations

### Rendering Performance
- Only render visible rows in viewport
- Minimize buffer operations during style application
- Cache column width calculations when constraints don't change
- Optimize selection area calculations

### Memory Management
- Avoid unnecessary allocations during frequent re-renders
- Consider row/cell pooling for dynamic data scenarios
- Efficient string handling for cell content
- Minimize state object allocations

### Scroll Performance
- O(n) viewport calculation where n is rows checked for visibility
- Early termination when area height is filled
- Efficient scroll position management
- Lazy evaluation of row heights when possible

## Acceptance Criteria

### Core Functionality
- [ ] Table creation with rows and column width constraints
- [ ] Header and footer row support with separate styling
- [ ] Multi-level selection (row, column, cell) with proper highlighting
- [ ] Configurable column spacing and layout options
- [ ] Block wrapper integration for borders and titles

### State Management
- [ ] External TableState for selection and scrolling
- [ ] Navigation methods with bounds checking
- [ ] Scroll position management and visibility preservation
- [ ] Both stateless and stateful rendering support

### Builder API
- [ ] Fluent configuration with method chaining
- [ ] Immutable builder pattern with consuming methods
- [ ] Generic input support for rows and constraints
- [ ] Validation during construction (constraint totals, etc.)

### Performance
- [ ] Efficient viewport rendering for large datasets
- [ ] Responsive scrolling and selection changes
- [ ] Minimal allocations during frequent re-renders
- [ ] Cached calculations where appropriate

### Integration
- [ ] Seamless integration with Layout constraint system
- [ ] Proper style composition and inheritance
- [ ] Buffer rendering with clipping and area management
- [ ] Compatible with existing widget architecture patterns

## See Also

- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [SPEC-LAYOUT-004](../../specs/SPEC-LAYOUT-004.md): Layout constraint system
- [WIDGET-BASE-001](../WIDGET-BASE-001/README.md): Base widget implementation
- [WIDGET-LIST-001](../WIDGET-LIST-001/README.md): Similar interactive widget pattern