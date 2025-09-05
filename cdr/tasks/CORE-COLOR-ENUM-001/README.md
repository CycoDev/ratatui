# Color Enum Implementation

## Overview

Implement the core `Color` enum and supporting types to provide comprehensive color support for CycoTui, including ANSI colors, RGB colors, indexed colors, and string parsing capabilities based on the Ratatui color implementation.

## Implementation Approach

### Core Color Structure
Create a `Color` readonly struct with these characteristics:
- Support for all ANSI colors (16 basic colors)
- RGB true color support (24-bit)
- Indexed color support (8-bit, 256 colors)
- Reset/default color support
- Efficient memory layout using discriminated union pattern

### String Parsing Implementation
Implement comprehensive string parsing that matches Ratatui's behavior:
- Case-insensitive ANSI color names
- Comprehensive alias support (grey/gray, bright/light prefixes, etc.)
- Hex color parsing (#RRGGBB format)
- Numeric indexed color parsing
- Input normalization (removing separators, handling variants)

### Factory Methods
Provide convenient factory methods and implicit conversions:
- Static properties for all ANSI colors
- `Rgb(byte, byte, byte)` method
- `Indexed(byte)` method  
- `FromU32(uint)` for hex integer values
- Implicit conversions from tuples and arrays

## Key Challenges

### String Parsing Complexity
The string parsing needs to handle many aliases and format variations:
- Multiple separator styles: "light red", "light-red", "light_red"
- Spelling variants: "grey" vs "gray"
- Prefix handling: "bright" → "light"
- Case insensitivity
- Alias mapping: "silver" → "gray", "lightblack" → "darkgray"

### Performance Considerations
- String parsing should be efficient for runtime use
- Consider caching or lookup tables for common color names
- Minimize allocations during parsing
- Use spans and readonly memory where possible

### Platform Color Support Detection
Different terminals support different color capabilities:
- Some only support 16 ANSI colors
- Others support 256 indexed colors
- Modern terminals support 24-bit RGB
- Need runtime detection or configuration

## Related Components

### Style System Integration
- Must integrate seamlessly with `Style` struct
- Support implicit conversions to `Style`
- Work with style composition and patching

### Backend Integration
- Terminal backends need to convert colors to escape sequences
- Different backends may have different color support
- Need conversion utilities for backend-specific types

### Serialization Support
- Optional JSON serialization/deserialization
- Backward compatibility with different serialization formats
- Use efficient string representation for serialization

## Testing Approach

### Unit Tests for Parsing
Test comprehensive color parsing scenarios:
- All ANSI color names and aliases
- Hex color parsing with various cases
- Indexed color parsing
- Error cases and invalid inputs
- Normalization behavior

### Conversion Tests
Test conversions between different representations:
- RGB to/from hex strings
- Validation of all factory methods
- Implicit conversion operators
- Roundtrip serialization tests

### Performance Tests
- Parsing performance for common colors
- Memory allocation during parsing
- Comparison with direct color creation

## Platform-Specific Details

### Windows Considerations
- Ensure compatibility with Windows Terminal and ConHost
- Handle VT sequence support detection
- Test color rendering on different Windows terminal versions

### Unix Considerations
- Standard ANSI escape sequence support
- Terminal capability detection via environment variables
- Handle various terminal emulator differences

## Acceptance Criteria

1. **Color Enum Implementation**:
   - All ANSI colors supported as static properties
   - RGB and Indexed color support
   - Efficient memory layout and value semantics
   - Proper equality and hash code implementation

2. **String Parsing**:
   - All Ratatui color name formats supported
   - Hex color parsing (#RRGGBB)
   - Indexed color parsing (0-255)
   - Comprehensive alias support
   - Proper error handling for invalid inputs

3. **API Convenience**:
   - Factory methods for all color types
   - Implicit conversions from common types
   - Integration with styling system
   - Optional serialization support

4. **Testing**:
   - Comprehensive unit test coverage
   - Performance benchmarks
   - Cross-platform validation
   - Documentation and examples

5. **Documentation**:
   - Complete XML documentation
   - Usage examples
   - Platform compatibility notes
   - Performance guidance

## See Also

- [SPEC-STYLE-005.md](../../specs/SPEC-STYLE-005.md): Style system specification
- [005-STYLE-SYSTEM-001.md](../../features/005-STYLE-SYSTEM-001.md): Style system feature
- [ratatui-core-src-style-color-ANALYSIS.md](../../file-analyses/ratatui-core-src-style-color-ANALYSIS.md): Source file analysis