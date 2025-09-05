# Text Reflow Implementation

## Overview

Implement text reflow algorithms for CycoTui, including word-boundary wrapping and line truncation. This includes the LineComposer abstraction and concrete implementations for WordWrapper and LineTruncator.

## Implementation Approach

### Core Abstraction
- Define `ILineComposer<T>` interface for reflow strategies
- Create `WrappedLine<T>` result structure
- Support generic input types for flexibility

### Word Wrapping Algorithm
- Implement state machine for word boundary detection
- Handle Unicode character width calculation accurately
- Support configurable whitespace trimming
- Use object pooling to minimize garbage collection pressure
- Handle edge cases: long words, double-width characters, zero-width characters

### Line Truncation Algorithm
- Simple single-pass truncation with width limits
- Support horizontal scrolling through offset parameter
- Preserve text alignment during truncation
- Ensure Unicode character boundary safety

### Unicode Support
- Integrate with Unicode width calculation library
- Handle grapheme cluster boundaries properly
- Support East Asian width standards
- Handle special whitespace characters correctly

## Key Challenges

### Performance Optimization
- **Object Pooling**: Reuse List<T> and other collections to reduce GC pressure
- **Efficient Buffering**: Minimize memory allocations during text processing
- **State Management**: Maintain complex state efficiently across multiple calls

### Unicode Complexity
- **Character Width**: Accurately calculate display width for various Unicode characters
- **Grapheme Clusters**: Handle combining characters and complex scripts properly
- **Boundary Detection**: Ensure safe text splitting without corrupting characters

### Memory Management
- **Borrowing Semantics**: Translate Rust's lifetime management to C# patterns
- **Iterator Efficiency**: Balance between IEnumerable overhead and performance
- **Buffer Reuse**: Implement efficient buffer management strategies

## Related Components

### Dependencies
- Unicode width calculation library (custom or third-party)
- Style system for StyledGrapheme handling
- Layout system for Alignment support
- Text hierarchy (Span, Line, Text types)

### Consumers
- Paragraph widget for text rendering
- List widget for item text wrapping
- Any widget requiring text layout and overflow handling

## Integration Points

### Widget System
- Integrate with widget rendering pipeline
- Support for area-constrained text layout
- Alignment handling within widget boundaries

### Buffer System
- Efficient rendering to terminal buffer
- Coordinate with cell-based display model
- Style information preservation during reflow

## Performance Considerations

### Algorithm Complexity
- WordWrapper: O(n) where n is number of characters
- LineTruncator: O(k) where k is characters up to width limit
- Memory usage: Minimize allocation per character processed

### Optimization Strategies
- Pre-allocate buffers for common text lengths
- Reuse collections through object pooling
- Early termination for truncation scenarios
- Efficient grapheme cluster iteration

## Testing Approach

### Unit Tests
- Basic wrapping scenarios (short lines, long words, mixed content)
- Unicode edge cases (CJK characters, combining marks, zero-width)
- Alignment preservation across wrapping operations
- Trimming behavior with various whitespace configurations
- Performance regression tests for large text processing

### Integration Tests
- Widget rendering with wrapped text
- Alignment correctness in various scenarios
- Buffer output validation for complex Unicode text
- Memory usage patterns and GC pressure analysis

## Acceptance Criteria

### Functional Requirements
- [ ] ILineComposer interface implemented and tested
- [ ] WordWrapper correctly handles word boundaries and overflow
- [ ] LineTruncator supports horizontal scrolling and alignment
- [ ] Unicode character width calculation is accurate
- [ ] Whitespace trimming behavior matches specification
- [ ] All edge cases (empty text, zero width, long words) handled correctly

### Performance Requirements
- [ ] No observable GC pressure during typical text processing
- [ ] Object pooling reduces allocation overhead significantly
- [ ] Processing speed comparable to or better than simple string operations
- [ ] Memory usage scales linearly with input text size

### Integration Requirements
- [ ] Works correctly with Style system
- [ ] Integrates seamlessly with widget rendering
- [ ] Supports all alignment options
- [ ] Compatible with existing text hierarchy

## See Also

- SPEC-TEXT-001: Text System Specification
- SPEC-WIDGET-003: Widget Implementation Specification
- UNICODE-WIDTH-001: Unicode Width Implementation Task
- TEXT-HIERARCHY-001: Text Type Hierarchy Task