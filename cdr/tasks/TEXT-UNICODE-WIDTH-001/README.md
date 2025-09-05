# Implement Unicode Width Calculation for .NET

## Overview

Implement accurate Unicode character width calculation for terminal display purposes. This is critical for proper text layout and cursor positioning in terminal applications. .NET lacks a built-in equivalent to Rust's `unicode-width` crate, so this implementation is essential for text rendering accuracy.

## Implementation Approach

### Unicode Width Standards
- Follow Unicode Standard Annex #11 (East Asian Width)
- Implement character categories: Narrow, Wide, Fullwidth, Halfwidth, Ambiguous, Neutral
- Handle special cases for control characters, combining marks, and zero-width characters
- **Character Boundary Splitting**: Implement safe text splitting at grapheme boundaries (required for Bar widget overflow handling)
- **Performance Critical**: Used in rendering loops, must be highly optimized

### API Design
```csharp
public static class UnicodeWidth
{
    // Character-level width calculation
    public static int Width(char c)
    public static int Width(Rune rune) 
    
    // String-level width calculation
    public static int Width(string str)
    public static int Width(ReadOnlySpan<char> span)
    
    // Grapheme-aware width calculation
    public static int GraphemeWidth(string grapheme)
    
    // Character boundary detection for safe text splitting
    public static int FindCharacterBoundary(string text, int maxLength)
    public static int FindCharacterBoundary(ReadOnlySpan<char> span, int maxLength)
    
    // CJK-specific handling
    public static int WidthCjk(string str)
    public static int WidthCjk(ReadOnlySpan<char> span)
}

// Extension methods for convenience
public static class StringWidthExtensions
{
    public static int DisplayWidth(this string str)
    public static int DisplayWidth(this ReadOnlySpan<char> span)
    public static int DisplayWidthCjk(this string str)
}
```

### Implementation Strategy
1. **Unicode Data Tables**: Create lookup tables for character width categories
2. **Range-Based Lookup**: Use binary search for efficient character classification
3. **Caching**: Cache frequently accessed width calculations
4. **Grapheme Support**: Integrate with `System.Globalization.StringInfo` for proper grapheme handling

## Key Challenges

### Unicode Data Management
- Large Unicode tables need efficient storage and lookup
- Balance between memory usage and lookup performance
- Keep up with Unicode version updates

### East Asian Width Ambiguity
- Ambiguous characters can be either narrow or wide depending on context
- Need configuration for different terminal environments
- Default to narrow for most terminal applications

### Performance Requirements
- Must be fast enough for real-time text rendering
- Minimize allocations during width calculations
- Optimize for common cases (ASCII text)

### Platform Differences
- Different terminals may handle certain characters differently
- Windows console vs Unix terminals may have different width interpretations
- Need configuration or detection mechanisms

## Related Components

### Dependencies
- `System.Globalization.StringInfo` for grapheme cluster enumeration
- `System.Text.Rune` for proper Unicode scalar value handling
- Unicode character database (embedded or external)

### Integration Points
- Used by `Span.Width()` method for display width calculation
- Used during text rendering for cursor positioning
- Required for proper text alignment and truncation
- Used by layout algorithms for space calculations

## Testing Approach

### Unit Tests
- ASCII characters (width = 1)
- CJK characters (width = 2) 
- Zero-width characters (width = 0)
- Combining characters and diacritics
- Emoji and other multi-width characters
- Edge cases and special Unicode ranges

### Integration Tests
- Real terminal rendering tests
- Comparison with other Unicode width implementations
- Performance benchmarks for large text blocks
- Cross-platform behavior verification

### Test Data
- Use Unicode test data where available
- Create comprehensive test cases for common scenarios
- Include edge cases found in real-world usage

## Performance Considerations

### Optimization Strategies
- Fast path for ASCII characters (0x00-0x7F)
- Binary search for Unicode ranges
- Cache results for frequently used characters
- Avoid string allocations during calculation

### Memory Usage
- Minimize Unicode table size through compression
- Use efficient data structures (arrays vs dictionaries)
- Consider lazy loading for less common ranges

## Platform-Specific Details

### Windows
- Consider Windows console API width behavior
- Handle console font differences
- Test with Windows Terminal vs legacy console

### Unix
- Test with various terminal emulators
- Handle different font rendering systems
- Consider locale-specific width handling

## Migration Notes

### From Rust unicode-width Crate
- Port core width calculation algorithms
- Adapt Unicode data tables to C# format
- Ensure API compatibility where practical
- Maintain same precision and edge case handling

## Acceptance Criteria

### Functional Requirements
- [ ] ASCII characters return width 1
- [ ] CJK characters return width 2
- [ ] Zero-width characters return width 0
- [ ] Combining characters handled correctly
- [ ] Emoji width calculated correctly
- [ ] Grapheme clusters calculated as single unit
- [ ] CJK mode provides appropriate width calculations

### Performance Requirements
- [ ] ASCII character width calculation < 10ns
- [ ] Unicode character lookup < 100ns
- [ ] String width calculation scales linearly with length
- [ ] Memory usage < 100KB for Unicode tables
- [ ] No allocations during width calculation

### Compatibility Requirements
- [ ] Results match Rust unicode-width crate for common cases
- [ ] Cross-platform consistency where possible
- [ ] Proper handling of Unicode versions and updates
- [ ] Integration with .NET string and text APIs

### Quality Requirements
- [ ] Comprehensive test coverage for Unicode ranges
- [ ] Performance benchmarks establish baselines
- [ ] Documentation covers usage patterns and edge cases
- [ ] Examples demonstrate integration with text system

## See Also

- TEXT-SPAN-001: Span Implementation Task
- TEXT-GRAPHEME-001: Grapheme Handling Implementation
- SPEC-TEXT-001: Text System Specification
- Unicode Standard Annex #11: East Asian Width Properties
   - Double-width emoji and special symbols

3. **Implementation Options**:
   - Port Unicode width tables from existing implementations
   - Use .NET's CharUnicodeInfo for basic categorization
   - Create lookup tables for efficient width determination
   - Handle edge cases and special character ranges

## Key Challenges

1. **Unicode Standard Compliance**:
   - Must match terminal behavior across different platforms
   - Handle differences between Unicode standards
   - Account for terminal-specific rendering quirks

2. **Performance Requirements**:
   - Width calculation called frequently during rendering
   - Need efficient lookup mechanism (hash tables, ranges)
   - Consider caching for frequently calculated strings

3. **Emoji and Complex Characters**:
   - Modern emoji can be composed of multiple code points
   - Skin tone modifiers and gender variants
   - Regional indicator sequences (flags)
   - Zero-width joiners and variation selectors

4. **Platform Differences**:
   - Windows vs Unix terminal width handling
   - Different font support across terminals
   - Console vs terminal application differences

## Related Components

- **Line**: Primary consumer for width calculations
- **Span**: May need width calculations for layout
- **Buffer**: Uses width for proper positioning
- **Rendering**: Critical for accurate text layout

## Integration Points

1. **Text System**:
   - Integrate with Line.Width() method
   - Support for Span width calculations
   - Enable efficient text measurement

2. **Rendering Pipeline**:
   - Used during buffer rendering for positioning
   - Critical for alignment and truncation algorithms
   - Performance impact on overall rendering speed

## Implementation Strategy

```csharp
public static class UnicodeWidth
{
    // Main width calculation methods
    public static int GetWidth(string text)
    public static int GetWidth(ReadOnlySpan<char> text)
    public static int GetWidthCjk(string text)  // CJK-aware calculation
    
    // Character-level methods
    public static int GetWidth(Rune rune)
    public static int GetWidthCjk(Rune rune)
    
    // Extension methods for convenience
    public static int DisplayWidth(this string text)
    public static int DisplayWidthCjk(this string text)
}

public enum UnicodeWidthCategory
{
    Zero,       // Combining marks, control chars
    Narrow,     // ASCII, half-width
    Wide,       // CJK, full-width
    Ambiguous   // Depends on CJK context
}
```

## Testing Approach

1. **Test Cases**:
   - ASCII text (should be 1:1 character to width)
   - CJK characters (should be 2-width)
   - Emoji sequences (various complex cases)
   - Combining characters (should be zero-width)
   - Mixed content scenarios

2. **Compatibility Testing**:
   - Compare with known-good implementations
   - Test against terminal behavior on different platforms
   - Validate with unicode-width test cases if available

3. **Performance Testing**:
   - Benchmark calculation speed for various text lengths
   - Test memory usage and allocation patterns
   - Compare different implementation approaches

## Acceptance Criteria

- [ ] Accurate width calculation for ASCII text
- [ ] Correct handling of CJK characters (double-width)
- [ ] Proper emoji width calculation including complex sequences
- [ ] Zero-width handling for combining characters
- [ ] Performance meets rendering requirements
- [ ] Cross-platform consistency where possible
- [ ] Comprehensive test coverage with edge cases
- [ ] Integration with Line and Span classes
- [ ] Documentation with usage examples

## Research Notes

- Study existing implementations (Go's runewidth, Python's wcwidth)
- Investigate .NET CharUnicodeInfo capabilities
- Research terminal-specific width handling differences
- Consider Unicode Consortium's East Asian Width specification

## See Also

- TEXT-LINE-001: Line implementation that depends on this
- SPEC-TEXT-001: Text System Specification
- 006-TEXT-SYSTEM-001: Text System Feature