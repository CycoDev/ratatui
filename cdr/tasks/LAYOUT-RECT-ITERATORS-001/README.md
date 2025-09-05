# Rectangle Iterator Implementation

## Overview

Implement comprehensive iteration capabilities for the Rect type, including row iteration, column iteration, and position iteration. This provides efficient ways to traverse rectangular areas for rendering, hit testing, and other spatial operations.

## Implementation Approach

### Core Iterator Types

Implement three primary iterator types as struct-based enumerables:

1. **RowIterator**: Iterates over horizontal strips (rows) as Rect objects
2. **ColumnIterator**: Iterates over vertical strips (columns) as Rect objects  
3. **PositionIterator**: Iterates over individual positions in row-major order

### Iterator Features

All iterators should provide:
- Standard `IEnumerable<T>` interface for LINQ compatibility
- Accurate count properties for collection optimization
- Efficient struct-based implementation to minimize allocations
- Proper handling of edge cases (zero-width/height rectangles)

### Bidirectional Iteration

Row and Column iterators should support bidirectional iteration:
- Forward iteration (left-to-right for columns, top-to-bottom for rows)
- Backward iteration (right-to-left for columns, bottom-to-top for rows)
- Meeting in the middle (when both directions are used)

## Key Challenges

### C# Iterator Pattern Adaptation

- Rust's `Iterator` and `DoubleEndedIterator` traits don't have direct C# equivalents
- Need to design appropriate interfaces for bidirectional iteration
- Balance between Rust-like functionality and C# idiomatic patterns

### Performance Optimization

- Use struct-based iterators to avoid heap allocations
- Implement efficient bounds checking without overflow
- Provide accurate count/size estimates for collection pre-allocation

### Edge Case Handling

- Zero-width or zero-height rectangles
- Maximum coordinate values (u16::MAX boundaries)
- Proper termination when iterators meet in the middle

## Related Components

- `Rect` struct (core rectangle type)
- `Position` struct (coordinate representation)
- Layout system (for spatial calculations)
- Rendering system (primary consumer of iteration)

## Integration Points

### Rect Type Extension

Add iterator methods to the Rect struct:
```csharp
public IEnumerable<Rect> Rows() => new RowIterator(this);
public IEnumerable<Rect> Columns() => new ColumnIterator(this);
public IEnumerable<Position> Positions() => new PositionIterator(this);
```

### Custom Iterator Interfaces

Consider defining custom interfaces for advanced iteration:
```csharp
public interface IBidirectionalEnumerable<T> : IEnumerable<T>
{
    IEnumerable<T> Reverse();
    int Count { get; }
}
```

## Testing Approach

### Unit Tests

Test each iterator type with:
- Standard forward iteration scenarios
- Backward iteration scenarios (for bidirectional iterators)
- Mixed forward/backward iteration (meeting in middle)
- Edge cases (zero dimensions, single dimension, maximum values)
- Count accuracy verification
- LINQ operation compatibility

### Performance Tests

- Measure iteration speed vs. simple for-loops
- Test memory allocation patterns
- Verify count optimization effectiveness

### Integration Tests

- Test with actual rendering scenarios
- Verify correct behavior in layout operations
- Test with various rectangle sizes and positions

## Acceptance Criteria

- [ ] RowIterator provides bidirectional iteration over rectangle rows
- [ ] ColumnIterator provides bidirectional iteration over rectangle columns  
- [ ] PositionIterator provides row-major iteration over all positions
- [ ] All iterators implement IEnumerable<T> correctly
- [ ] Accurate count/size properties provided for optimization
- [ ] Edge cases handled gracefully (zero dimensions, boundaries)
- [ ] Performance comparable to equivalent for-loop constructs
- [ ] Comprehensive unit test coverage including edge cases
- [ ] Integration with Rect type through extension methods
- [ ] LINQ operations work correctly with all iterator types

## See Also

- [SPEC-LAYOUT-004.md](../../specs/SPEC-LAYOUT-004.md): Layout engine specification
- [003-LAYOUT-ENGINE-001.md](../../features/003-LAYOUT-ENGINE-001.md): Layout engine feature
- File analysis: `ratatui-core/src/layout/rect/iter.rs`