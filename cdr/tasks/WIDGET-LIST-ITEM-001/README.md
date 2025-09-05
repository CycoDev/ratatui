# ListItem Implementation

## Overview

Implement the `ListItem` structure that wraps individual items within a List widget. The ListItem provides content management, styling capabilities, dimension calculations, and flexible content conversion. This is a foundational component that bridges between raw content types and the widget rendering system.

## Implementation Approach

Create a `ListItem` structure that encapsulates text content and styling, providing a fluent API for configuration and efficient dimension calculations for layout purposes.

### Core Structure Design
```csharp
public struct ListItem : IEquatable<ListItem>
{
    private readonly Text _content;
    private readonly Style _style;
    
    public ListItem(Text content, Style style = default);
    
    // Fluent API
    public ListItem Style(Style style);
    
    // Dimension queries
    public int Height { get; }
    public int Width { get; }
    
    // Content access
    public Text Content { get; }
    public Style ItemStyle { get; }
}
```

### Conversion Strategy
- Implicit conversion operators for common types (string, Text, Line)
- Constructor overloads for different content types
- Generic factory methods for type-safe creation
- Extension methods for convenient creation from collections

### Style Integration
- Hierarchical style composition (item style + text content style)
- Support for Stylize trait pattern through extension methods
- Integration with the core styling system
- Proper style inheritance and override behavior

## Key Challenges

### Flexible Content Input
- Support conversion from string, Text, Line, Span, and custom types
- Maintain type safety while providing convenient APIs
- Handle multiline content correctly
- Preserve styling information during conversion

### Performance Optimization
- Efficient dimension calculations (width/height)
- Minimize allocations during creation and rendering
- Cache dimension calculations where beneficial
- Zero-copy scenarios where possible

### Style Composition
- Proper style merging between item and content styles
- Clear precedence rules for style conflicts
- Integration with the Stylize pattern for fluent styling
- Support for both compile-time and runtime styling

### API Design Consistency
- Follow C# conventions for property naming and method signatures
- Maintain consistency with Ratatui's fluent API patterns
- Support both builder pattern and direct property setting
- Clear separation between mutable and immutable operations

## Related Components

### Core Framework Dependencies
- `Text` system for content representation
- `Style` system for appearance management
- `Line` and `Span` types for text hierarchy
- Styling traits and extension methods

### Widget System Integration
- Integration with List widget rendering pipeline
- Support for widget composition patterns
- Alignment with widget interface expectations

### Text System Dependencies
- Text measurement and dimension calculation
- Multiline text handling and line breaking
- Unicode support and grapheme cluster handling
- Text alignment and formatting capabilities

## Integration Points

### List Widget Integration
- Seamless integration with List<T> widget
- Efficient collection handling for large lists
- Support for dynamic item creation and updates
- Proper rendering within list contexts

### Style System
- Integration with core Style types and operations
- Support for style inheritance and composition
- Compatibility with Stylize extension methods
- Runtime style application and modification

### Text Rendering
- Proper text rendering within widget contexts
- Support for complex text formatting and styling
- Integration with buffer-based rendering system
- Handling of text overflow and clipping

## Testing Approach

### Unit Tests
- Creation from various content types (string, Text, Line, Span)
- Style application and composition
- Dimension calculation accuracy
- Equality and comparison operations
- Conversion operator functionality

### Integration Tests
- Integration with List widget rendering
- Style inheritance and override behavior
- Text measurement accuracy in rendering contexts
- Performance with large content and style variations

### Edge Case Tests
- Empty content handling
- Very long single-line content
- Complex multiline content with mixed styling
- Unicode content with wide characters and combining marks
- Extreme styling combinations

## Acceptance Criteria

### Content Management
- [ ] ListItem can be created from string, Text, Line, and Span types
- [ ] Implicit conversion operators work correctly for common types
- [ ] Constructor overloads support different initialization patterns
- [ ] Content property provides access to underlying text

### Style Functionality
- [ ] Style property can be set during creation and through fluent API
- [ ] Style composition follows documented precedence rules
- [ ] Stylize extension methods work correctly for fluent styling
- [ ] Style inheritance integrates properly with List widget styles

### Dimension Calculation
- [ ] Height property returns correct line count for multiline content
- [ ] Width property returns maximum width across all lines
- [ ] Calculations handle Unicode content correctly
- [ ] Performance is acceptable for large content

### API Design
- [ ] Fluent API methods support method chaining
- [ ] Property names follow C# conventions
- [ ] Type conversions are intuitive and safe
- [ ] Error handling is consistent and helpful

### Performance Requirements
- [ ] Creation and style application are efficient
- [ ] Dimension calculations are cached when beneficial
- [ ] Memory usage is optimized for common scenarios
- [ ] Large content doesn't cause performance degradation

## Implementation Notes

Based on analysis of `ratatui-widgets/src/list/item.rs`:

### Key Patterns to Preserve
- Fluent API with method chaining for style application
- Generic content input through trait bounds (adapt to C# conversion operators)
- Dimension calculation delegation to underlying Text system
- Style composition where text styles are added to item styles

### C# Specific Adaptations
- Use implicit conversion operators instead of `Into<T>` trait
- Implement `IEquatable<ListItem>` for value equality
- Use properties instead of methods for dimension access
- Follow C# naming conventions (PascalCase for public members)

### Performance Considerations
- Consider caching width/height calculations if Text system is expensive
- Use readonly struct semantics for immutability
- Minimize allocations in common usage patterns
- Support both reference and value semantics as appropriate

## See Also

- [006-LIST-WIDGET-001](../../features/006-LIST-WIDGET-001.md): List widget feature specification
- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [SPEC-TEXT-001](../../specs/SPEC-TEXT-001.md): Text system specification
- [WIDGET-LIST-001](../WIDGET-LIST-001/README.md): List widget implementation task
- [TEXT-HIERARCHY-001](../TEXT-HIERARCHY-001/README.md): Text system hierarchy implementation