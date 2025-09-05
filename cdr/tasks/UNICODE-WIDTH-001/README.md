# Unicode Width Calculation Implementation

## Overview

Implement accurate Unicode character width calculation for terminal display in CycoTui. This is a critical foundation component needed for proper text alignment, wrapping, and cursor positioning across all text-based widgets, including tabs, tables, and text-based layouts.

## Implementation Approach

### Research Phase
1. **Evaluate Existing Libraries**:
   - Search NuGet for "Unicode width", "EastAsianWidth", "terminal width"
   - Evaluate WcWidth.NET or similar packages if available
   - Review System.Globalization capabilities for Unicode handling

2. **Algorithm Analysis**:
   - Study wcwidth algorithm from Unicode Standard Annex #11
   - Understand East Asian Width property categories (F, H, W, Na, A, N)
   - Research zero-width character handling (combining marks, control chars)

### Critical Use Cases

#### Tabs Widget Support
The tabs widget requires accurate width calculation for:
- **Tab Layout**: Determining if tabs fit within available horizontal space
- **Divider Spacing**: Proper positioning of separators between tabs
- **Padding Calculation**: Consistent spacing around tab titles
- **CJK Character Support**: Accurate width for Chinese, Japanese, Korean characters
- **Mixed Content**: Handling tabs with different character types

#### Table Widget Support
Tables need width calculation for:
- **Column Sizing**: Determining column widths based on content
- **Cell Alignment**: Proper text alignment within cells
- **Row Layout**: Ensuring consistent row formatting

### Implementation Strategy

```csharp
public static class UnicodeWidth
{
    // Primary width calculation method
    public static int GetWidth(string text);
    public static int GetWidth(char c);
    public static int GetWidth(Rune rune);
    
    // Grapheme cluster width (most accurate for display)
    public static int GetGraphemeWidth(string graphemeCluster);
    
    // Utility methods
    public static bool IsZeroWidth(Rune rune);
    public static bool IsWideChar(Rune rune);
    public static bool IsCombiningMark(Rune rune);
}
```

### East Asian Width Categories
- **F (Fullwidth)**: Width = 2 (CJK characters, full-width punctuation)
- **H (Halfwidth)**: Width = 1 (Halfwidth CJK characters)  
- **W (Wide)**: Width = 2 (Wide characters, some symbols)
- **Na (Narrow)**: Width = 1 (ASCII characters, Latin-1)
- **A (Ambiguous)**: Width = 1 or 2 (context-dependent, default to 1)
- **N (Neutral)**: Width = 1 (Most other characters)

### Zero-Width Character Handling
- **Control Characters**: Width = 0 (except \t which varies)
- **Combining Marks**: Width = 0 (overlay on previous character)
- **Zero-Width Joiners/Non-Joiners**: Width = 0
- **Format Characters**: Width = 0

## Key Challenges

### Ambiguous Width Characters
- **Challenge**: Some characters have context-dependent width
- **Approach**: 
  - Default ambiguous characters to width 1 (standard approach)
  - Consider configuration option for East Asian contexts
  - Document behavior clearly in API

### Emoji and Modern Unicode
- **Challenge**: Emoji width varies by terminal implementation
- **Approach**:
  - Follow Unicode standard recommendations
  - Most emoji are width 2
  - Handle skin tone modifiers and ZWJ sequences
  - Test against common terminal emulators

### Performance Optimization
- **Challenge**: Width calculation called frequently during rendering
- **Approach**:
  - Cache width values for common characters
  - Use lookup tables for ASCII range (0-127)
  - Consider caching recent width calculations
  - Profile and optimize hot paths

## Related Components

- **Text System**: All text rendering depends on accurate width calculation
- **Paragraph Widget**: Text wrapping and alignment
- **Buffer System**: Cursor positioning and cell allocation
- **Layout System**: Text measurement for layout calculations

## Integration Points

### System.Globalization Integration
- Use StringInfo.GetTextElementEnumerator() for grapheme clusters
- Leverage Rune type for Unicode scalar value operations
- Consider CharUnicodeInfo for character category information

### Terminal Compatibility
- Test width calculations against various terminal emulators
- Document any terminal-specific behavior
- Provide fallback mechanisms for unsupported characters

## Testing Approach

### Character Category Tests
```csharp
[Test]
public void FullwidthCharacters_ReturnWidth2()
{
    Assert.AreEqual(2, UnicodeWidth.GetWidth('あ')); // Hiragana
    Assert.AreEqual(2, UnicodeWidth.GetWidth('中')); // CJK
    Assert.AreEqual(2, UnicodeWidth.GetWidth('！')); // Fullwidth punctuation
}

[Test]  
public void CombiningMarks_ReturnWidth0()
{
    Assert.AreEqual(0, UnicodeWidth.GetWidth('\u0300')); // Combining grave
    Assert.AreEqual(1, UnicodeWidth.GetWidth("é")); // e + combining acute
}

[Test]
public void EmojiCharacters_ReturnWidth2()
{
    Assert.AreEqual(2, UnicodeWidth.GetWidth("😃")); // Basic emoji
    Assert.AreEqual(2, UnicodeWidth.GetWidth("👍🏽")); // Emoji with modifier
}
```

### Integration Tests
- Test with actual terminal rendering to verify accuracy
- Compare with reference implementations (wcwidth library)
- Test edge cases: empty strings, null inputs, malformed Unicode

### Performance Tests
- Benchmark width calculation for large text blocks
- Profile memory usage and allocation patterns
- Compare performance with/without caching

## Implementation Notes

### Unicode Database Integration
- Consider embedding Unicode data tables vs. using .NET's Unicode support
- Balance between accuracy and binary size
- Plan for Unicode version updates

### Error Handling
- Handle invalid Unicode sequences gracefully
- Return sensible defaults for unmapped characters
- Log warnings for unexpected character categories

### Platform Considerations
- Verify behavior across different .NET runtimes
- Test on Windows, Linux, macOS terminals
- Document platform-specific differences if any

## Acceptance Criteria

- [ ] Accurate width calculation for ASCII characters (width 1)
- [ ] Correct handling of CJK characters (width 2)
- [ ] Zero width for combining marks and control characters
- [ ] Proper emoji width calculation (typically width 2)
- [ ] Grapheme cluster support for complex characters
- [ ] Performance acceptable for text rendering use cases
- [ ] Integration with System.Globalization.StringInfo
- [ ] Comprehensive test coverage including edge cases
- [ ] Documentation and usage examples
- [ ] Verified against reference wcwidth implementations

## See Also

- `SPEC-TEXT-001.md` - Text system specification
- `WIDGET-PARAGRAPH-001` - Paragraph widget implementation
- `TEXT-WRAPPING-001` - Text wrapping implementation
- `TEXT-ALIGNMENT-001` - Text alignment implementation
- Unicode Standard Annex #11: East Asian Width
- wcwidth library reference implementation