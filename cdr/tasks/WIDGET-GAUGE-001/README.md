# Gauge Widget Implementation

## Overview

Implement the Gauge and LineGauge widgets for displaying progress bars in CycoTui. These widgets provide horizontal progress indicators with different display styles and configuration options.

## Implementation Approach

### Core Components

**Gauge Widget**:
- Horizontal progress bar with centered label
- Support for percentage (0-100) and ratio (0.0-1.0) input
- Optional Unicode enhancement for higher precision
- Dual styling system (widget background + gauge fill)
- Label color inversion over filled areas
- Block wrapper support for borders and titles

**LineGauge Widget**:
- Single-line compact progress indicator
- Left-aligned label display
- Configurable filled/unfilled symbols
- Separate styling for filled and unfilled areas
- Support for predefined symbol sets

### Key Challenges

**Unicode Block Precision**:
- Implement 8-level fractional precision using Unicode block characters
- Map fractional values (0.0-1.0) to appropriate block characters
- Handle edge cases for exact precision boundaries

**Label Positioning and Color Inversion**:
- Calculate center position for Gauge labels
- Implement color inversion for label visibility over filled areas
- Handle label overflow and clipping scenarios

**Validation and Error Handling**:
- Validate percentage (0-100) and ratio (0.0-1.0) input ranges
- Provide clear error messages for invalid inputs
- Handle zero-size and minimal buffer rendering gracefully

**Performance Optimization**:
- Minimize buffer operations during rendering
- Cache calculated positions and dimensions
- Efficient Unicode character selection

## Implementation Notes

### Unicode Block Characters

Create constants for Unicode block characters:
```csharp
public static class BlockSymbols
{
    public const string FULL = "█";
    public const string SEVEN_EIGHTHS = "▉";
    public const string THREE_QUARTERS = "▊";
    public const string FIVE_EIGHTHS = "▋";
    public const string HALF = "▌";
    public const string THREE_EIGHTHS = "▍";
    public const string ONE_QUARTER = "▎";
    public const string ONE_EIGHTH = "▏";
    public const string EMPTY = " ";
}
```

### Builder Pattern Implementation

Follow C# fluent API conventions:
```csharp
public struct Gauge : IWidget, IStyled
{
    public Gauge Percent(int percent) { /* validation and assignment */ }
    public Gauge Ratio(double ratio) { /* validation and assignment */ }
    public Gauge Label<T>(T label) where T : IConvertible<Span> { /* conversion */ }
    public Gauge UseUnicode(bool unicode = true) { /* flag setting */ }
    public Gauge GaugeStyle<S>(S style) where S : IConvertible<Style> { /* style conversion */ }
    public Gauge Block(Block block) { /* block assignment */ }
}
```

### Rendering Algorithm

1. **Setup**: Apply base widget style and render optional block
2. **Calculate Areas**: Determine inner area and label positioning
3. **Progress Calculation**: Convert ratio to pixel-accurate filled width
4. **Fill Rendering**: Render filled area with appropriate symbols
5. **Unicode Enhancement**: Add fractional block character if enabled
6. **Label Rendering**: Position and render label with color handling

## Related Components

**Dependencies**:
- Buffer class for cell manipulation
- Rect struct for area calculations
- Style system for color and text attributes
- Block widget for border wrapper
- Span/Line text components for labels
- Unicode symbol constants

**Integration Points**:
- IWidget interface implementation
- IStyled interface for styling support
- Block extension methods for inner area calculation
- Buffer indexer for efficient cell access

## Testing Approach

### Unit Tests

**Input Validation**:
- Test percentage bounds (0-100)
- Test ratio bounds (0.0-1.0)
- Verify exception throwing for invalid inputs

**Rendering Accuracy**:
- Test progress calculation precision
- Verify Unicode vs non-Unicode rendering
- Test label positioning and clipping
- Verify color inversion behavior

**Edge Cases**:
- Zero-size rendering areas
- Minimal buffer scenarios
- Maximum and minimum progress values
- Empty and very long labels

### Integration Tests

**Buffer Output Verification**:
- Capture rendered buffer contents
- Verify expected character placement
- Test style application correctness
- Validate Unicode character usage

**Block Integration**:
- Test with various border styles
- Verify inner area calculations
- Test title and padding interactions

## Acceptance Criteria

- [ ] Gauge widget supports percentage and ratio input methods
- [ ] Input validation throws appropriate exceptions for invalid ranges
- [ ] Unicode enhancement provides 8-level fractional precision
- [ ] Label positioning works correctly for various label lengths
- [ ] Color inversion functions properly over filled areas
- [ ] LineGauge provides configurable filled/unfilled symbols
- [ ] Both widgets integrate properly with Block wrapper
- [ ] Zero-size and minimal buffer rendering handles gracefully
- [ ] All styling options work as expected
- [ ] Unit tests achieve >95% code coverage
- [ ] Integration tests verify buffer output correctness
- [ ] Performance tests show efficient rendering

## See Also

- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md) - Widget implementation specification
- [SPEC-BUFFER-002](../../specs/SPEC-BUFFER-002.md) - Buffer and rendering model
- [CORE-SYMBOLS-001](../CORE-SYMBOLS-001/README.md) - Symbol system implementation
- [WIDGET-BLOCK-001](../WIDGET-BLOCK-001/README.md) - Block widget implementation