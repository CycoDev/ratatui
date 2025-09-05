# Clear Widget Implementation

## Overview

Implement the Clear widget, a utility widget that clears/resets a rectangular area in the buffer to allow overdrawing (e.g., for popup overlays). This is a zero-sized widget with no configuration options that implements the area clearing functionality.

## Implementation Approach

### Core Implementation
- Create a Clear widget class (or static class) following C# conventions
- Implement IWidget interface with Render method
- Use nested loops to iterate through area coordinates
- Call Reset() method on each cell in the specified area
- Support both instance and static usage patterns

### API Design
```csharp
public static class Clear
{
    public static void Render(Rect area, Buffer buffer);
}

// Or as instance widget:
public struct Clear : IWidget
{
    public void Render(Rect area, Buffer buffer);
}
```

### Buffer Clearing Logic
- Iterate through area.Left to area.Right (exclusive)
- Iterate through area.Top to area.Bottom (exclusive)
- Call buffer[x, y].Reset() for each coordinate
- Ensure safe handling of empty areas (no-op)

## Key Challenges

### Zero-Sized Type Translation
- **Challenge**: Rust's zero-sized types don't have direct C# equivalent
- **Solution**: Use static class for utility pattern, or empty struct with singleton behavior
- **Consideration**: Maintain API consistency with other widgets

### Buffer Indexing
- **Challenge**: Adapt Rust's `buf[(x, y)]` indexing to C# conventions
- **Solution**: Use `buffer[x, y]` indexer or `buffer.GetCell(x, y)` method
- **Consideration**: Ensure consistent buffer access patterns

### Delegation Pattern
- **Challenge**: Rust's owned vs borrowed widget pattern
- **Solution**: Not applicable for zero-sized type, use consistent API
- **Consideration**: Document why delegation isn't needed here

## Related Components

### Dependencies
- Buffer system for cell access and Reset() method
- Rect type for area specification
- IWidget interface for polymorphic usage

### Integration Points
- Used primarily for popup implementations
- Often rendered before other widgets in the same area
- Works with all buffer implementations

## Testing Approach

### Unit Tests
- Test clearing empty area (no-op behavior)
- Test clearing single cell area
- Test clearing rectangular areas of various sizes
- Verify cells are properly reset after clearing
- Test area boundary handling

### Integration Tests
- Test Clear widget with popup scenarios
- Verify overdrawing works correctly after clearing
- Test with different buffer implementations

## Performance Considerations

- O(width × height) operation - document performance characteristics
- Consider bulk operations if buffer supports them for large areas
- Profile nested loop vs alternative approaches
- Minimal memory allocation (static implementation preferred)

## Documentation Requirements

### XML Documentation
- Document Clear widget purpose and typical usage
- Include popup example in documentation
- Note limitation about first render (use Terminal.Clear instead)
- Document performance characteristics

### Code Examples
```csharp
// Clear area before rendering popup
Clear.Render(popupArea, buffer);
popup.Render(popupArea, buffer);
```

## Acceptance Criteria

- [ ] Clear widget clears specified rectangular areas correctly
- [ ] Integration with IWidget interface (if using instance pattern)
- [ ] Proper handling of empty areas
- [ ] Performance acceptable for typical popup sizes
- [ ] Comprehensive unit tests covering edge cases
- [ ] Clear documentation with usage examples
- [ ] Consistent with overall widget API patterns

## See Also

- [SPEC-WIDGET-003](../../specs/SPEC-WIDGET-003.md): Widget implementation specification
- [SPEC-BUFFER-002](../../specs/SPEC-BUFFER-002.md): Buffer system specification
- [002-WIDGET-SYSTEM-001](../../features/002-WIDGET-SYSTEM-001.md): Widget system feature