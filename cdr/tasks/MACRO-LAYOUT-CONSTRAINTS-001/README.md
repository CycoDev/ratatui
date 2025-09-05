# Layout Constraint Macros Implementation

## Overview

Implement C# equivalent functionality for Ratatui's layout constraint macros (`constraint!`, `constraints!`, `vertical!`, `horizontal!`). This task focuses on providing ergonomic APIs for creating layout constraints that match the convenience of Ratatui's macro system.

## Implementation Approach

### Phase 1: Extension Methods for Constraint Creation
Create extension methods on integer types to provide fluent constraint syntax:

```csharp
public static class ConstraintExtensions
{
    public static Constraint Fixed(this int value) => Constraint.Length((ushort)value);
    public static Constraint Percent(this int value) => Constraint.Percentage((ushort)value);
    public static Constraint Fill(this int value) => Constraint.Fill((ushort)value);
    public static Constraint Min(this int value) => Constraint.Min((ushort)value);
    public static Constraint Max(this int value) => Constraint.Max((ushort)value);
}
```

### Phase 2: Layout Creation Methods
Enhance Layout class with constraint array overloads:

```csharp
public static class Layout
{
    public static Layout Vertical(params Constraint[] constraints);
    public static Layout Horizontal(params Constraint[] constraints);
    
    // Collection initializer support
    public static Layout Vertical(IEnumerable<Constraint> constraints);
    public static Layout Horizontal(IEnumerable<Constraint> constraints);
}
```

### Phase 3: Advanced Constraint Builders
For complex scenarios not handled by extension methods:

```csharp
public static class ConstraintBuilder
{
    public static Constraint Ratio(int numerator, int denominator) => 
        Constraint.Ratio((uint)numerator, (uint)denominator);
}
```

### Phase 4: Collection Support
Enable collection initializer syntax for constraint arrays:

```csharp
public class ConstraintCollection : List<Constraint>
{
    // Implicit conversion to Constraint[]
    public static implicit operator Constraint[](ConstraintCollection collection)
        => collection.ToArray();
}
```

## Key Challenges

1. **Type Safety**: Ensure constraint values are within valid ranges (ushort limits)
2. **API Discoverability**: Make constraint creation intuitive through IntelliSense
3. **Performance**: Minimize allocation overhead in constraint creation
4. **Syntax Compatibility**: Match Ratatui's ergonomic syntax as closely as possible

## Related Components

- `Constraint` enum and its variants
- `Layout` class for applying constraints
- Extension method infrastructure
- Collection initializer support

## Integration Points

- Must work with existing layout calculation engine
- Should integrate with fluent API patterns throughout CycoTui
- Extension methods should appear in IntelliSense for integer literals

## Testing Approach

### Unit Tests
- Test all extension methods with valid and edge case values
- Verify constraint type mappings match expected Ratatui behavior
- Test collection initializer syntax compilation and runtime behavior

### Integration Tests  
- Test layout creation with various constraint combinations
- Verify constraint resolution in actual layout scenarios
- Test performance with large constraint arrays

### Syntax Tests
```csharp
[Test]
public void ConstraintSyntax_MatchesRatatuiEquivalents()
{
    // constraint!(==50) equivalent
    var c1 = 50.Fixed();
    Assert.AreEqual(Constraint.Length(50), c1);
    
    // constraint!(>=20) equivalent  
    var c2 = 20.Min();
    Assert.AreEqual(Constraint.Min(20), c2);
    
    // vertical![==1, *=1, >=3] equivalent
    var layout = Layout.Vertical(
        1.Fixed(),
        1.Fill(), 
        3.Min()
    );
    
    // Verify layout was created correctly
    Assert.IsNotNull(layout);
}
```

## Performance Considerations

- Extension methods should inline to direct constructor calls
- Avoid boxing of integer values where possible
- Use struct-based constraint types to minimize allocations
- Consider compile-time validation for literal values

## Acceptance Criteria

- [ ] Extension methods provide fluent constraint creation syntax
- [ ] Layout.Vertical() and Layout.Horizontal() accept constraint arrays
- [ ] Collection initializer syntax works for constraint creation  
- [ ] All constraint types from Ratatui are supported
- [ ] Performance is equivalent to direct Constraint constructor calls
- [ ] IntelliSense shows constraint extension methods on integer literals
- [ ] Unit tests cover all constraint creation patterns
- [ ] Integration tests verify layout behavior with macro-created constraints

## See Also

- SPEC-MACROS-001: Macro system specification
- SPEC-LAYOUT-004: Layout engine specification  
- MACRO-EXTENSION-METHODS-001: Extension method patterns
- LAYOUT-CONSTRAINTS-001: Core constraint implementation