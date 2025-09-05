# Widget Padding Implementation

## Overview

Implement the Padding value type for CycoTui widgets, providing a CSS-like padding system optimized for terminal usage. This type will be used primarily by Block widgets and other container widgets to define internal spacing.

## Implementation Approach

Create a value type (struct) that stores four padding values (left, right, top, bottom) and provides convenient constructor methods for common padding patterns.

### Core Structure
```csharp
public readonly struct Padding : IEquatable<Padding>, IComparable<Padding>
{
    public readonly int Left;
    public readonly int Right; 
    public readonly int Top;
    public readonly int Bottom;
    
    // Constructor methods
    public static Padding Uniform(int value) => new(value, value, value, value);
    public static Padding Horizontal(int value) => new(value, value, 0, 0);
    public static Padding Vertical(int value) => new(0, 0, value, value);
    public static Padding Proportional(int value) => new(2 * value, 2 * value, value, value);
    // ... other constructors
}
```

### Key Decisions
- Use `int` instead of `ushort` for broader .NET compatibility and easier arithmetic
- Make struct readonly and immutable for value semantics
- Provide static factory methods instead of const functions (C# limitation)
- Include XML documentation referencing CSS padding concepts

## Key Challenges

**Terminal Aspect Ratio**: The proportional constructor addresses the fact that terminal characters are typically taller than wide, requiring 2:1 horizontal-to-vertical ratio for visual balance.

**Const vs Static**: Unlike Rust's const fn, C# const is limited to compile-time constants. Use static readonly properties or static methods for computed values.

**API Consistency**: Balance between Ratatui's naming (lowercase) and .NET conventions (PascalCase). Choose .NET conventions for better ecosystem integration.

## Related Components

- **Block Widget**: Primary consumer of Padding type
- **Layout System**: May use padding in area calculations
- **Buffer**: Rendering system must respect padding during widget rendering

## Integration Points

- Must integrate with Block widget implementation
- Should work with layout constraint system for inner area calculations  
- Consider integration with margin types for complete spacing system

## Testing Approach

- Unit tests for all constructor methods and edge cases
- Validation that proportional padding produces 2:1 ratio
- Equality and comparison tests for IEquatable/IComparable
- JSON serialization tests if System.Text.Json support is added

## Acceptance Criteria

- [ ] Padding struct with four integer properties implemented
- [ ] All constructor methods from Ratatui equivalent implemented
- [ ] IEquatable<Padding> and IComparable<Padding> implemented
- [ ] Comprehensive unit tests with 100% code coverage
- [ ] XML documentation for all public members
- [ ] Integration with Block widget (in separate task)
- [ ] Performance benchmarks for constructor methods

## See Also

- SPEC-WIDGET-003.md: Widget Implementation Specification
- SPEC-LAYOUT-004.md: Layout Engine Specification  
- WIDGET-BLOCK-001: Block widget implementation that will use Padding
- Original Ratatui file: ratatui-widgets/src/block/padding.rs