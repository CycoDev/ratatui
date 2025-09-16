# Terminal Control Abstraction Implementation

## Overview

This task implements the terminal control abstraction layer that provides cross-platform terminal management including raw mode control, ANSI escape sequences, cursor management, and screen buffer operations. This component serves as the foundation for the application framework's terminal integration.

## Implementation Approach

1. **Create Terminal Control Interface**:
   - Define `ITerminalController` interface for cross-platform terminal operations
   - Implement ANSI escape sequence generation for cursor, colors, and screen control
   - Create platform-specific implementations for Windows Console API and Unix termios
   - Support raw mode enable/disable, alternate screen buffer, and mouse capture

2. **Implement ANSI Sequence Management**:
   - Create `AnsiSequences` static class with all standard terminal control codes
   - Implement cursor positioning, movement, and visibility control
   - Add screen clearing operations (full screen, line, regions)
   - Support color output (16-color, 256-color, RGB/truecolor)
   - Handle text styling (bold, italic, underline, strikethrough)

3. **Platform-Specific Implementations**:
   - **Windows**: Use P/Invoke to Windows Console API with VT processing fallback
   - **Unix/Linux**: Use termios system calls for raw mode and ANSI sequences
   - **Cross-platform**: Provide Crossterm-style unified interface
   - Handle platform detection and capability negotiation

4. **Terminal State Management**:
   - Track current terminal state (raw mode, alternate screen, cursor visibility)
   - Implement automatic state restoration on application exit
   - Handle unexpected termination scenarios with proper cleanup
   - Support nested terminal applications with state stack

## Key Challenges

1. **Cross-Platform Compatibility**: Ensuring consistent behavior across Windows, macOS, and Linux
2. **Terminal Capability Detection**: Determining what features are supported by the current terminal
3. **State Management**: Reliable restoration of terminal state even during unexpected exits
4. **Performance**: Minimizing system calls and optimizing ANSI sequence generation
5. **Error Handling**: Graceful degradation when terminal features are not available

## Implementation Notes

### Terminal Controller Interface

```csharp
public interface ITerminalController : IDisposable
{
    // Terminal mode management
    Task EnableRawModeAsync();
    Task DisableRawModeAsync();
    Task EnterAlternateScreenAsync();
    Task ExitAlternateScreenAsync();

    // Cursor operations
    Task HideCursorAsync();
    Task ShowCursorAsync();
    Task SetCursorPositionAsync(Position position);
    Task<Position> GetCursorPositionAsync();

    // Screen operations
    Task ClearScreenAsync();
    Task ClearLineAsync();
    Task ClearRegionAsync(Rect region);

    // Size and capabilities
    Task<Size> GetSizeAsync();
    bool SupportsColors { get; }
    bool SupportsMouse { get; }
    bool SupportsAlternateScreen { get; }

    // Mouse and input
    Task EnableMouseCaptureAsync();
    Task DisableMouseCaptureAsync();

    // Output
    Task WriteAsync(string content);
    Task FlushAsync();
}
```

### ANSI Sequence Constants

```csharp
public static class AnsiSequences
{
    // Cursor control
    public const string HideCursor = "\x1B[?25l";
    public const string ShowCursor = "\x1B[?25h";
    public static string SetCursorPosition(int x, int y) => $"\x1B[{y + 1};{x + 1}H";

    // Screen control
    public const string ClearScreen = "\x1B[2J";
    public const string ClearLine = "\x1B[2K";
    public const string EnableAlternateScreen = "\x1B[?1049h";
    public const string DisableAlternateScreen = "\x1B[?1049l";

    // Mouse control
    public const string EnableMouseTracking = "\x1B[?1000h\x1B[?1002h\x1B[?1015h\x1B[?1006h";
    public const string DisableMouseTracking = "\x1B[?1006l\x1B[?1015l\x1B[?1002l\x1B[?1000l";

    // Color support
    public static string SetForegroundColor(Color color) => color switch
    {
        Color.Reset => "\x1B[39m",
        Color.Rgb(var r, var g, var b) => $"\x1B[38;2;{r};{g};{b}m",
        Color.Indexed(var i) => $"\x1B[38;5;{i}m",
        _ => GetBasicColorCode(color, false)
    };
}
```

### Platform Detection

```csharp
public static class TerminalPlatform
{
    public static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    public static bool IsUnix => !IsWindows;

    public static ITerminalController CreateController()
    {
        if (IsWindows)
        {
            // Try Windows Terminal/ConPTY first, fallback to legacy Console API
            return WindowsTerminalController.TryCreateModern() ?? new WindowsConsoleController();
        }
        else
        {
            // Unix implementation using termios
            return new UnixTerminalController();
        }
    }
}
```

## Testing Approach

1. **Unit Tests**: Test ANSI sequence generation and state management
2. **Integration Tests**: Test actual terminal operations with test backend
3. **Cross-Platform Tests**: Verify behavior on Windows, macOS, and Linux
4. **Capability Tests**: Test graceful degradation when features are unavailable
5. **Performance Tests**: Measure overhead of terminal operations
6. **Cleanup Tests**: Verify proper state restoration under various exit scenarios

## Related Components

- `src/CycoTui.Core/Terminal/ITerminalController.cs` - Main interface
- `src/CycoTui.Core/Terminal/AnsiSequences.cs` - ANSI escape codes
- `src/CycoTui.Core/Terminal/WindowsTerminalController.cs` - Windows implementation
- `src/CycoTui.Core/Terminal/UnixTerminalController.cs` - Unix implementation
- `src/CycoTui.Core/Terminal/TerminalCapabilities.cs` - Feature detection
- `src/CycoTui.Core/Terminal/TerminalState.cs` - State management

## Integration Points

- **Application Framework**: Main Application class uses this for terminal lifecycle
- **Backend System**: Terminal backends may use this for low-level control
- **Input System**: Mouse and resize events depend on terminal configuration
- **Rendering System**: Screen clearing and cursor management during frame updates

## Performance Considerations

- Cache ANSI sequences to avoid repeated string construction
- Batch terminal operations to reduce system call overhead
- Use StringBuilder for complex sequence construction
- Minimize async/await overhead for high-frequency operations
- Consider unsafe code for performance-critical ANSI generation

## Platform-Specific Details

### Windows Implementation
- Use `SetConsoleMode` to enable VT processing when available
- Fall back to Console API calls for legacy terminals
- Handle UTF-8/UTF-16 conversion properly
- Support both Windows Terminal and legacy Command Prompt

### Unix Implementation
- Use `tcgetattr`/`tcsetattr` for raw mode control
- Handle signal-based resize notifications (SIGWINCH)
- Support various terminal emulators (xterm, iTerm2, GNOME Terminal)
- Respect TERM environment variable for capability detection

## Acceptance Criteria

- [ ] `ITerminalController` interface implemented with cross-platform support
- [ ] ANSI sequence generation working for all common terminal operations
- [ ] Raw mode enable/disable functions correctly on all platforms
- [ ] Alternate screen buffer support with proper restoration
- [ ] Cursor visibility and positioning control working
- [ ] Screen clearing operations (full, line, region) implemented
- [ ] Mouse capture enable/disable functionality
- [ ] Terminal size detection working reliably
- [ ] Capability detection for colors, mouse, and alternate screen
- [ ] Proper error handling and graceful degradation
- [ ] State restoration working even during unexpected exits
- [ ] Cross-platform tests passing on Windows, macOS, and Linux
- [ ] Performance benchmarks meeting target thresholds
- [ ] Integration with existing backend system working

## See Also

- [SPEC-APPLAYER-001.md](../../specs/SPEC-APPLAYER-001.md): Application framework specification
- [SPEC-BACKEND-001.md](../../specs/SPEC-BACKEND-001.md): Backend interface specification
- [SPEC-TERMINAL-007.md](../../specs/SPEC-TERMINAL-007.md): Terminal implementation specification
- [APPLAYER-M6-001.md](../../roadmap/APPLAYER-M6-001.md): Application layer roadmap