# StyledGrapheme Implementation

## Overview

Implement the StyledGrapheme type as the atomic unit of styled text for rendering operations. This type represents a single grapheme cluster with associated styling information and provides essential operations for text rendering in the terminal.

## Implementation Approach

### Core Structure
```csharp
public readonly struct StyledGrapheme
{
    public readonly string Symbol;
    public readonly Style Style;
    
    public StyledGrapheme(string symbol, Style style)
    {
        Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
        Style = style;
    }
}
```

### Key Methods
- Constructor with implicit style conversion support
- `IsWhitespace()` method with proper Unicode handling
- Style application methods following immutable pattern
- Integration with the `IStyled` interface

### Unicode Considerations
- Use `StringInfo` class for proper grapheme cluster handling
- Implement correct whitespace detection:
  - ZWSP (U+200B) → whitespace
  - NBSP (U+00A0) → NOT whitespace  
  - Standard Unicode whitespace → whitespace
- Consider using `ReadOnlySpan<char>` for performance optimization

## Key Challenges

### Unicode Grapheme Clusters
- Challenge: .NET string handling vs proper grapheme clusters
- Solution: Use `StringInfo.GetTextElementEnumerator()` or specialized Unicode library
- Risk: Performance impact of proper Unicode handling

### Immutable Style Updates
- Challenge: Maintaining immutable semantics while allowing style changes
- Solution: Return new instances, potentially with copy optimization
- Risk: Memory allocation pressure in rendering loops

### Whitespace Detection Performance
- Challenge: Character iteration may be expensive in hot rendering paths
- Solution: Cache results or optimize for single-character common cases
- Risk: Incorrect optimization breaking Unicode support

## Related Components

- `Style` class - for styling information
- `IStyled` interface - for consistent style operations  
- Text hierarchy (`Span`, `Line`, `Text`) - higher-level text types
- Buffer rendering system - primary consumer of StyledGrapheme

## Integration Points

- Terminal buffer rendering - primary use case
- Widget rendering pipeline - source of styled graphemes
- Text measurement and layout - width calculation
- Style system - style application and conversion

## Performance Considerations

- Struct vs class decision for memory efficiency
- String interning for common symbols
- Style instance reuse and pooling
- Optimization for ASCII-only content

## Testing Approach

### Unit Tests
- Constructor with various style types
- Whitespace detection with Unicode edge cases
- Style application operations
- Equality and hash code operations

### Integration Tests  
- Integration with text hierarchy types
- Buffer rendering with styled graphemes
- Unicode text rendering accuracy
- Performance benchmarks for rendering operations

### Unicode Test Cases
- Combining characters (é = e + ´)
- Zero Width Space handling
- Non-Breaking Space handling
- Multi-byte UTF-8 characters
- East Asian wide characters

## Acceptance Criteria

- [ ] StyledGrapheme can be constructed with string and style
- [ ] Implicit conversion from various style types works
- [ ] IsWhitespace() correctly identifies Unicode whitespace edge cases
- [ ] ZWSP returns true for IsWhitespace()
- [ ] NBSP returns false for IsWhitespace()
- [ ] Style can be applied immutably
- [ ] Integration with IStyled interface works
- [ ] Proper equality semantics implemented
- [ ] Performance acceptable for rendering operations
- [ ] Unicode grapheme clusters handled correctly
- [ ] Memory usage optimized for rendering scenarios

## See Also

- SPEC-TEXT-001: Text System Specification
- 006-TEXT-SYSTEM-001: Text System Feature
- CORE-STYLE-SYSTEM-001: Style System Implementation
- BUFFER-MODEL-001: Buffer Implementation