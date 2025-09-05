# Masked Text Implementation

## Overview

Implement the Masked text type that wraps strings to display as masked characters for secure data display (passwords, API keys, etc.). The implementation must preserve the original content for debugging while displaying only masked characters for security.

## Implementation Approach

### Core Implementation
Create a `Masked` class that:
- Stores original string content securely
- Provides configurable masking character (default '*')
- Implements character-by-character replacement for masked display
- Supports seamless integration with the text system

### Key Design Decisions
- **Debug vs Display**: Debug formatting shows original string, Display formatting shows masked string
- **Performance**: Accept O(n) masking operation for simplicity and clarity
- **Unicode Support**: Handle any Unicode character as mask character
- **Memory**: Store original content without modification, generate masked content on demand

### C# Implementation Strategy
```csharp
public class Masked
{
    private readonly string _content;
    private readonly char _maskChar;
    
    public Masked(string content, char maskChar = '*')
    {
        _content = content ?? throw new ArgumentNullException(nameof(content));
        _maskChar = maskChar;
    }
    
    public char MaskChar => _maskChar;
    
    public string Value => new string(_maskChar, _content.Length);
    
    // Conversion operators
    public static implicit operator string(Masked masked) => masked.Value;
    public static implicit operator Text(Masked masked) => Text.Raw(masked.Value);
    
    // Formatting - Debug shows original, Display shows masked
    public override string ToString() => Value;
}
```

## Key Challenges

### Unicode Character Handling
- **Challenge**: Ensure masking works correctly with multi-byte Unicode characters
- **Approach**: Use string.Length for consistency with terminal column counting
- **Consideration**: Document behavior with combining characters and grapheme clusters

### Performance Optimization
- **Challenge**: Avoid recreating masked string on every access
- **Options**:
  1. Cache masked result after first computation
  2. Accept regeneration for simplicity
  3. Lazy evaluation pattern
- **Recommendation**: Start with regeneration for clarity, optimize if needed

### Security Considerations
- **Challenge**: Debug output reveals original content
- **Approach**: Document this behavior clearly as feature for development
- **Consideration**: Ensure no accidental logging of ToString() in production

## Related Components

### Text System Integration
- Must convert seamlessly to `Text` type using masked content
- Should work with existing text hierarchy (Span, Line, Text)
- Must maintain consistent behavior with other text types

### Style System Integration
- Masked text should support styling like regular text
- Style should apply to masked representation, not original content
- Consider styled masked text for UI consistency

## Integration Points

### Widget Usage
Widgets should accept Masked text transparently:
```csharp
var passwordField = new Paragraph()
    .SetContent(new Masked(userInput, '•'));
```

### Text Conversion
Support implicit conversion to standard text types:
```csharp
Text content = new Masked("secret", 'x');  // Should work seamlessly
```

## Testing Approach

### Unit Tests
- Test masking with various characters including Unicode
- Verify conversion operations work correctly
- Test edge cases (empty string, single character, long strings)
- Validate Debug vs Display formatting behavior

### Integration Tests
- Test with widget rendering system
- Verify text system integration works correctly
- Test with style system if applicable

### Security Tests
- Verify original content is not leaked in normal display
- Test Debug formatting in development scenarios
- Validate ToString() behavior

## Performance Considerations

### Memory Usage
- Minimal overhead: original string + mask character storage
- Masked string generated on demand
- Consider caching for frequently accessed masked content

### CPU Performance
- O(n) operation for mask generation where n is string length
- Acceptable for typical password field usage
- Monitor performance for large text masking scenarios

## Platform-Specific Details

No platform-specific considerations identified. Standard string operations should work consistently across Windows, macOS, and Linux.

## Acceptance Criteria

### Functional Requirements
- [ ] Can create Masked instance with string content and custom mask character
- [ ] Default mask character is '*' when not specified
- [ ] Value property returns masked representation
- [ ] MaskChar property returns the configured mask character
- [ ] Implicit conversion to string returns masked content
- [ ] Implicit conversion to Text creates Text with masked content
- [ ] ToString() returns masked representation
- [ ] Debug formatting shows original content (if applicable in C#)

### Quality Requirements
- [ ] Handles empty strings correctly
- [ ] Works with Unicode mask characters
- [ ] No memory leaks in repeated usage
- [ ] Performance acceptable for typical use cases (< 1ms for 100 character strings)

### Integration Requirements
- [ ] Integrates seamlessly with text system
- [ ] Works with widget content setting
- [ ] Supports styling when used with styled text types
- [ ] Consistent behavior with other text types

## See Also

- SPEC-TEXT-001: Text System Specification
- 006-TEXT-SYSTEM-001: Text System Feature
- TEXT-HIERARCHY-001: Core text type implementation
- SPEC-STYLE-005: Style System (for styled masked text)