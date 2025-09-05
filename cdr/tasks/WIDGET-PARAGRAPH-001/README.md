# Paragraph Widget Implementation

## Overview

Implement the Paragraph widget for CycoTui, which provides text display capabilities with support for wrapping, alignment, scrolling, and container integration. This is one of the most fundamental text widgets in the system.

## Implementation Approach

### Core Structure
```csharp
public class Paragraph : IWidget, IStylable
{
    private readonly Text _text;
    private readonly Block? _block;
    private readonly Style _style;
    private readonly Wrap? _wrap;
    private readonly Position _scroll;
    private readonly Alignment _alignment;
    
    public Paragraph(Text text) { /* ... */ }
    
    // Fluent builder methods
    public Paragraph Block(Block block) => /* ... */;
    public Paragraph Style(Style style) => /* ... */;
    public Paragraph Wrap(Wrap wrap) => /* ... */;
    public Paragraph Scroll(int vertical, int horizontal) => /* ... */;
    public Paragraph Alignment(Alignment alignment) => /* ... */;
    
    // Convenience methods
    public Paragraph LeftAligned() => Alignment(Alignment.Left);
    public Paragraph Centered() => Alignment(Alignment.Center);
    public Paragraph RightAligned() => Alignment(Alignment.Right);
    
    public void Render(Rect area, Buffer buffer);
}
```

### Text Input Conversion
- Implement implicit conversion operators for common text types
- Support `string`, `Text`, `Line`, `Span` as input types
- Follow builder pattern with fluent API

### Rendering Pipeline
1. **Style Application**: Apply widget style to entire area
2. **Block Rendering**: Render optional block (borders/title) first
3. **Text Area Calculation**: Determine inner area for text content
4. **Text Composition**: Convert text to renderable lines with wrapping/alignment
5. **Scrolling**: Apply vertical/horizontal scroll offsets
6. **Buffer Writing**: Write styled graphemes to buffer

## Key Challenges

### Unicode Width Calculation
- **Challenge**: Accurate character width measurement for proper alignment and wrapping
- **Approach**: 
  - Research existing .NET Unicode width libraries
  - Consider porting unicode-width algorithms from Rust
  - Use System.Globalization.StringInfo for grapheme enumeration
  - Implement custom width calculation if needed

### Text Wrapping Performance
- **Challenge**: Efficient text wrapping for large content blocks
- **Approach**:
  - Implement lazy line composition similar to Rust version
  - Use iterator patterns for on-demand line generation
  - Avoid pre-computing all lines for scrollable content
  - Consider caching for repeated renders with same width

### Line Composer Abstraction
- **Challenge**: Supporting different text wrapping strategies
- **Approach**:
  - Define `ILineComposer` interface
  - Implement `WordWrapper` for wrapped text
  - Implement `LineTruncator` for non-wrapped text
  - Use strategy pattern for pluggable composition

## Related Components

- **Text System**: Text, Line, Span, StyledGrapheme types
- **Buffer System**: Cell, Buffer rendering target  
- **Layout System**: Rect, Alignment, Position types
- **Style System**: Style composition and inheritance
- **Block Widget**: Container integration
- **Unicode Libraries**: Character width measurement

## Integration Points

### Block Widget Integration
- Use composition pattern with optional Block field
- Delegate block rendering before text rendering
- Calculate inner area after block space accounting
- Support style inheritance from block to content

### Buffer System Integration  
- Write directly to buffer cells with position-based indexing
- Apply styling through buffer style operations
- Handle clipping and bounds checking
- Support partial rendering for performance

## Testing Approach

### Unit Tests
- Text wrapping with various widths and content types
- Alignment calculations for different scenarios  
- Scrolling behavior with edge cases
- Unicode character handling and width calculations
- Style composition and inheritance
- Block integration scenarios

### Visual Tests
- Render to test buffer and compare expected output
- Test with various terminal widths and content sizes
- Verify alignment and wrapping behavior visually
- Test Unicode content rendering

### Performance Tests
- Large text blocks with and without wrapping
- Scrolling performance with extensive content
- Memory usage during text composition
- Comparison with baseline implementations

## Acceptance Criteria

- [ ] Support all text input types (string, Text, Line, Span)
- [ ] Implement fluent builder API with method chaining
- [ ] Support Left, Center, Right alignment
- [ ] Implement text wrapping with word boundaries
- [ ] Support whitespace trimming configuration
- [ ] Implement bidirectional scrolling (vertical/horizontal)
- [ ] Integrate seamlessly with Block widget
- [ ] Handle Unicode characters correctly including wide chars
- [ ] Apply styling through composition and inheritance
- [ ] Pass all unit tests including edge cases
- [ ] Performance acceptable for large text blocks
- [ ] API documentation and examples complete

## See Also

- `SPEC-WIDGET-003.md` - Widget implementation specification
- `SPEC-TEXT-001.md` - Text system specification  
- `002-WIDGET-SYSTEM-001.md` - Widget system feature
- `TEXT-WRAPPING-001` - Text wrapping implementation task
- `TEXT-ALIGNMENT-001` - Text alignment implementation task
- `UNICODE-WIDTH-001` - Unicode width calculation task