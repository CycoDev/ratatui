# Cell Structure Implementation

## Overview

Implement the Cell structure that represents a single terminal character cell with content and styling information. This is the fundamental data type for the buffer system and must support Unicode grapheme clusters, style composition, and memory-efficient storage.

## Implementation Approach

**Core Structure:**
```csharp
public struct Cell : IEquatable<Cell>, IFormattable
{
    private string? symbol;
    public Color Foreground { get; set; }
    public Color Background { get; set; }
    #if FEATURE_UNDERLINE_COLOR
    public Color UnderlineColor { get; set; }
    #endif
    public TextModifier Modifier { get; set; }
    public bool Skip { get; set; }
    
    public static readonly Cell Empty;
    
    // Symbol property with space fallback (matches Ratatui behavior)
    public string Symbol => symbol ?? " ";
    
    // Fluent API methods
    public Cell SetSymbol(string? newSymbol);
    public Cell SetChar(char character);
    public Cell SetStyle(Style style);
    public Cell SetForeground(Color color);
    public Cell SetBackground(Color color);
    public Cell SetSkip(bool skip);
    public Cell Reset();
    
    // Box drawing character merging
    public Cell MergeSymbol(string symbol, MergeStrategy strategy);
    
    // Internal zero-width character support
    internal Cell AppendSymbol(string symbol);
    
    // Static factory methods
    public static Cell FromChar(char ch);
    public static Cell FromStyle(Style style);
}
```

**Key Design Decisions:**
- **Struct Type**: Use struct for value semantics and cache-friendly memory layout
- **Nullable Symbol**: Store symbol as nullable string with " " (space) fallback behavior
- **Fluent API**: All mutation methods return new Cell instance for immutable update pattern
- **Memory Optimization**: Consider CompactString alternative for short symbol storage
- **Feature Gates**: Support conditional underline color through preprocessor directives

**Unicode Handling Strategy:**
- Use System.Globalization.StringInfo for grapheme cluster enumeration
- Implement or integrate wcwidth-compatible display width calculation
- Support complex emoji sequences with zero-width joiners
- Handle combining characters and variation selectors properly

**Symbol Merging Implementation:**
- Port box drawing character merging algorithm from Ratatui
- Support exact and fuzzy merge strategies for Unicode limitations
- Enable border collapsing functionality for table/layout widgets

## Key Challenges

- **CompactString Alternative**: Need memory-efficient string storage equivalent to Rust's CompactString
- **Unicode Grapheme Clusters**: .NET string handling vs proper Unicode grapheme boundaries
- **Symbol Merging Algorithm**: Requires Unicode expertise for box drawing character combinations
- **Performance Optimization**: Cell is used extensively in rendering pipeline
- **Equality Semantics**: Custom equality treating null and " " as equivalent for diffing

## Related Components

- **Style System**: Integrates with Color, TextModifier, and Style types
- **Buffer**: Used as element type in buffer content array
- **Unicode Support**: Requires grapheme cluster and width calculation libraries
- **MergeStrategy**: Enum for controlling symbol merging behavior
- **CompactString**: Memory-efficient string storage implementation

## Testing Approach

**Unicode Test Cases:**
- Multi-byte characters (Japanese, Chinese, Arabic)
- Emoji sequences with skin tone modifiers
- Family emoji with zero-width joiners
- Combining characters and diacritical marks
- Zero-width characters and spaces

**Symbol Merging Tests:**
- Box drawing character combinations
- Invalid merge scenarios
- Performance with large symbol sets
- Edge cases with non-box-drawing characters

**Equality and Hashing Tests:**
- Null vs " " symbol equivalence
- Style comparison completeness
- Hash consistency for collections
- Performance of equality operations

## Integration Points

- **Buffer Array**: Cell[] storage in Buffer class
- **Diffing Algorithm**: Equality comparison for change detection
- **Style Application**: Integration with widget styling
- **Terminal Output**: Conversion to terminal escape sequences

## Performance Considerations

- **Memory Layout**: Struct design for cache efficiency
- **String Interning**: Consider for common symbols
- **Allocation Patterns**: Minimize during rendering operations
- **Copy Semantics**: Efficient struct copying for immutable updates

## Acceptance Criteria

- Cell struct properly handles all Unicode grapheme clusters including emoji
- Symbol property returns " " for null/empty symbols matching Ratatui behavior  
- Style application works correctly with all modifier combinations including conditional underline color
- Custom equality comparison treats null and " " symbols as equivalent
- Symbol merging correctly combines box drawing characters for border collapsing
- Memory usage is optimized through CompactString alternative or equivalent
- All mutation methods return new Cell instances enabling immutable update patterns
- Performance suitable for real-time rendering (60+ FPS) on typical terminal sizes
- Integration with buffer diffing algorithm works correctly

## See Also

- SPEC-BUFFER-002: Buffer and Rendering Model  
- SPEC-STYLE-005: Style System Specification
- BUFFER-UNICODE-001: Unicode width calculation implementation
- CORE-SYMBOLS-001: Symbol and box drawing character support