# Row Factory Methods and Convenience APIs

## Overview

Implement static factory methods and extension methods for creating table rows with ergonomic syntax, providing C# equivalents to Ratatui's `row!` macro patterns.

## Implementation Approach

### Static Factory Methods
Create static methods on the Row class that provide convenient construction patterns:

```csharp
public static class Row
{
    // Empty row creation
    public static Row Empty() => new Row();
    
    // Multi-cell creation with automatic conversion
    public static Row From(params object[] cells) => 
        new Row(cells.Select(cell => ConvertToCell(cell)));
    
    // Repeated cell creation
    public static Row Repeat(object cell, int count) => 
        new Row(Enumerable.Repeat(ConvertToCell(cell), count));
    
    // Specific typed overloads for performance
    public static Row From(params string[] texts) => 
        new Row(texts.Select(text => new Cell(text)));
    
    public static Row From(params Cell[] cells) => 
        new Row(cells);
}
```

### Extension Methods
Provide fluent extension methods for collection-based construction:

```csharp
public static class RowExtensions
{
    public static Row ToRow(this IEnumerable<string> texts) => 
        new Row(texts.Select(t => new Cell(t)));
    
    public static Row ToRow(this IEnumerable<Cell> cells) => 
        new Row(cells);
    
    public static Row ToRow(this IEnumerable<object> objects) => 
        new Row(objects.Select(obj => ConvertToCell(obj)));
}
```

### Type Conversion Strategy
Implement robust conversion from various types to Cell:

```csharp
private static Cell ConvertToCell(object value)
{
    return value switch
    {
        null => new Cell(string.Empty),
        Cell cell => cell,
        string text => new Cell(text),
        Text text => new Cell(text),
        Line line => new Cell(line),
        Span span => new Cell(span),
        _ => new Cell(value.ToString())
    };
}
```

## Key Challenges

### Performance Optimization
- Minimize allocations during row construction
- Use array-based construction when possible
- Consider object pooling for frequently created rows
- Optimize the type conversion switch statement

### Type Safety
- Ensure robust handling of null values
- Provide clear error messages for unsupported types
- Consider generic constraints where appropriate
- Validate count parameters for repetition methods

### API Consistency
- Maintain naming consistency with other factory methods
- Follow C# conventions for method naming and parameter ordering
- Ensure extension methods don't conflict with existing APIs
- Provide both params and IEnumerable overloads

## Related Components

- Cell class and its conversion capabilities
- Text, Line, and Span types for rich content
- Collection interfaces for LINQ compatibility
- Row class constructor and initialization logic

## Integration Points

### With Text System
- Integrate with Text, Line, and Span conversion methods
- Support styled content creation within cells
- Maintain formatting and style information during conversion

### With Widget System
- Ensure compatibility with Table widget expectations
- Support stateful row usage patterns
- Integrate with row selection and highlighting features

### With Macro System
- Coordinate with other convenience API patterns
- Maintain consistent naming conventions across factory methods
- Support composition with layout and styling convenience APIs

## Testing Approach

### Unit Tests
- Test all factory method overloads with various input types
- Verify correct handling of null and empty inputs
- Test performance characteristics with large data sets
- Validate type conversion behavior for edge cases

### Integration Tests
- Test integration with Table widget
- Verify styling preservation through conversion
- Test with complex nested content (Text containing Lines containing Spans)
- Validate LINQ compatibility and extension method chaining

### Performance Tests
- Benchmark factory methods vs manual construction
- Test memory allocation patterns
- Compare different construction approaches
- Measure impact on table rendering performance

## Acceptance Criteria

- [ ] Row.From() accepts heterogeneous object arrays and converts appropriately
- [ ] Row.Repeat() creates rows with repeated cells efficiently
- [ ] Row.Empty() creates empty rows with minimal overhead
- [ ] Extension methods provide fluent construction from collections
- [ ] Type conversion handles all supported content types correctly
- [ ] Performance is equivalent to manual Row construction
- [ ] API supports method chaining and LINQ integration
- [ ] Error handling provides clear messages for invalid inputs
- [ ] Integration with Table widget maintains all existing functionality
- [ ] Documentation includes examples for all supported usage patterns

## See Also

- SPEC-MACROS-001: Macro system specification including row patterns
- 009-TABLE-SYSTEM-001: Table system feature requirements
- 010-MACRO-SYSTEM-001: Macro system and convenience APIs
- WIDGET-TABLE-001: Table widget implementation task