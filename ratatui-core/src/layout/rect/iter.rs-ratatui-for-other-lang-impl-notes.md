# Ratatui Rect Iterators Implementation Notes

## Overview
The `rect/iter.rs` file implements three iterator types for working with rectangular areas in terminal space:

1. **Rows** - Iterates over horizontal rows of a rectangle, yielding one-height rectangles
2. **Columns** - Iterates over vertical columns of a rectangle, yielding one-width rectangles
3. **Positions** - Iterates over all positions (points) within a rectangle in row-major order

## Core Data Structures
This module depends on two key structures:

- **Rect**: A rectangle defined by `x`, `y`, `width`, and `height` (all `u16`)
- **Position**: A point in 2D space defined by `x` and `y` coordinates (both `u16`)

## Implementation Details

### Coordinate System
- Origin (0,0) is at the top-left corner
- X-axis increases to the right
- Y-axis increases downward
- Uses unsigned 16-bit integers (0-65535) for all coordinates and dimensions

### Iteration Patterns
- **Rows** and **Columns** both support bidirectional iteration via `DoubleEndedIterator`
- **Positions** iterates in row-major order (left-to-right, then top-to-bottom)
- All iterators implement `size_hint()` for optimization purposes
- Iterators handle edge cases gracefully (zero width/height rectangles)

### Notable Functions
Each iterator implements:
- `new()` - Creates a new iterator from a Rect
- `next()` - Gets the next item in forward iteration
- `size_hint()` - Provides size optimization hints

Rows and Columns additionally implement:
- `next_back()` - Gets the next item in backward iteration

## Cross-Platform Considerations

### Platform Independence
The implementation is pure geometry with no platform-specific code:
- No OS-specific dependencies
- No terminal driver dependencies
- No character encoding considerations at this level

### Porting to Other Languages
When implementing in another language, ensure:

1. **Integer precision**: Use unsigned integers with at least 16 bits of precision
2. **Coordinate system**: Maintain the top-left origin convention
3. **Iteration patterns**: Preserve row-major order for positions and bidirectional capabilities
4. **Boundary handling**: Correctly handle edge cases like zero-sized rectangles and overflow prevention

### Testing Considerations
The original implementation includes comprehensive tests that verify:
- Forward iteration
- Backward iteration
- Bidirectional iteration meeting in the middle
- Edge cases (maximum and minimum coordinate values)
- Zero dimension handling
- Size hint accuracy

These test cases should be replicated when porting to another language to ensure compatibility.