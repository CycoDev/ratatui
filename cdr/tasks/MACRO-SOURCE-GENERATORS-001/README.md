# Source Generators for Compile-Time Convenience

## Overview

Implement C# source generators to provide compile-time code generation for advanced macro-like functionality that cannot be efficiently achieved through runtime APIs alone.

## Implementation Approach

### Constraint DSL Generator
- Parse constraint definition strings at compile time
- Generate optimized constraint arrays
- Support symbolic notation matching Ratatui macro syntax
- Provide compile-time validation of constraint syntax

### Text Template Generator
- Generate strongly-typed text creation methods from templates
- Support style interpolation within templates
- Compile-time validation of template syntax
- Generate efficient runtime code with minimal allocations

### Layout Pattern Generator
- Generate common layout patterns from declarative specifications
- Create type-safe layout builders for specific patterns
- Support complex nested layout definitions
- Optimize generated code for performance

### Attribute-Based Generation
- Use attributes to mark targets for generation
- Support configuration through attribute parameters
- Generate additional metadata and helper methods
- Integrate with existing type hierarchies

## Key Challenges

### Source Generator Complexity
- Source generators are significantly more complex than runtime APIs
- Debugging generated code can be challenging
- Compilation performance impact must be considered
- IDE integration and IntelliSense support varies

### Syntax Design and Parsing
- Design domain-specific language syntax for constraints and templates
- Implement robust parsing with good error messages
- Handle edge cases and validation gracefully
- Provide meaningful compile-time diagnostics

### Generated Code Quality
- Generate readable and maintainable code
- Optimize for runtime performance
- Minimize allocations and boxing
- Support debugger step-through in generated code

### Integration and Compatibility
- Ensure compatibility across .NET versions
- Handle different project types and configurations
- Integrate with build systems and CI/CD
- Support hot reload and incremental compilation

## Related Components

- Layout engine (Constraint, Layout)
- Text system (Text, Line, Span)
- Style system (Style, Color, Modifier)
- Build and compilation pipeline

## Integration Points

### With Runtime APIs
- Generated code should use standard runtime APIs
- Seamless integration with hand-written code
- Support for mixed generation and runtime patterns
- Consistent API surface across generated and manual code

### With Development Tools
- Excellent Visual Studio and VS Code integration
- Clear error messages and diagnostics
- Support for IntelliSense on generated members
- Debugging support for generated code

## Testing Approach

### Generator Testing
- Unit test source generator logic in isolation
- Test syntax parsing and validation
- Verify generated code correctness
- Test error handling and diagnostics

### Integration Testing
- Test generated code in real project scenarios
- Verify compilation performance impact
- Test with various project configurations
- Validate IDE integration and tooling support

### Generated Code Testing
- Unit test generated methods and properties
- Performance test generated code efficiency
- Test generated code debugging experience
- Validate generated code maintainability

## Implementation Strategy

### Phase 1: Constraint DSL Generator
```csharp
[Constraints("==50, >=20, *=1, <=100")]
public static partial class LayoutConstraints
{
    // Generator produces:
    // public static readonly Constraint[] Default = new[]
    // {
    //     Constraint.Length(50),
    //     Constraint.Min(20),
    //     Constraint.Fill(1),
    //     Constraint.Max(100)
    // };
}
```

### Phase 2: Text Template Generator
```csharp
[TextTemplate(@"
Name: {name:bold.green}
Status: {status:italic.blue}
Count: {count:red}
")]
public static partial Text CreateUserStatus(string name, string status, int count);

// Generator produces optimized method implementation
```

### Phase 3: Layout Pattern Generator
```csharp
[LayoutPattern("vertical(header=fixed(3), content=fill, footer=fixed(2))")]
public static partial class MainLayout
{
    // Generator produces type-safe layout creation methods
    // and helper types for accessing specific areas
}
```

## Performance Considerations

### Compilation Time
- Minimize generator execution time
- Cache parsed results when possible
- Incremental generation for large projects
- Efficient syntax tree manipulation

### Generated Code Efficiency
- Generate optimal runtime code
- Minimize memory allocations
- Use efficient collection initialization
- Inline simple operations where possible

### Development Experience
- Fast incremental compilation
- Responsive IntelliSense
- Quick error detection and reporting
- Minimal impact on build times

## Acceptance Criteria

- [ ] Constraint DSL supports all Ratatui constraint patterns
- [ ] Text templates provide type-safe interpolation
- [ ] Generated code is readable and debuggable
- [ ] Compilation time impact is minimal (< 10% increase)
- [ ] IDE integration provides excellent developer experience
- [ ] Error messages are clear and actionable
- [ ] Generated code performance matches hand-written equivalents
- [ ] Documentation includes comprehensive generation examples

## See Also

- SPEC-MACROS-001: Macro system specification
- MACRO-BUILDER-PATTERNS-001: Builder pattern implementation
- MACRO-EXTENSION-METHODS-001: Extension method implementation
- 010-MACRO-SYSTEM-001: Macro system feature requirements