# Color String Parsing Implementation

## Overview

Implement comprehensive string parsing capabilities for the Color type that matches Ratatui's parsing behavior, including ANSI color names, aliases, hex colors, and indexed colors with proper normalization and error handling.

## Implementation Approach

### Parsing Strategy
Implement a multi-stage parsing approach:
1. **Input Normalization**: Convert to lowercase, remove separators, handle aliases
2. **Format Detection**: Determine if input is color name, hex, or numeric
3. **Specific Parsing**: Apply format-specific parsing logic
4. **Validation**: Ensure parsed values are valid
5. **Color Construction**: Create appropriate Color instance

### Normalization Rules
Based on Ratatui's implementation, apply these transformations:
```csharp
private static string NormalizeColorString(string input)
{
    return input
        .ToLowerInvariant()
        .Replace(" ", "")
        .Replace("-", "")
        .Replace("_", "")
        .Replace("bright", "light")
        .Replace("grey", "gray")
        .Replace("silver", "gray")
        .Replace("lightblack", "darkgray")
        .Replace("lightwhite", "white")
        .Replace("lightgray", "white");
}
```

### Color Name Mapping
Create comprehensive color name mapping:
```csharp
private static readonly Dictionary<string, Color> ColorNames = new()
{
    ["reset"] = Color.Reset,
    ["black"] = Color.Black,
    ["red"] = Color.Red,
    ["green"] = Color.Green,
    ["yellow"] = Color.Yellow,
    ["blue"] = Color.Blue,
    ["magenta"] = Color.Magenta,
    ["cyan"] = Color.Cyan,
    ["gray"] = Color.Gray,
    ["darkgray"] = Color.DarkGray,
    ["lightred"] = Color.LightRed,
    ["lightgreen"] = Color.LightGreen,
    ["lightyellow"] = Color.LightYellow,
    ["lightblue"] = Color.LightBlue,
    ["lightmagenta"] = Color.LightMagenta,
    ["lightcyan"] = Color.LightCyan,
    ["white"] = Color.White
};
```

## Key Challenges

### Performance Optimization
String parsing can be called frequently during runtime:
- Use ReadOnlySpan<char> where possible to avoid allocations
- Consider caching frequently parsed colors
- Optimize normalization to minimize string operations
- Use efficient lookup structures for color names

### Comprehensive Alias Support
Must handle all variations found in Ratatui:
- Case variations: "Red", "RED", "red"
- Separator variations: "light red", "light-red", "light_red"
- Prefix variations: "bright red", "light red"
- Spelling variations: "grey", "gray"
- Historical aliases: "silver" → "gray"

### Hex Color Parsing
Implement robust hex color parsing:
- Validate '#' prefix
- Ensure exactly 6 hex characters
- Handle case insensitivity
- Proper error handling for malformed input
- Efficient conversion to RGB bytes

### Error Handling Strategy
Provide clear error information:
- Distinguish between format errors and value errors
- Provide helpful error messages
- Support both exception-throwing and TryParse patterns
- Consider providing suggestions for near-matches

## Related Components

### Color Enum Integration
- Must work seamlessly with Color struct
- Support all Color variants (ANSI, RGB, Indexed)
- Maintain consistency with other Color factory methods

### Performance Requirements
- Parsing should be efficient enough for runtime use
- Consider caching strategies for repeated parses
- Minimize memory allocations during parsing
- Profile against common usage patterns

### Validation Requirements
- Validate all input formats properly
- Provide meaningful error messages
- Handle edge cases gracefully
- Support roundtrip parsing (parse → toString → parse)

## Testing Approach

### Comprehensive Test Coverage
Test all supported formats and variations:
```csharp
[TestCase("red", ExpectedResult = Color.Red)]
[TestCase("Red", ExpectedResult = Color.Red)]
[TestCase("RED", ExpectedResult = Color.Red)]
[TestCase("light red", ExpectedResult = Color.LightRed)]
[TestCase("light-red", ExpectedResult = Color.LightRed)]
[TestCase("light_red", ExpectedResult = Color.LightRed)]
[TestCase("lightred", ExpectedResult = Color.LightRed)]
[TestCase("bright red", ExpectedResult = Color.LightRed)]
[TestCase("grey", ExpectedResult = Color.Gray)]
[TestCase("gray", ExpectedResult = Color.Gray)]
[TestCase("silver", ExpectedResult = Color.Gray)]
[TestCase("#FF0000", ExpectedResult = Color.Rgb(255, 0, 0))]
[TestCase("#ff0000", ExpectedResult = Color.Rgb(255, 0, 0))]
[TestCase("255", ExpectedResult = Color.Indexed(255))]
[TestCase("0", ExpectedResult = Color.Indexed(0))]
public Color TestColorParsing(string input) => Color.Parse(input);
```

### Error Case Testing
Test invalid inputs and error conditions:
```csharp
[TestCase("invalid")]
[TestCase("#FF00")]      // Too short
[TestCase("#FF0000000")] // Too long
[TestCase("")]           // Empty
[TestCase(null)]         // Null
[TestCase("256")]        // Out of range for indexed
[TestCase("#GGGGGG")]    // Invalid hex
public void TestParsingErrors(string input)
{
    Assert.Throws<ColorParseException>(() => Color.Parse(input));
    Assert.False(Color.TryParse(input, out _));
}
```

### Performance Testing
Benchmark parsing performance:
- Common color names (should be very fast)
- Hex colors (should be reasonably fast)
- Complex alias resolution
- Memory allocation patterns

### Roundtrip Testing
Ensure parsing and formatting are consistent:
```csharp
[Test]
public void TestParsingRoundtrip()
{
    var colors = new[] 
    {
        Color.Red, Color.Rgb(128, 64, 192), Color.Indexed(42)
    };
    
    foreach (var color in colors)
    {
        var str = color.ToString();
        var parsed = Color.Parse(str);
        Assert.AreEqual(color, parsed);
    }
}
```

## Implementation Notes

### String Normalization Performance
Consider optimizing the normalization chain:
```csharp
// Option 1: StringBuilder for multiple replacements
// Option 2: Custom state machine for single pass
// Option 3: Regex-based normalization
// Profile to determine best approach
```

### Hex Parsing Implementation
Efficient hex parsing without allocations:
```csharp
private static bool TryParseHexColor(ReadOnlySpan<char> input, out Color color)
{
    if (input.Length != 7 || input[0] != '#')
    {
        color = default;
        return false;
    }
    
    if (TryParseHexByte(input.Slice(1, 2), out byte r) &&
        TryParseHexByte(input.Slice(3, 2), out byte g) &&
        TryParseHexByte(input.Slice(5, 2), out byte b))
    {
        color = Color.Rgb(r, g, b);
        return true;
    }
    
    color = default;
    return false;
}
```

## Acceptance Criteria

1. **Format Support**:
   - All ANSI color names and aliases supported
   - Hex color parsing (#RRGGBB format)
   - Indexed color parsing (0-255)
   - Case-insensitive parsing
   - Multiple separator styles supported

2. **API Design**:
   - `Color.Parse(string)` method with exceptions
   - `Color.TryParse(string, out Color)` method without exceptions
   - Clear error messages for invalid inputs
   - Consistent with .NET parsing patterns

3. **Performance**:
   - Efficient parsing suitable for runtime use
   - Minimal memory allocations
   - Reasonable performance for all input types
   - Benchmarks showing acceptable performance

4. **Testing**:
   - Comprehensive test coverage for all formats
   - Error case testing
   - Performance benchmarks
   - Roundtrip consistency testing

5. **Documentation**:
   - Complete method documentation
   - Examples of supported formats
   - Performance characteristics
   - Error handling guidance

## See Also

- [CORE-COLOR-ENUM-001](../CORE-COLOR-ENUM-001/README.md): Color enum implementation
- [SPEC-STYLE-005.md](../../specs/SPEC-STYLE-005.md): Style system specification
- [ratatui-core-src-style-color-ANALYSIS.md](../../file-analyses/ratatui-core-src-style-color-ANALYSIS.md): Source file analysis