# Canvas Grid System Implementation

## Overview

Implement the grid system that provides different drawing surfaces for the Canvas widget. This includes the IGrid interface and concrete implementations for Braille, HalfBlock, and Character grids with different resolutions and capabilities.

## Implementation Approach

### Grid Interface Design
- Define IGrid interface with common operations (Paint, Save, Reset, Resolution)
- Create ILayer interface for saved grid states
- Implement grid factory for creating appropriate grid types

### Grid Implementations
- **BrailleGrid**: 2x4 dots per cell using Unicode Braille patterns
- **HalfBlockGrid**: 1x2 pixels per cell using half-block characters
- **CharGrid**: 1x1 character per cell using configurable characters

### Unicode Handling
- Implement Braille pattern encoding using bitwise operations
- Handle UTF-16 code point manipulation for Braille characters
- Support half-block character selection based on pixel patterns

## Key Challenges

### Unicode Pattern Encoding
- **Challenge**: Efficiently encoding 2x4 dot patterns into Braille Unicode characters
- **Solution**: Use bitwise OR operations with pre-computed pattern masks

### Half-Block Color Logic
- **Challenge**: Representing two pixels per cell using foreground/background colors
- **Solution**: Implement logic to choose appropriate half-block character and colors

### Memory Efficiency
- **Challenge**: Optimizing memory usage for large grids
- **Solution**: Use appropriate data structures for each grid type

### Bounds Checking
- **Challenge**: Preventing index overflow and panic conditions
- **Solution**: Implement saturating arithmetic and bounds validation

## Implementation Notes

### BrailleGrid Details
```csharp
// Braille pattern bit masks for 2x4 dot pattern
private static readonly ushort[,] DotMasks = new ushort[4, 2]
{
    { 0x01, 0x08 }, // Row 0: dots 1, 4
    { 0x02, 0x10 }, // Row 1: dots 2, 5  
    { 0x04, 0x20 }, // Row 2: dots 3, 6
    { 0x40, 0x80 }  // Row 3: dots 7, 8
};
```

### HalfBlockGrid Color Logic
- Space character: both pixels reset
- Lower half block (▄): upper reset, lower colored
- Upper half block (▀): upper colored, lower reset/different
- Full block (█): both pixels same color

### Performance Considerations
- Use array-based storage for optimal access patterns
- Implement efficient index calculations
- Minimize allocations during drawing operations

## Related Components

- `Marker` enum for grid type selection
- `Color` types for pixel/cell coloring
- Unicode symbol constants from symbols module

## Testing Approach

### Unit Tests
- Test each grid type independently
- Verify coordinate-to-index calculations
- Test paint operations with various coordinates
- Validate save/reset functionality

### Unicode Tests
- Test Braille pattern encoding/decoding
- Verify half-block character selection
- Test UTF-16 string generation

### Performance Tests
- Benchmark grid operations with large sizes
- Test memory usage patterns
- Verify bounds checking doesn't create bottlenecks

### Edge Case Tests
- Test with maximum grid dimensions
- Verify overflow protection
- Test out-of-bounds painting operations

## Acceptance Criteria

### AC-1: Grid Interface
- IGrid interface is properly defined and implemented
- All grid types implement the interface consistently
- Grid factory correctly creates appropriate grid instances

### AC-2: BrailleGrid Implementation
- Properly encodes 2x4 dot patterns into Braille characters
- Supports foreground color for each Braille cell
- Generates correct UTF-16 strings for rendering

### AC-3: HalfBlockGrid Implementation
- Correctly represents 1x2 pixels per terminal cell
- Chooses appropriate half-block characters based on pixel colors
- Supports full foreground/background color control

### AC-4: CharGrid Implementation
- Supports configurable characters for drawing
- Handles color assignment correctly
- Works with standard drawing characters (dot, block, bar)

### AC-5: Performance and Reliability
- Grid operations complete in reasonable time for large grids
- Bounds checking prevents crashes and panics
- Memory usage scales predictably with grid size

## See Also

- [SPEC-CANVAS-001](../../specs/SPEC-CANVAS-001.md): Canvas system specification
- [WIDGET-CANVAS-001](../WIDGET-CANVAS-001/README.md): Canvas widget infrastructure
- [WIDGET-CANVAS-COORDINATES-001](../WIDGET-CANVAS-COORDINATES-001/README.md): Coordinate transformation