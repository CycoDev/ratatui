# Layout Flex Distribution Algorithms

## Overview

Implement the specific mathematical algorithms for each flex distribution type. This task focuses on the core algorithms that distribute space according to different flex strategies, ensuring accurate and efficient space allocation.

## Implementation Approach

1. **Algorithm Research and Design**:
   - Analyze Ratatui's flex behavior and examples
   - Design mathematical formulas for each distribution type
   - Plan handling of rounding and precision issues

2. **Core Algorithm Implementation**:
   - Implement DistributeLegacy() with constraint priority integration
   - Implement alignment algorithms (Start, End, Center)
   - Implement distribution algorithms (SpaceBetween, SpaceEvenly, SpaceAround)

3. **Edge Case Handling**:
   - Handle single element scenarios
   - Manage insufficient space situations
   - Address rounding and precision in integer arithmetic

4. **Performance Optimization**:
   - Minimize memory allocations
   - Optimize calculation loops
   - Cache common calculations where beneficial

## Key Challenges

1. **Legacy Algorithm Complexity**: Understanding and implementing constraint priority interaction
2. **Integer Arithmetic Precision**: Ensuring accurate distribution without floating point errors
3. **Rounding Strategies**: Deciding how to handle remainder pixels in uneven distributions
4. **Performance vs. Accuracy**: Balancing calculation speed with distribution precision

## Implementation Notes

### Legacy Distribution Algorithm
- Must interact with constraint priority system (Min, Max, Length, Percentage, Ratio, Fill)
- Excess space goes to lowest priority constraints
- Must maintain exact compatibility with Ratatui behavior

### Alignment Algorithms
- **Start**: Elements at beginning, excess at end
- **End**: Elements at end, excess at beginning  
- **Center**: Equal excess before and after element group

### Distribution Algorithms
- **SpaceBetween**: Divide excess by (n-1) gaps between elements
- **SpaceEvenly**: Divide excess by (n+1) gaps including edges
- **SpaceAround**: Each element gets equal space around it (edge space = half element space)

### Rounding Strategy
```csharp
// Example approach for handling remainder
var spacePerGap = excessSpace / gapCount;
var remainder = excessSpace % gapCount;

// Distribute remainder across first 'remainder' gaps
for (int i = 0; i < gapCount; i++)
{
    var gapSpace = spacePerGap + (i < remainder ? 1 : 0);
    // Apply gap space
}
```

## Testing Approach

1. **Algorithm Unit Tests**: Test each distribution method in isolation
2. **Mathematical Verification**: Verify that distributed space equals total space
3. **Precision Tests**: Test with various space/element combinations for rounding
4. **Comparison Tests**: Compare results with Ratatui examples where available
5. **Performance Tests**: Benchmark algorithms with large constraint arrays

## Performance Considerations

- Use spans and stack allocation for temporary arrays where possible
- Minimize heap allocations in hot paths
- Consider caching for repeated layout calculations
- Profile with realistic constraint combinations

## Acceptance Criteria

- [ ] All flex distribution algorithms are implemented
- [ ] Legacy algorithm correctly implements constraint priority
- [ ] Mathematical accuracy is verified (distributed space = total space)
- [ ] Rounding is handled consistently and predictably
- [ ] Edge cases (single element, zero space) are handled gracefully
- [ ] Performance meets requirements for responsive UI
- [ ] Algorithms match Ratatui behavior where documented
- [ ] Unit tests cover all algorithms and edge cases
- [ ] Documentation explains mathematical approach for each algorithm

## See Also

- [LAYOUT-FLEX-001](../LAYOUT-FLEX-001/README.md): Flex system implementation
- [LAYOUT-CONSTRAINTS-001](../LAYOUT-CONSTRAINTS-001/README.md): Constraint system (for Legacy flex)
- [SPEC-LAYOUT-004.md](../../specs/SPEC-LAYOUT-004.md): Layout engine specification