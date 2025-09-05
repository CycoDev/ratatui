# Line Implementation

## Overview

Implement the Line class that represents a single line of styled text with alignment support, serving as the central building block of the text hierarchy in CycoTui.

## Implementation Approach

The Line class should be implemented as a collection of Spans with additional properties for overall styling and alignment. Based on analysis of `ratatui-macros/src/line.rs`, the implementation must support these construction patterns:

1. **Core Structure**:
   - Maintain a collection of Span objects
   - Store overall line style that patches span styles
   - Support optional alignment (nullable enum)
   - Implement IReadOnlyList<Span> for collection access

2. **Construction Patterns** (from Ratatui line! macro analysis):
   ```csharp
   // Empty line: line![] equivalent
   var line = Line.Empty();
   var line = new Line(); // default constructor
   
   // Multiple spans: line!["hello", "world"] equivalent
   var line = Line.From("hello", "world");
   var line = new Line { "hello", "world" }; // collection initializer
   
   // Repeated span: line!["hello"; 3] equivalent
   var line = Line.Repeat("hello", 3);
   var line = Line.FromSpan("hello", count: 3);
   
   // Mixed types with automatic conversion
   var line = Line.From("text", span, "more text");
   
   // Builder pattern for complex scenarios
   var line = Line.Builder()
       .Add("hello")
       .Add(span)
       .WithAlignment(Alignment.Center)
       .Build();
   ```

3. **Automatic Type Conversion** (equivalent to Rust .into()):
   - Implicit operators from string to Line
   - Implicit operators from Span to Line
   - Implicit operators from arrays of strings/spans
   - Support for heterogeneous collections via method overloads

4. **Collection Integration**:
   - Implement ICollection<Span> to support collection initializers
   - Extension methods: ToLine() on IEnumerable<Span> and IEnumerable<string>
   - LINQ compatibility for span manipulation

5. **Unicode Handling**:
   - Implement accurate width calculation using .NET Unicode APIs
   - Create safe truncation logic that respects grapheme boundaries
   - Handle emoji, CJK characters, and combining marks correctly

6. **Rendering Support**:
   - Implement alignment-aware rendering with truncation
   - Create efficient span skipping algorithm for performance
   - Support styled grapheme iteration for rendering pipeline

## Key Challenges

1. **Unicode Width Calculation**:
   - Need to implement or find .NET equivalent of unicode-width crate
   - Must handle East Asian characters, emoji, and combining marks
   - Performance considerations for width calculation

2. **Safe Truncation Algorithm**:
   - Prevent corruption of multi-byte characters
   - Handle emoji that span multiple code units
   - Maintain visual integrity during truncation

3. **Alignment and Rendering**:
   - Complex algorithm for handling different alignments with truncation
   - Efficient span processing for large lines
   - Performance optimization for common cases

4. **Memory Management**:
   - Balance between performance and memory usage
   - Consider string interning for common content
   - Optimize collection storage for typical use cases

## Related Components

- **Span**: Line contains and manages Span objects
- **Text**: Text contains Line objects in collection
- **Style**: Line integrates with style system for formatting
- **Buffer**: Line renders to Buffer during UI rendering
- **Widget**: All text widgets consume Line objects

## Integration Points

1. **Style System Integration**:
   - Line style patches individual span styles
   - Support for style inheritance and composition
   - Integration with theme system

2. **Widget Integration**:
   - All text-displaying widgets should accept Line objects
   - Consistent API patterns across widget implementations
   - Support for both simple and complex text scenarios

3. **Rendering Pipeline**:
   - Integration with buffer rendering system
   - Support for clipping and viewport management
   - Efficient rendering performance

## Testing Approach

1. **Unit Tests**:
   - Test all construction methods and conversions
   - Verify Unicode width calculations with test cases
   - Test alignment rendering with various scenarios
   - Validate safe truncation with problematic Unicode

2. **Performance Tests**:
   - Benchmark width calculation performance
   - Test rendering performance with large lines
   - Memory usage validation

3. **Integration Tests**:
   - Test with various widget implementations
   - Validate style system integration
   - Cross-platform Unicode behavior

## Acceptance Criteria

- [ ] Line can be constructed from string, spans, or combinations using multiple patterns
- [ ] Empty line construction: `Line.Empty()` and `new Line()` work correctly
- [ ] Multiple span construction: `Line.From("a", "b", "c")` works correctly
- [ ] Repeated span construction: `Line.Repeat("text", count)` works correctly
- [ ] Collection initializer syntax: `new Line { "a", "b" }` works correctly
- [ ] Extension method construction: `new[] {"a", "b"}.ToLine()` works correctly
- [ ] Automatic type conversion from strings and spans works correctly
- [ ] Fluent API allows chaining of style and alignment operations
- [ ] Unicode width calculation is accurate for all character types
- [ ] Alignment rendering works correctly with truncation
- [ ] Safe truncation preserves character integrity
- [ ] Performance meets established benchmarks for all construction patterns
- [ ] Integration with style system works correctly
- [ ] All existing widgets can consume Line objects
- [ ] Comprehensive test coverage includes all construction patterns
- [ ] Documentation covers all construction patterns and migration from Ratatui line! macro

## See Also

- SPEC-TEXT-001: Text System Specification
- 006-TEXT-SYSTEM-001: Text System Feature
- TEXT-HIERARCHY-001: Text hierarchy implementation
- TEXT-UNICODE-WIDTH-001: Unicode width calculation task