# CompactString Implementation for Cell Storage

## Overview

Implement a memory-efficient string storage alternative to optimize Cell symbol storage. This addresses the need for a .NET equivalent to Rust's CompactString, which uses inline storage for short strings to reduce memory overhead.

## Implementation Approach

**Core Design:**
```csharp
public readonly struct CompactString : IEquatable<CompactString>, IComparable<CompactString>
{
    private readonly object storage; // Either string or inline data
    
    public static CompactString Empty { get; }
    public int Length { get; }
    public bool IsEmpty { get; }
    
    // Constructors
    public CompactString(string value);
    public CompactString(char value);
    public CompactString(ReadOnlySpan<char> value);
    
    // String operations
    public override string ToString();
    public ReadOnlySpan<char> AsSpan();
    
    // Equality and comparison
    public bool Equals(CompactString other);
    public int CompareTo(CompactString other);
    
    // Implicit conversions
    public static implicit operator CompactString(string value);
    public static implicit operator string(CompactString value);
}
```

**Memory Layout Strategy:**
- Use union-like pattern with object field
- Store short strings (≤16 chars) inline using unsafe code or PrivateImplementationDetails
- Store longer strings as regular string references
- Optimize for common single-character symbols used in terminal rendering

**Alternative Approaches:**
1. **String Interning**: Use string interning for common symbols
2. **String Pool**: Implement custom string pool for terminal symbols
3. **Direct String**: Use regular string with memory profiling to validate overhead
4. **Hybrid Approach**: Combine interning for common symbols with compact storage for others

## Key Challenges

- **Memory Layout**: Achieving efficient inline storage without unsafe code
- **UTF-16 Handling**: .NET uses UTF-16 vs Rust's UTF-8, affecting storage calculations
- **Performance**: Ensuring no regression compared to direct string usage
- **Compatibility**: Seamless integration with existing .NET string APIs
- **GC Pressure**: Minimizing allocations during frequent Cell operations

## Related Components

- **Cell Structure**: Primary consumer of CompactString
- **Buffer Operations**: High-frequency Cell creation during rendering
- **Symbol Constants**: Common terminal characters and box drawing symbols
- **Memory Management**: Integration with buffer pooling strategies

## Implementation Notes

**Memory Size Calculations:**
- .NET reference: 8 bytes (64-bit) / 4 bytes (32-bit)
- String overhead: ~24 bytes + character data
- Target: inline storage for strings ≤ 8-12 characters
- Break-even point: strings longer than reference size + metadata

**Performance Considerations:**
- Benchmark against regular string usage
- Profile memory usage in typical terminal scenarios
- Measure GC impact during high-frequency rendering
- Optimize for common single-character symbols

**Safety and Correctness:**
- Handle Unicode surrogates correctly
- Maintain string immutability semantics
- Ensure proper equality and hashing behavior
- Support all .NET string comparison options

## Testing Approach

**Functionality Tests:**
- String storage and retrieval accuracy
- Unicode character handling (emoji, CJK, combining)
- Equality and comparison operations
- Conversion to/from regular strings

**Performance Tests:**
- Memory usage compared to string
- Allocation patterns during Cell operations
- GC pressure in rendering scenarios
- Performance impact on buffer operations

**Edge Case Tests:**
- Empty strings and null handling
- Very long strings exceeding inline threshold
- Unicode edge cases (surrogates, normalization)
- Thread safety if applicable

## Integration Points

- **Cell.SetSymbol()**: Primary usage point for symbol storage
- **Buffer Rendering**: High-frequency Cell creation and copying
- **Symbol Constants**: Static storage for common box drawing characters
- **Serialization**: Support for buffer serialization if needed

## Performance Targets

- **Memory Reduction**: 50-70% reduction for typical single-character symbols
- **Performance**: No more than 5% overhead compared to direct string usage
- **GC Pressure**: Measurable reduction in allocation rate during rendering
- **Compatibility**: Drop-in replacement for string in Cell implementation

## Acceptance Criteria

- CompactString successfully reduces memory usage for typical Cell symbols
- Performance impact is minimal (< 5% overhead) compared to direct string usage
- Handles all Unicode characters correctly including emoji and combining marks
- Integrates seamlessly with Cell implementation
- Passes comprehensive testing including edge cases and performance benchmarks
- Documentation includes usage guidelines and performance characteristics
- Memory profiling shows measurable improvement in typical rendering scenarios

## Alternative Solutions

If CompactString proves too complex:
1. **String Interning**: Use string.Intern() for common symbols
2. **Symbol Pool**: Implement dedicated pool for terminal characters
3. **Profiling First**: Measure actual memory impact before optimization
4. **Regular String**: Use string with focused optimization elsewhere

## See Also

- BUFFER-CELL-001: Cell Structure Implementation
- SPEC-BUFFER-002: Buffer and Rendering Model
- CORE-SYMBOLS-001: Symbol and box drawing character support