# Extension Methods for Convenience APIs

## Overview

Implement extension methods that provide convenient shorthand syntax for common operations, closely mirroring Ratatui's macro functionality while remaining idiomatic to C#.

## Implementation Approach

### Style Extension Methods
- Extend `string` with styling methods (`.Bold()`, `.Italic()`, `.Green()`, etc.)
- Extend `Style` with fluent composition methods
- Extend core types with styling convenience methods
- Support method chaining for complex style combinations

### Constraint Extension Methods
- Extend `int` with constraint creation methods (`.Fixed()`, `.Percent()`, `.Fill()`)
- Extend arrays/collections with constraint collection builders
- Provide symbolic operator equivalents where possible
- Support constraint composition and validation

### Collection Extension Methods
- Extend `IEnumerable<T>` with specialized collection builders
- Provide LINQ-style methods for UI element construction
- Support functional programming patterns for UI building
- Enable collection initializer syntax where appropriate
- Add `.ToRow()` methods for table row construction from collections
- Support `.ToLine()` methods for text line construction
- Provide `.Repeat()` methods for element repetition patterns

### Conversion Extension Methods
- Provide implicit and explicit conversion extensions
- Support automatic type promotion for UI elements
- Handle common conversion scenarios (string to Span, etc.)
- Maintain type safety while reducing verbosity

## Key Challenges

### Method Resolution and Overloads
- Avoid extension method conflicts with existing APIs
- Ensure proper method resolution order
- Handle generic method constraints correctly
- Provide clear disambiguation when conflicts arise

### Type Safety and IntelliSense
- Maintain excellent IntelliSense discovery
- Provide meaningful parameter names and documentation
- Ensure compile-time type checking
- Handle nullable reference types appropriately

### Performance Considerations
- Extension methods should inline to direct calls
- Avoid unnecessary allocations in hot paths
- Consider caching for expensive operations
- Measure and optimize common usage patterns

## Related Components

- Core style system (Style, Color, Modifier)
- Text types (Span, Line, Text)
- Layout types (Constraint, Layout)
- Collection types and interfaces

## Integration Points

### With Builder Patterns
- Extension methods can provide entry points to builders
- Builders can use extension methods internally
- Consistent API patterns across approaches
- Seamless transition between extension and builder syntax

### With Core Types
- Extensions should feel like natural members of extended types
- Maintain compatibility with existing constructor patterns
- Support round-trip conversion scenarios
- Integrate with existing method chains

## Testing Approach

### Extension Method Testing
- Test each extension method in isolation
- Verify correct type inference and overload resolution
- Test method chaining scenarios
- Validate performance characteristics

### Integration Testing
- Test with real-world usage patterns
- Verify IntelliSense behavior in IDE
- Test with various target framework versions
- Validate with different C# language versions

### Usability Testing
- Test discoverability of extension methods
- Verify intuitive method naming
- Test common usage patterns for clarity
- Validate error message quality

## Implementation Notes

### String Styling Extensions
```csharp
public static class StringExtensions
{
    public static Span Bold(this string text) => Span.Styled(Modifier.Bold, text);
    public static Span Italic(this string text) => Span.Styled(Modifier.Italic, text);
    public static Span Green(this string text) => Span.Styled(Color.Green, text);
    public static Span Red(this string text) => Span.Styled(Color.Red, text);
    // ... other color and style methods
}
```

### Constraint Extensions
```csharp
public static class ConstraintExtensions
{
    public static Constraint Fixed(this int value) => Constraint.Length(value);
    public static Constraint Percent(this int value) => Constraint.Percentage(value);
    public static Constraint Fill(this int weight = 1) => Constraint.Fill(weight);
    public static Constraint Min(this int value) => Constraint.Min(value);
    public static Constraint Max(this int value) => Constraint.Max(value);
}
```

### Collection Extensions
```csharp
public static class CollectionExtensions
{
    public static Line ToLine(this IEnumerable<Span> spans) => Line.From(spans);
    public static Text ToText(this IEnumerable<Line> lines) => Text.From(lines);
    public static Row ToRow(this IEnumerable<Cell> cells) => Row.From(cells);
    public static Constraints ToConstraints(this IEnumerable<Constraint> constraints) => Constraints.From(constraints);
}
```

## Acceptance Criteria

- [ ] String styling is as concise as Ratatui span macros
- [ ] Constraint creation matches symbolic notation from Ratatui
- [ ] Collection building supports fluent and functional patterns
- [ ] IntelliSense discovery is excellent for all extensions
- [ ] Method chaining works seamlessly with core APIs
- [ ] Performance is equivalent to direct constructor calls
- [ ] Extension methods integrate well with builder patterns
- [ ] Documentation provides clear usage examples

## See Also

- SPEC-MACROS-001: Macro system specification
- MACRO-BUILDER-PATTERNS-001: Builder pattern implementation
- CORE-STYLE-SYSTEM-001: Style system implementation
- TEXT-HIERARCHY-001: Text system implementation