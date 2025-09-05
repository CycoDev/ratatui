# Canvas Rectangle Shape Implementation

## Overview

Implement the Rectangle shape for the Canvas widget, providing rectangular outline drawing capabilities with configurable position, size, and color. The Rectangle uses mathematical coordinates and decomposes into four line segments for rendering.

## Implementation Approach

### Rectangle Structure
- Create Rectangle class implementing IShape interface
- Support floating-point coordinates (X, Y, Width, Height)
- Position from bottom-left corner (mathematical convention)
- Single color for entire rectangle outline

### Drawing Algorithm
- Decompose rectangle into 4 line segments
- Create lines for: left edge, top edge, right edge, bottom edge
- Delegate actual line drawing to Line.Draw() method
- Maintain consistent color across all edges

### Coordinate System
- Follow mathematical coordinate system (bottom-left origin)
- Use floating-point precision for sub-cell accuracy
- Rely on Canvas coordinate transformation to screen coordinates

## Key Challenges

### Coordinate Convention
- **Challenge**: Rectangle positioned from bottom-left corner differs from typical UI conventions
- **Solution**: Document clearly and maintain consistency with Canvas mathematical coordinate system

### Line Decomposition Efficiency
- **Challenge**: Creating 4 Line objects for each rectangle may be inefficient
- **Solution**: Accept overhead for simplicity and consistency with Shape interface design

### Edge Case Handling
- **Challenge**: Zero-width or zero-height rectangles
- **Solution**: Allow such rectangles as they may represent valid degenerate cases (lines or points)

## Related Components

### Direct Dependencies
- `IShape` interface for drawing protocol
- `Line` class for edge rendering
- `IPainter` interface for coordinate transformation
- `Color` type for styling

### Integration Components
- `Canvas` widget as container
- `ICanvasContext` for drawing operations
- Grid implementations for actual rendering
- Marker types for rendering style

## Integration Points

### Shape Interface
- Must implement IShape.Draw(IPainter) method
- Should follow established patterns from other shapes
- Handle painter coordinate transformation properly

### Canvas System
- Integrate with Canvas drawing pipeline
- Support all marker types (Braille, HalfBlock, etc.)
- Work within Canvas coordinate bounds

### Line Dependencies
- Rely on Line class for actual edge drawing
- Pass color information to each line segment
- Maintain coordinate precision through line drawing

## Implementation Notes

### Constructor Design
```csharp
public Rectangle(double x, double y, double width, double height, Color color)
{
    X = x;
    Y = y;
    Width = width;
    Height = height;
    Color = color;
}
```

### Drawing Implementation
```csharp
public void Draw(IPainter painter)
{
    var lines = new[]
    {
        new Line(X, Y, X, Y + Height, Color),                    // Left edge
        new Line(X, Y + Height, X + Width, Y + Height, Color),   // Top edge  
        new Line(X + Width, Y, X + Width, Y + Height, Color),    // Right edge
        new Line(X, Y, X + Width, Y, Color)                      // Bottom edge
    };
    
    foreach (var line in lines)
    {
        line.Draw(painter);
    }
}
```

### Property Design
- All properties should be mutable for flexibility
- Use standard double type for coordinates
- Follow established naming conventions

## Testing Approach

### Unit Tests
- Test rectangle creation with various coordinates
- Verify line decomposition correctness
- Test edge cases (zero dimensions, negative coordinates)
- Validate color propagation to lines

### Integration Tests  
- Test rectangle rendering with different markers
- Verify coordinate transformation works correctly
- Test rectangles at canvas boundaries
- Validate clipping behavior

### Visual Tests
- Create test canvases with rectangles of various sizes
- Test with different marker types (Block, HalfBlock, Braille)
- Verify visual output matches expected patterns
- Test overlapping rectangles for layering behavior

## Performance Considerations

### Memory Allocation
- Rectangle creates 4 Line objects on each draw call
- Consider object pooling if performance becomes an issue
- Line objects are short-lived and should be GC-friendly

### Drawing Efficiency
- Leverages optimized line drawing algorithms
- No redundant coordinate calculations
- Efficient iteration over fixed-size line array

## Acceptance Criteria

### AC-1: Rectangle Structure
- Rectangle class implements IShape interface
- Properties for X, Y, Width, Height, and Color are available
- Constructor accepts all required parameters

### AC-2: Drawing Implementation
- Rectangle.Draw() decomposes into 4 line segments correctly
- Lines are positioned at proper coordinates (left, top, right, bottom edges)
- Color is propagated to all line segments

### AC-3: Canvas Integration
- Rectangle can be drawn on Canvas through ICanvasContext.Draw()
- Works with all supported marker types
- Handles coordinate transformation properly

### AC-4: Edge Cases
- Zero-width rectangles render as vertical lines
- Zero-height rectangles render as horizontal lines
- Negative dimensions are handled gracefully
- Out-of-bounds rectangles are clipped appropriately

### AC-5: Visual Output
- Rectangle renders as expected outline in terminal
- Different marker types produce appropriate visual representation
- Color styling is applied correctly
- Multiple rectangles can be drawn without interference

## See Also

- [SPEC-CANVAS-001](../../specs/SPEC-CANVAS-001.md): Canvas system specification
- [WIDGET-CANVAS-LINE-001](../WIDGET-CANVAS-LINE-001/README.md): Line shape implementation
- [WIDGET-CANVAS-001](../WIDGET-CANVAS-001/README.md): Canvas widget infrastructure
- [007-CANVAS-SYSTEM-001](../../features/007-CANVAS-SYSTEM-001.md): Canvas system features