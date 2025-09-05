# Buffer Testing Utilities

## Overview

Implement comprehensive testing and validation utilities for the CycoTui buffer system, enabling detailed comparison and debugging of buffer states during development and testing.

## Implementation Approach

Based on analysis of `ratatui-core/src/buffer/assert.rs`, implement a C# equivalent that provides:

1. **BufferAssert static class** with methods for buffer comparison
2. **Detailed diff reporting** showing position-specific differences  
3. **Integration with C# testing frameworks** through appropriate exception types
4. **Performance-optimized comparison** with lazy error message generation

### Core Components

```csharp
public static class BufferAssert
{
    public static void AreEqual(Buffer expected, Buffer actual, string? message = null);
    public static BufferDifference Compare(Buffer expected, Buffer actual);
    public static string CreateDiffReport(BufferDifference difference);
}

public class BufferAssertionException : Exception
{
    public BufferDifference Difference { get; }
    // Constructor with formatted message
}
```

### Design Decisions

- Use static methods instead of macros (C# doesn't have macro system)
- Provide both throwing and non-throwing comparison methods
- Support custom exception types for better test framework integration
- Use lazy evaluation for expensive diff formatting operations

## Key Challenges

1. **C# vs Rust Patterns**:
   - Replace Rust macro with static method approach
   - Use C# string interpolation instead of Rust format! macro
   - Handle memory allocation differently (GC vs explicit allocation)

2. **Testing Framework Integration**:
   - Support xUnit, NUnit, MSTest assertion patterns
   - Provide extension methods for fluent assertion APIs
   - Ensure exception types work well with test runners

3. **Performance Optimization**:
   - Lazy evaluation of diff formatting (only format on failure)
   - Efficient string building for large diffs
   - Configurable verbosity levels

## Related Components

- **Buffer class**: Core buffer implementation being tested
- **Cell struct**: Individual cell comparison logic
- **BufferDiff**: May share algorithms with rendering diff system
- **Testing frameworks**: Integration points for assertion libraries

## Integration Points

- **CycoTui.Testing namespace**: Dedicated namespace for testing utilities
- **Buffer.Equals()**: May use shared comparison logic
- **Extension methods**: For popular testing frameworks if beneficial

## Testing Approach

Test the testing utilities themselves:

1. **Positive cases**: Verify equal buffers don't throw
2. **Area mismatch**: Verify appropriate exceptions for different areas
3. **Content differences**: Test various types of content differences
4. **Performance**: Ensure diff generation doesn't impact test performance significantly
5. **Integration**: Test with actual C# testing frameworks

## Performance Considerations

- Use StringBuilder for large diff reports
- Implement early termination for area mismatches  
- Consider caching string representations of common cells
- Lazy evaluation of detailed error messages

## Acceptance Criteria

- [ ] BufferAssert.AreEqual() method throws descriptive exceptions for unequal buffers
- [ ] Equal buffers pass assertion without exceptions
- [ ] Diff reports include position, expected cell, and actual cell information
- [ ] Performance is acceptable for typical buffer sizes (80x24)
- [ ] Integration works with at least one major C# testing framework (xUnit recommended)
- [ ] Exception messages are clear and actionable for developers
- [ ] Memory usage is reasonable during diff generation

## See Also

- SPEC-BUFFER-002: Buffer and Rendering Model specification
- 001-BUFFER-MODEL-001: Buffer Model feature document
- Analysis: ratatui-core/src/buffer/assert.rs