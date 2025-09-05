# Implement Span Type - Core Text Building Block

## Overview

Implement the `Span` class as the fundamental building block of the text system. A Span represents a contiguous segment of text with uniform styling. This is the smallest unit that can be styled and forms the foundation for Lines and Text.

## Implementation Approach

### Core Structure
- Implement `Span` class with `string Content` and `Style Style` properties
- Use immutable design with fluent API for modifications
- Support both owned and borrowed string content patterns

### Construction Methods
- `Span.Raw(string content)` - Creates span with default style
- `Span.Styled<T>(string content, T style)` - Creates span with specified style using generic conversion
- Implicit conversion from `string` to `Span`

### Fluent API Pattern
- All modification methods consume `this` and return new `Span` instance
- Use `[MustUseReturnValue]` attribute or similar to enforce usage
- Methods: `SetContent()`, `SetStyle()`, `PatchStyle()`, `ResetStyle()`

### Unicode Width Calculation
- Implement accurate width calculation using Unicode standards
- Handle multi-width characters (CJK, emojis)
- Handle zero-width characters (combining marks, etc.)
- Use `System.Globalization.StringInfo` for grapheme cluster handling

### Widget Integration
- Implement `IWidget` interface for direct rendering capability
- Complex rendering logic for grapheme positioning
- Handle area overflow and truncation
- Special handling for zero-width and multi-width characters

## Key Challenges

### Unicode Width Implementation
- .NET doesn't have built-in equivalent to Rust's `unicode-width` crate
- Need to implement or find East Asian Width calculation
- Must handle various Unicode categories correctly

### Fluent API Enforcement
- Ensure return values are used in method chains
- Consider analyzer rules or naming conventions
- Balance between usability and correctness

### Performance Optimization
- Minimize allocations during common operations
- Consider string interning for frequently used content
- Optimize grapheme iteration for rendering

## Related Components

### Dependencies
- `Style` class and conversion interfaces
- `IWidget` interface and rendering system
- Unicode width calculation implementation
- `Buffer` and `Rect` types for rendering

### Integration Points
- Used by `Line` class as fundamental building block
- Converted from any `IFormattable` type through extension methods
- Renders directly to `Buffer` for widget scenarios

## Testing Approach

### Unit Tests
- Construction patterns and conversions
- Fluent API method chaining
- Style patching and composition
- Unicode width calculations
- Edge cases (empty strings, special characters)

### Integration Tests
- Widget rendering with various content types
- Buffer integration and area handling
- Performance benchmarks for large text
- Unicode edge cases and multi-width characters

### Property-Based Tests
- Random string content and style combinations
- Invariant: width calculation consistency
- Invariant: style patching commutativity where applicable

## Performance Considerations

### Memory Management
- Use `string` immutability to advantage
- Consider `ReadOnlyMemory<char>` for slicing scenarios
- Pool common `Style` instances

### Rendering Performance
- Optimize grapheme iteration during rendering
- Cache width calculations where appropriate
- Minimize buffer write operations

## Acceptance Criteria

### Core Functionality
- [ ] Can create `Span` with string content and default style
- [ ] Can create styled `Span` with any type convertible to `Style`
- [ ] Fluent API methods work correctly and enforce return value usage
- [ ] Implicit conversion from `string` to `Span` works
- [ ] Style patching preserves existing properties correctly
- [ ] Style reset clears all styling to default

### Unicode Support
- [ ] Width calculation correct for ASCII text
- [ ] Width calculation correct for CJK characters
- [ ] Width calculation correct for emojis and other multi-width characters
- [ ] Zero-width characters handled correctly
- [ ] Grapheme clusters iterated correctly with proper styling

### Widget Integration
- [ ] Renders correctly to buffer within specified area
- [ ] Handles area overflow with proper truncation
- [ ] Multi-width characters truncated correctly at boundaries
- [ ] Zero-width characters positioned correctly relative to base characters
- [ ] Style inheritance from base style works during rendering

### Performance
- [ ] String-to-Span conversion completes in < 1ms for typical content
- [ ] Width calculation scales linearly with content length
- [ ] Memory usage reasonable compared to plain string
- [ ] No excessive allocations during fluent API usage

## See Also

- SPEC-TEXT-001: Text System Specification
- 006-TEXT-SYSTEM-001: Text System Feature
- TEXT-UNICODE-WIDTH-001: Unicode Width Implementation Task
- CORE-STYLE-SYSTEM-001: Style System Implementation