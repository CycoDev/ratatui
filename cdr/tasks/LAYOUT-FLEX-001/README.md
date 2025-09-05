# Layout Flex Distribution System

## Overview

Implement the Flex enum and distribution system for controlling how extra space is distributed among layout segments in a container. This task focuses on creating the flex distribution strategies that complement the constraint system.

## Implementation Approach

1. **Create Flex Enum**:
   - Define Flex enum with all variants (Legacy, Start, End, Center, SpaceBetween, SpaceEvenly, SpaceAround)
   - Add appropriate attributes and documentation
   - Implement ToString() override for debugging

2. **Implement Distribution Algorithms**:
   - Create FlexDistribution static class with distribution methods
   - Implement mathematical algorithms for each flex type
   - Handle edge cases (single element, rounding, zero space)

3. **Integration with Layout System**:
   - Extend Layout class to accept Flex parameter
   - Integrate flex distribution with constraint resolution
   - Ensure compatibility with existing layout algorithms

## Key Challenges

1. **Mathematical Precision**: Ensuring accurate space distribution with integer arithmetic
2. **Edge Case Handling**: Properly handling scenarios like single elements or insufficient space
3. **Performance**: Optimizing distribution calculations for responsive layouts
4. **Legacy Compatibility**: Maintaining backward compatibility with existing layout behavior

## Related Components

- `Constraint` system (for Legacy flex priority handling)
- `Layout` class (for integration with existing layout algorithms)
- `Rect` operations (for final area calculations)

## Integration Points

- Layout splitting methods must accept Flex parameter
- Constraint resolution must work with flex distribution
- Rectangle calculation must use flex-distributed positions and sizes

## Testing Approach

1. **Unit Tests**: Test each flex distribution algorithm independently
2. **Edge Case Tests**: Test with single elements, zero space, rounding scenarios
3. **Integration Tests**: Test flex with various constraint combinations
4. **Visual Tests**: Create demonstration layouts showing each flex type

## Acceptance Criteria

- [ ] Flex enum is implemented with all variants
- [ ] All distribution algorithms are implemented and tested
- [ ] Legacy flex properly integrates with constraint priority system
- [ ] Edge cases are handled gracefully
- [ ] Performance is optimized for typical layout scenarios
- [ ] Documentation includes clear examples of each flex type
- [ ] Integration with Layout class is seamless
- [ ] Backward compatibility is maintained

## See Also

- [SPEC-LAYOUT-004.md](../../specs/SPEC-LAYOUT-004.md): Layout engine specification
- [003-LAYOUT-ENGINE-001.md](../../features/003-LAYOUT-ENGINE-001.md): Layout engine feature
- [LAYOUT-CONSTRAINTS-001](../LAYOUT-CONSTRAINTS-001/README.md): Constraint system implementation