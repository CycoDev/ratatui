# Test Backend Implementation

## Overview

Implement the TestBackend class that provides an in-memory, testable implementation of the IBackend interface. This backend enables automated testing of terminal applications without requiring a real terminal, and provides rich assertion capabilities for verifying terminal output.

Based on analysis of Ratatui's TestBackend implementation, this task involves creating a comprehensive testing infrastructure that handles buffer management, scrollback functionality, and debugging utilities.

## Implementation Approach

### Core Data Structure
Create TestBackend class with these components:
- Main screen buffer (Buffer) for current terminal content
- Scrollback buffer (Buffer) for historical content that scrolled off-screen
- Cursor position tracking (x, y coordinates)
- Cursor visibility state (boolean)

### Key Methods to Implement
1. **Constructors**:
   - `TestBackend(ushort width, ushort height)` - Create with specified dimensions
   - `TestBackend.WithLines(IEnumerable<string> lines)` - Create with initial content

2. **Buffer Access**:
   - `Buffer` property - Access current screen buffer
   - `Scrollback` property - Access scrollback history buffer

3. **Assertion Methods**:
   - `AssertBuffer(Buffer expected)` - Assert buffer equals expected content
   - `AssertBufferLines(IEnumerable<string> expectedLines)` - Assert buffer matches expected lines
   - `AssertScrollback(Buffer expected)` - Assert scrollback content
   - `AssertScrollbackLines(IEnumerable<string> expectedLines)` - Assert scrollback matches lines
   - `AssertScrollbackEmpty()` - Assert no scrollback content
   - `AssertCursorPosition(Position expected)` - Assert cursor position

4. **IBackend Implementation**:
   - All interface methods should modify in-memory buffers
   - Use infallible return types (no error handling needed)
   - Implement proper scrollback management in AppendLines

5. **Debugging Support**:
   - `GetBufferView()` - Return string representation of buffer for debugging
   - Handle multi-width character visualization
   - Show hidden/overwritten characters for debugging

## Key Challenges

### Scrollback Buffer Management
- Implement efficient scrollback with size limits (up to ushort.MaxValue lines)
- Handle buffer rotation when content exceeds maximum size
- Preserve content correctly when scrolling

### Multi-Width Character Handling
- Track characters that span multiple columns
- Handle overwritten cells in buffer visualization
- Ensure correct width calculations for assertion methods

### Regional Clear Operations
- Implement ClearType.All, AfterCursor, BeforeCursor, CurrentLine, UntilNewLine
- Calculate correct buffer regions based on cursor position
- Handle edge cases at buffer boundaries

### Performance Considerations
- Efficient buffer operations for large content
- Minimize memory allocations during buffer operations
- Optimize scrollback buffer management

## Related Components

### Dependencies
- `Buffer` class for content storage
- `Cell` class for individual character/style combinations
- `Position` and `Size` types for coordinates
- `ClearType` enum for clear operations
- `IBackend` interface definition

### Integration Points
- Must integrate with testing frameworks (xUnit, NUnit, MSTest)
- Should work with buffer assertion utilities
- Needs to support buffer visualization for debugging

## Testing Approach

### Unit Tests
Create comprehensive tests for:
- Basic buffer operations (draw, clear, resize)
- Cursor position tracking and assertion
- Scrollback buffer management and size limits
- Multi-width character handling
- Regional clear operations
- Buffer visualization output

### Integration Tests
Test with:
- Real widget rendering to verify buffer output
- Large content scenarios to test performance
- Edge cases like zero-size buffers, extreme scrolling
- Unicode content with various character widths

### Test Data
Create test fixtures for:
- Various buffer sizes and content patterns
- Multi-width character scenarios
- Complex scrolling scenarios
- Buffer assertion failure cases

## Acceptance Criteria

1. **Interface Compliance**: TestBackend implements all IBackend methods correctly
2. **Buffer Management**: Properly maintains screen and scrollback buffers with content integrity
3. **Assertion Capabilities**: Provides rich assertion methods that give clear failure messages
4. **Scrollback Functionality**: Correctly manages scrollback with size limits and content preservation
5. **Multi-Width Support**: Handles Unicode characters with proper width calculations
6. **Clear Operations**: Implements all ClearType variants correctly based on cursor position
7. **Performance**: Handles large buffers efficiently without memory leaks
8. **Debugging Support**: Provides useful buffer visualization for test debugging
9. **Error Messages**: Assertion failures provide clear, actionable error messages
10. **Documentation**: Class and methods are thoroughly documented with usage examples

## See Also

- `SPEC-BACKEND-001.md` - Backend interface specification
- `004-BACKEND-ABSTRACTION-001.md` - Backend abstraction feature
- `SPEC-BUFFER-002.md` - Buffer model specification
- File analysis: `ratatui-core-src-backend-test.md`