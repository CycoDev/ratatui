# Text Convenience API Implementation

## Overview

Implement C# equivalents to Ratatui's text! macro functionality, providing declarative text creation patterns through factory methods, builder patterns, and extension methods while maintaining type safety and performance.

## Implementation Approach

### Factory Method Pattern
- Implement Text.From() accepting variadic parameters
- Provide Text.Repeat() for repeated line patterns  
- Create Text.Empty() for zero-content initialization
- Support automatic type conversion from strings, Lines, and Spans

### Builder Pattern Implementation
- Create TextBuilder class with fluent interface
- Support AddLine(), RepeatLine(), and AddLines() methods
- Enable method chaining for complex text construction
- Provide Build() method returning immutable Text instance

### Extension Method Strategy
- Implement ToText() extensions for collections
- Create string.Repeat() extension for repeated text patterns
- Support implicit conversions where semantically clear
- Maintain IntelliSense discoverability

## Key Challenges

### Type Conversion Flexibility
Rust's text! macro uses .into() for automatic conversion. In C#, need to balance:
- Implicit operators for seamless usage
- Explicit factory methods for clarity
- Overloaded methods for different input types
- Collection initializer syntax support

### Performance Considerations
- Minimize allocations during construction
- Ensure builder pattern has low overhead
- Optimize repeated line creation
- Cache commonly used text instances

### API Design Consistency
- Align with other macro-equivalent patterns (span!, line!)
- Follow C# naming conventions
- Maintain fluent interface throughout hierarchy
- Provide both terse and explicit variants

## Related Components

- Text, Line, and Span core types
- Style system for styled text creation
- Extension method utilities
- Implicit conversion operators

## Integration Points

### With Style System
- Support styled text creation through factory methods
- Enable fluent styling through extension methods
- Integrate with Style.Default patterns

### With Widget System
- Ensure compatibility with widget text properties
- Support automatic conversion in widget APIs
- Maintain performance in widget rendering paths

## Testing Approach

### Unit Testing
- Test all factory method variants
- Verify builder pattern functionality
- Validate extension method behavior
- Check implicit conversion operations

### Performance Testing
- Benchmark against direct constructor usage
- Measure allocation overhead
- Test with large text content
- Validate memory usage patterns

### Integration Testing
- Test with widget integration
- Verify style system compatibility
- Check collection initializer syntax
- Validate IntelliSense experience

## Acceptance Criteria

- [ ] Text.From() accepts variadic object parameters and creates appropriate Text instance
- [ ] Text.Repeat() creates Text with specified line repeated N times
- [ ] Text.Empty() creates empty Text instance equivalent to text![]
- [ ] TextBuilder provides fluent interface for complex text construction
- [ ] Extension methods enable .ToText() conversion from collections
- [ ] Implicit conversions work seamlessly for string and Line inputs
- [ ] Collection initializer syntax works: new Text { "line1", "line2" }
- [ ] Performance is comparable to direct constructor usage
- [ ] API follows C# conventions and provides good IntelliSense experience
- [ ] Integration tests pass with widget system and style system
- [ ] Documentation includes examples equivalent to text! macro patterns

## See Also

- SPEC-MACROS-001: Macro system specification
- 006-TEXT-SYSTEM-001: Text system feature
- SPEC-TEXT-001: Text implementation specification
- MACRO-SPAN-CONVENIENCE-001: Related span convenience task