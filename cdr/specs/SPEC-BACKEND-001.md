---
id: SPEC-BACKEND-001
title: Terminal Backend Specification
status: draft
date: 2023-11-28
---

# Terminal Backend Specification

## Overview

The Terminal Backend is a core abstraction in CycoTui that provides a platform-independent interface for interacting with terminal environments. It handles raw terminal operations, such as cursor movement, color output, alternate screen management, and input events.

Based on analysis of Ratatui's architecture, the backend system uses a plugin-like approach with a common interface that can be implemented for different terminal libraries or direct platform APIs.

## Scope

This specification covers:

1. The `ITerminalBackend` interface and its requirements
2. Platform-specific backend implementations
3. Terminal capability detection and handling
4. Input and event handling
5. Terminal state management (raw mode, alternate screen)
6. Integration with the buffer rendering system
7. Viewport area management and coordinate translation

## Requirements

### Frame-Based Rendering Model

Based on analysis of `ratatui-core/src/terminal/frame.rs`, CycoTui must implement a frame-based rendering system that provides:

```csharp
public class Frame : IDisposable
{
    // Properties
    public Rect Area { get; }  // The area of the current frame
    public ulong Count { get; }  // Frame sequence number
    
    // Widget rendering methods
    public void RenderWidget<T>(T widget, Rect area) where T : IWidget;
    public void RenderStatefulWidget<T, TState>(T widget, Rect area, ref TState state) 
        where T : IStatefulWidget<TState>;
    
    // Cursor management
    public void SetCursorPosition(Position position);
    
    // Buffer access
    public Buffer Buffer { get; }
}

public class CompletedFrame
{
    public Buffer Buffer { get; }
    public Rect Area { get; }
    public ulong Count { get; }
}
```

**Frame System Requirements:**

1. **Isolated Rendering Context**: Each frame provides isolated access to rendering buffer
2. **Guaranteed Stability**: Frame area guaranteed not to change during rendering
3. **Widget Rendering Interface**: Unified interface for both stateless and stateful widgets
4. **Cursor Control**: Frame-level cursor positioning that applies after rendering
5. **Buffer Diffing**: Changes applied only after comparing with previous frame
6. **Frame Sequencing**: Monotonic frame counter for animation and debugging
7. **Lifetime Management**: Frame cannot outlive the terminal that created it

**Frame Lifecycle:**

1. Terminal creates Frame with current buffer and viewport
2. Application renders widgets to Frame
3. Application optionally sets cursor position
4. Frame is disposed, triggering buffer diff and terminal update
5. CompletedFrame returned with final state for inspection

### Interface Definition

Based on analysis of `ratatui-core/src/backend.rs`, the `ITerminalBackend` interface must provide:

```csharp
public interface ITerminalBackend : IDisposable
{
    // Error type for backend operations
    // In C#, we'll use exceptions instead of associated error types
    
    // Drawing operations
    void Draw(IEnumerable<(int x, int y, Cell cell)> content);
    void Flush();
    
    // Cursor operations  
    void HideCursor();
    void ShowCursor();
    Position GetCursorPosition();
    void SetCursorPosition(Position position);
    
    // Screen operations
    void Clear();
    void ClearRegion(ClearType clearType);
    void AppendLines(int count = 1);  // Optional operation
    
    // Size operations
    Size GetSize();
    WindowSize GetWindowSize();
    
    // Advanced operations (feature-gated in Rust version)
    void ScrollRegionUp(Range region, int lineCount);
    void ScrollRegionDown(Range region, int lineCount);
}
```

**Key Interface Notes from Ratatui Analysis:**
- Drawing uses iterator pattern for efficient batch rendering
- Clear operations support multiple modes via ClearType enum
- Cursor position uses coordinate system with (0,0) at top-left
- Size operations provide both character and pixel dimensions
- Error handling uses associated types in Rust (exceptions in C#)
- Some operations are optional and may not be implemented by all backends

**Supporting Types:**

```csharp
public enum ClearType
{
    All,           // Clear entire screen
    AfterCursor,   // Clear everything after cursor
    BeforeCursor,  // Clear everything before cursor
    CurrentLine,   // Clear current line
    UntilNewLine   // Clear from cursor until next newline
}

public struct WindowSize
{
    public Size ColumnsRows { get; set; }  // Character dimensions
    public Size Pixels { get; set; }       // Pixel dimensions (may be 0,0)
}
```

### Terminal Initialization and Restoration

Based on analysis of the initialization patterns in Ratatui, CycoTui must provide:

```csharp
public static class Terminal
{
    // Initialize terminal with default settings (throws exceptions on error)
    public static ITerminal Init()
    
    // Try to initialize terminal, returning errors instead of throwing
    public static Result<ITerminal> TryInit()
    
    // Initialize with custom options
    public static ITerminal Init(TerminalOptions options)
    
    // Try to initialize with custom options
    public static Result<ITerminal> TryInit(TerminalOptions options)
    
    // Run a function within a terminal session, ensuring cleanup
    public static void Run(Action<ITerminal> action)
    
    // Run a function returning a result, ensuring cleanup
    public static TResult Run<TResult>(Func<ITerminal, TResult> func)
    
    // Restore terminal state
    public static void Restore(ITerminal terminal)
    
    // Try to restore terminal state
    public static Result<Unit> TryRestore(ITerminal terminal)
}
```

The initialization functions should:
1. Enable raw mode
2. Enter alternate screen (if requested)
3. Hide cursor (if requested)
4. Set up exception handlers for proper cleanup
5. Return a configured terminal instance

The restoration functions should:
1. Show cursor (if previously hidden)
2. Exit alternate screen (if previously entered)
3. Disable raw mode
4. Handle errors appropriately based on function variant

### Backend Selection System

Based on analysis of `xtask/src/commands/backend.rs`, CycoTui must provide a mechanism for selecting and testing different backend implementations:

**Backend Enumeration**:
```csharp
public enum TerminalBackendType
{
    Crossterm,  // Cross-platform backend using P/Invoke
    Windows,    // Native Windows Console API
    Unix,       // Unix/Linux termios + ANSI sequences
    Test        // In-memory backend for testing
}
```

**Backend Factory Pattern**:
```csharp
public static class TerminalBackendFactory
{
    public static ITerminalBackend Create(TerminalBackendType type = TerminalBackendType.Crossterm)
    {
        return type switch
        {
            TerminalBackendType.Crossterm => new CrosstermBackend(),
            TerminalBackendType.Windows when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                => new WindowsConsoleBackend(),
            TerminalBackendType.Unix when !RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                => new UnixTerminalBackend(),
            TerminalBackendType.Test => new TestBackend(),
            _ => throw new PlatformNotSupportedException($"Backend {type} not supported on this platform")
        };
    }
    
    // Platform validation similar to Ratatui's approach
    public static void ValidateBackend(TerminalBackendType type)
    {
        if (type == TerminalBackendType.Unix && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException("Unix backend is not supported on Windows");
        }
        // Additional platform validations...
    }
}
```

**Build System Requirements**:
1. **Conditional Compilation**: Use preprocessor directives or project references to include/exclude backends
2. **Platform-Specific Packages**: Separate NuGet packages for different backend implementations
3. **Feature Detection**: Runtime detection of terminal capabilities per backend
4. **Testing Strategy**: Support for testing multiple backends in CI/CD pipeline
5. **Backend Isolation**: Each backend should be testable independently

**Configuration-Based Selection**:
```csharp
public class TerminalConfiguration
{
    public TerminalBackendType PreferredBackend { get; set; } = TerminalBackendType.Crossterm;
    public bool EnableFallback { get; set; } = true;
    public Dictionary<TerminalBackendType, bool> BackendAvailability { get; } = new();
    
    public ITerminalBackend CreateBackend()
    {
        if (BackendAvailability.TryGetValue(PreferredBackend, out var available) && available)
        {
            return TerminalBackendFactory.Create(PreferredBackend);
        }
        
        if (EnableFallback)
        {
            // Try fallback backends...
            return TerminalBackendFactory.Create(TerminalBackendType.Crossterm);
        }
        
        throw new InvalidOperationException($"Backend {PreferredBackend} not available");
    }
}
```

### Terminal Capabilities

The backend must detect and handle:

1. **Color support levels**: None, ANSI 16-color, 256-color, RGB/truecolor
2. **Mouse support**: Detecting and enabling mouse event reporting
3. **Unicode support**: Handling wide and combining characters
4. **Terminal size**: Getting and responding to size changes

### Event Handling

Input event types to support:

1. **Key events**: Including modifiers (Ctrl, Alt, Shift)
2. **Mouse events**: Press, release, drag, scroll
3. **Resize events**: Terminal size changes
4. **Focus events**: Terminal focus gain/loss (when supported)

### Initialization and Cleanup

Based on analysis of Ratatui's init.rs, the backend must support:

1. **Terminal Initialization**: 
   - Easy setup of terminal with sane defaults
   - Support for customization via options
   - Error handling for initialization failures

2. **Terminal Cleanup**:
   - Reliable restoration of terminal state
   - Proper handling of exceptions during cleanup
   - Automatic cleanup via IDisposable pattern

3. **Initialization API**:
   ```csharp
   // Recommended approach using IDisposable
   public static Terminal Init()
   public static Terminal InitWithOptions(TerminalOptions options)
   public static void Run(Action<Terminal> action)
   
   // Terminal implements IDisposable for cleanup
   public void Dispose()
   
   // Manual cleanup if needed
   public void Restore()
   ```

4. **Terminal Options**:
   - Enable/disable alternate screen
   - Enable/disable raw mode
   - Enable/disable mouse capture
   - Custom viewport settings

### Error Handling

Error handling requirements:

1. Provide meaningful exceptions for terminal operation failures
2. Ensure proper cleanup even when exceptions occur
3. Handle unexpected terminal states gracefully
4. Support diagnostic modes for troubleshooting

## Technical Approach

### Windows Implementation

For Windows, the implementation will:

1. Use P/Invoke to call Windows Console API functions
2. Enable Virtual Terminal Processing for ANSI support when available
3. Fall back to direct Console API calls when necessary
4. Handle UTF-8 encoding and wide character support
5. Implement mouse support via ReadConsoleInput

Key Windows APIs to use:
- `SetConsoleMode` for enabling VT processing and raw input
- `GetConsoleScreenBufferInfo` for terminal size
- `SetConsoleCursorPosition` for cursor control
- `ReadConsoleInput` for input events

### Unix Implementation

For Unix-based systems (Linux, macOS), the implementation will:

1. Use P/Invoke to termios functions for raw mode
2. Use ANSI escape sequences for terminal control
3. Parse terminal input for key and mouse events
4. Support standard ANSI/VT100 control sequences

Key Unix APIs to use:
- `tcgetattr`/`tcsetattr` for terminal modes
- `ioctl` with `TIOCGWINSZ` for terminal size
- Standard input/output file descriptors for I/O

## Type Conversion System

Based on analysis of `ratatui-crossterm/src/lib.rs`, CycoTui must implement a robust type conversion system between CycoTui types and underlying platform library types:

```csharp
// Conversion interfaces for type system interoperability
public interface IConvertToPlatform<out TPlatform>
{
    TPlatform ToPlatform();
}

public interface IConvertFromPlatform<in TPlatform>
{
    static abstract Self FromPlatform(TPlatform value);
}

// Color conversion implementations
public partial struct Color : IConvertToPlatform<PlatformColor>, IConvertFromPlatform<PlatformColor>
{
    public PlatformColor ToPlatform() => this switch
    {
        { Type: ColorType.Reset } => PlatformColor.Reset,
        { Type: ColorType.Black } => PlatformColor.Black,
        { Type: ColorType.Red } => PlatformColor.DarkRed,
        // ... additional mappings
        { Type: ColorType.Rgb, R: var r, G: var g, B: var b } => new PlatformColor.Rgb(r, g, b),
        { Type: ColorType.Indexed, Index: var i } => new PlatformColor.Indexed(i),
    };
    
    public static Color FromPlatform(PlatformColor value) => value switch
    {
        PlatformColor.Reset => Color.Reset,
        PlatformColor.Black => Color.Black,
        PlatformColor.DarkRed => Color.Red,
        // ... additional mappings
    };
}
```

**Key Type Conversion Requirements:**
1. **Bidirectional Conversion**: Support conversion both to and from platform types
2. **Color Mapping**: Handle differences in color naming between systems (Dark vs Light variants)
3. **Style Conversion**: Map text modifiers and attributes correctly
4. **Feature Compatibility**: Handle cases where platform doesn't support all CycoTui features
5. **Performance**: Use pattern matching and avoid allocations in conversion

### Style Differential System

Implement Ratatui's efficient style update system:

```csharp
public struct StyleDiff
{
    public Modifier Added { get; }
    public Modifier Removed { get; }
    
    public StyleDiff(Modifier from, Modifier to)
    {
        Added = to & ~from;      // Modifiers in 'to' but not in 'from'
        Removed = from & ~to;    // Modifiers in 'from' but not in 'to'
    }
    
    public void ApplyTo<TWriter>(TWriter writer) where TWriter : ITerminalWriter
    {
        // Apply removed modifiers first
        if (Removed.HasFlag(Modifier.Bold) || Removed.HasFlag(Modifier.Dim))
        {
            writer.WriteAttribute(Attribute.NormalIntensity);
            // Re-apply remaining intensity modifiers
            if (Added.HasFlag(Modifier.Dim)) writer.WriteAttribute(Attribute.Dim);
            if (Added.HasFlag(Modifier.Bold)) writer.WriteAttribute(Attribute.Bold);
        }
        
        if (Removed.HasFlag(Modifier.Italic))
            writer.WriteAttribute(Attribute.NoItalic);
        
        // Apply added modifiers
        if (Added.HasFlag(Modifier.Bold))
            writer.WriteAttribute(Attribute.Bold);
        if (Added.HasFlag(Modifier.Italic))
            writer.WriteAttribute(Attribute.Italic);
        // ... additional modifier handling
    }
}
```

### Platform Library Version Management

Handle multiple versions of underlying terminal libraries:

```csharp
public enum PlatformLibraryVersion
{
    Latest,     // Use latest available version
    Specific    // Use specific version
}

public class BackendConfiguration
{
    public PlatformLibraryVersion Version { get; set; } = PlatformLibraryVersion.Latest;
    public bool EnableUnderlineColor { get; set; } = true;
    public bool EnableScrollingRegions { get; set; } = true;
    public bool EnableMouse { get; set; } = true;
    
    // Feature compatibility matrix
    public Dictionary<string, bool> Features { get; } = new();
}

public static class BackendFactory
{
    public static ITerminalBackend CreateCrossterm(BackendConfiguration config = null)
    {
        config ??= new BackendConfiguration();
        
        // Version selection and feature detection logic
        var platformLibrary = SelectPlatformLibrary(config.Version);
        var capabilities = DetectCapabilities(platformLibrary);
        
        return new CrosstermBackend(Console.Out, capabilities, config);
    }
}
```

### Test Backend Implementation

The test backend provides a memory-based implementation for automated testing that offers:

```csharp
public class TestBackend : IBackend
{
    // Properties
    public Buffer Buffer { get; }
    public Buffer Scrollback { get; }
    
    // Constructor
    public TestBackend(ushort width, ushort height)
    public static TestBackend WithLines(IEnumerable<string> lines)
    
    // Testing utilities
    public void AssertBuffer(Buffer expected)
    public void AssertBufferLines(IEnumerable<string> expectedLines)
    public void AssertScrollback(Buffer expected)
    public void AssertScrollbackLines(IEnumerable<string> expectedLines)
    public void AssertScrollbackEmpty()
    public void AssertCursorPosition(Position expected)
    
    // Buffer management
    public void Resize(ushort width, ushort height)
    public string GetBufferView() // For debugging visualization
}
```

Key test backend features:
1. **In-memory buffers**: Both main screen and scrollback history
2. **Buffer assertions**: Rich assertion methods for testing
3. **Scrollback management**: Automatic management with size limits (up to ushort.MaxValue lines)
4. **Multi-width character support**: Proper handling of Unicode characters
5. **Regional clear operations**: Support for various clear types
6. **Buffer visualization**: String representation for debugging
7. **Infallible operations**: No error handling needed for test scenarios

The test backend must handle:
- Line scrolling and scrollback buffer management
- Cursor position tracking and validation
- Multi-width character rendering and display
- Various clear operations (All, AfterCursor, BeforeCursor, CurrentLine, UntilNewLine)
- Buffer resizing with content preservation
- Scrollback size limits and truncation

Error Handling for Test Backend:
- Use simple return types (no Result<T> wrapper needed)
- All operations should succeed in test environment
- Provide meaningful assertion failure messages

## Terms and Definitions

- **Raw Mode**: Terminal mode that provides unbuffered input without echo
- **Alternate Screen**: Secondary buffer that allows restoring the terminal to its previous state
- **Cell**: A single character position with associated styling
- **ANSI Escape Sequences**: Control codes that change terminal behavior or appearance
- **VT100/VT220**: Terminal standards that define control sequences
- **Capability Detection**: Process of determining what features a terminal supports

## Platform-Specific Implementation Details

### Termwiz-Style Backend Implementation

Based on analysis of `ratatui-termwiz/src/lib.rs`, one backend implementation pattern uses a buffered terminal approach:

#### Core Architecture
- Wraps an underlying terminal library (like termwiz) with a buffered interface
- Accumulates terminal changes before flushing for efficiency
- Provides automatic terminal state management (raw mode, alternate screen)

#### Key Implementation Patterns
```csharp
public class BufferedTerminalBackend : ITerminalBackend
{
    private readonly IBufferedTerminal _bufferedTerminal;
    
    // Initialization with automatic setup
    public static BufferedTerminalBackend Create()
    {
        var terminal = CreateSystemTerminal();
        terminal.EnableRawMode();
        terminal.EnterAlternateScreen();
        return new BufferedTerminalBackend(terminal);
    }
    
    // Buffered drawing approach
    public void Draw<T>(IEnumerable<T> content) where T : ICellContent
    {
        foreach (var cell in content)
        {
            // Accumulate changes in buffer
            _bufferedTerminal.AddChanges(ConvertCellToChanges(cell));
        }
    }
    
    public void Flush()
    {
        _bufferedTerminal.Flush(); // Send all accumulated changes
    }
}
```

#### Type Conversion System
Must handle conversion between CycoTui types and underlying terminal library types:

```csharp
// Conversion interfaces for type safety
public interface ITerminalTypeConverter<TSource, TTarget>
{
    TTarget Convert(TSource source);
}

// Color conversion example
public class ColorConverter : ITerminalTypeConverter<Color, TerminalColor>
{
    public TerminalColor Convert(Color color) => color switch
    {
        Color.Reset => TerminalColor.Default,
        Color.Rgb(var r, var g, var b) => TerminalColor.TrueColor(r, g, b),
        Color.Indexed(var i) => TerminalColor.Palette(i),
        // ... etc
    };
}
```

#### Feature Flag Equivalent System
Support optional features through interfaces or conditional compilation:

```csharp
// Interface-based approach
public interface IScrollingRegionSupport
{
    void ScrollRegionUp(Range region, int amount);
    void ScrollRegionDown(Range region, int amount);
}

// Or conditional compilation
#if FEATURE_SCROLLING_REGIONS
public void ScrollRegionUp(Range region, int amount) { /* implementation */ }
#endif
```

### Windows Special Considerations

1. **Console API vs. VT Processing**: Windows 10+ supports ANSI sequences with VT processing enabled
2. **Legacy Console vs. Windows Terminal**: Different capabilities and behavior
3. **Code Page Handling**: Ensure UTF-8 support with proper code page settings
4. **ConPTY**: Consider Windows Pseudo Console API for advanced scenarios

### Unix Special Considerations

1. **Terminal Type Differences**: Handle variations between xterm, rxvt, iTerm2, etc.
2. **Signal Handling**: Manage SIGWINCH for resize events
3. **tty vs. pipes**: Detect and handle non-interactive environments
4. **Terminal Database**: Consider terminfo capabilities for advanced features

### Termion Backend Implementation Details

Based on analysis of `ratatui-termion/src/lib.rs`:

**Color Optimization Patterns**:
- Use Display trait implementations for color formatting to ANSI sequences
- Map Ratatui Color enum directly to platform color types
- Handle color edge cases: Gray maps to White, DarkGray maps to LightBlack
- Support all color models: Basic 16, Indexed 256, RGB truecolor

**Drawing Optimization Strategy**:
```csharp
// Implement similar optimization pattern
public void Draw(IEnumerable<(int x, int y, Cell cell)> content)
{
    var output = new StringBuilder(capacity: content.Count() * 3);
    var currentFg = Color.Reset;
    var currentBg = Color.Reset;
    var currentModifier = Modifier.Empty;
    Position? lastPos = null;
    
    foreach (var (x, y, cell) in content)
    {
        // Only move cursor if position is non-contiguous
        if (lastPos == null || x != lastPos.Value.X + 1 || y != lastPos.Value.Y)
        {
            output.Append($"\x1B[{y + 1};{x + 1}H"); // ANSI cursor positioning
        }
        lastPos = new Position(x, y);
        
        // Only emit style changes when different
        if (cell.Modifier != currentModifier)
        {
            AppendModifierDiff(output, currentModifier, cell.Modifier);
            currentModifier = cell.Modifier;
        }
        
        if (cell.Fg != currentFg)
        {
            AppendForegroundColor(output, cell.Fg);
            currentFg = cell.Fg;
        }
        
        if (cell.Bg != currentBg)
        {
            AppendBackgroundColor(output, cell.Bg);
            currentBg = cell.Bg;
        }
        
        output.Append(cell.Symbol);
    }
    
    // Reset all styles after drawing
    output.Append("\x1B[39m\x1B[49m\x1B[0m"); // Reset fg, bg, all attributes
    
    writer.Write(output.ToString());
    writer.Flush();
}
```

**Modifier Difference Calculation**:
- Calculate removed and added modifiers separately
- Handle complex modifier interactions (Bold/Dim conflicts)
- Emit minimal ANSI sequences for style changes
- Consider terminal-specific modifier behaviors

**Scrolling Region Support** (feature-gated):
- Use ANSI sequences: `\x1B[{top};{bottom}r` to set region
- Use `\x1B[r` to reset region
- Implement using Range<int> types for region specification
- Flush after each scroll operation to ensure immediate effect

## See Also

- [VISION-TECH-002.md](../vision/VISION-TECH-002.md): Technical architecture
- [SPEC-BUFFER-002.md](SPEC-BUFFER-002.md): Buffer rendering system
- [001-BACKEND-ABSTRACTION-001.md](../features/004-BACKEND-ABSTRACTION-001.md): Backend abstraction feature
- [CORE-BACKEND-INTERFACE-001](../tasks/CORE-BACKEND-INTERFACE-001/README.md): Backend interface implementation task