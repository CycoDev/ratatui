# Unix Terminal Backend Implementation (Termion-style)

## Overview

Implement a Unix-focused terminal backend for CycoTui that provides optimized terminal operations using P/Invoke to termios and ANSI escape sequences. This implementation is based on analysis of `ratatui-termion/src/lib.rs` and provides efficient terminal control for Linux and macOS platforms.

## Implementation Approach

### Core Backend Structure

Create a `UnixTerminalBackend` class that wraps a `TextWriter` and implements `ITerminalBackend`:

```csharp
public class UnixTerminalBackend : ITerminalBackend
{
    private readonly TextWriter writer;
    private Color currentFg = Color.Reset;
    private Color currentBg = Color.Reset;
    private Modifier currentModifier = Modifier.Empty;
    private Position? lastPosition = null;

    public UnixTerminalBackend(TextWriter writer)
    {
        this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }
}
```

### Optimized Drawing Implementation

Based on the Termion optimization patterns, implement efficient batch drawing:

```csharp
public void Draw(IEnumerable<(int x, int y, Cell cell)> content)
{
    var output = new StringBuilder(capacity: EstimateOutputSize(content));
    
    foreach (var (x, y, cell) in content)
    {
        // Only move cursor if position is non-contiguous
        if (lastPosition == null || x != lastPosition.Value.X + 1 || y != lastPosition.Value.Y)
        {
            output.Append($"\x1B[{y + 1};{x + 1}H");
        }
        lastPosition = new Position(x, y);
        
        // Apply style changes using difference calculation
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
    
    // Reset styles after drawing
    output.Append("\x1B[39m\x1B[49m\x1B[0m");
    writer.Write(output.ToString());
    writer.Flush();
}
```

### Color Translation System

Implement color-to-ANSI conversion following Termion patterns:

```csharp
private static void AppendForegroundColor(StringBuilder output, Color color)
{
    output.Append(color switch
    {
        { Type: ColorType.Reset } => "\x1B[39m",
        { Type: ColorType.Black } => "\x1B[30m",
        { Type: ColorType.Red } => "\x1B[31m",
        { Type: ColorType.Green } => "\x1B[32m",
        { Type: ColorType.Yellow } => "\x1B[33m",
        { Type: ColorType.Blue } => "\x1B[34m",
        { Type: ColorType.Magenta } => "\x1B[35m",
        { Type: ColorType.Cyan } => "\x1B[36m",
        { Type: ColorType.Gray } => "\x1B[37m",
        { Type: ColorType.DarkGray } => "\x1B[90m",
        { Type: ColorType.LightRed } => "\x1B[91m",
        { Type: ColorType.LightGreen } => "\x1B[92m",
        { Type: ColorType.LightYellow } => "\x1B[93m",
        { Type: ColorType.LightBlue } => "\x1B[94m",
        { Type: ColorType.LightMagenta } => "\x1B[95m",
        { Type: ColorType.LightCyan } => "\x1B[96m",
        { Type: ColorType.White } => "\x1B[97m",
        { Type: ColorType.Indexed, Index: var i } => $"\x1B[38;5;{i}m",
        { Type: ColorType.Rgb, R: var r, G: var g, B: var b } => $"\x1B[38;2;{r};{g};{b}m",
    });
}

private static void AppendBackgroundColor(StringBuilder output, Color color)
{
    // Similar to foreground but with background codes (40-47, 100-107, 48;5;n, 48;2;r;g;b)
}
```

### Modifier Difference Calculation

Implement efficient style modifier updates based on Termion's ModifierDiff approach:

```csharp
private static void AppendModifierDiff(StringBuilder output, Modifier from, Modifier to)
{
    var removed = from & ~to;
    var added = to & ~from;
    
    // Handle removed modifiers first
    if (removed.HasFlag(Modifier.Reversed))
        output.Append("\x1B[27m");
    
    if (removed.HasFlag(Modifier.Bold) || removed.HasFlag(Modifier.Dim))
    {
        output.Append("\x1B[22m"); // Reset intensity
        if (to.HasFlag(Modifier.Dim))
            output.Append("\x1B[2m");
    }
    
    if (removed.HasFlag(Modifier.Italic))
        output.Append("\x1B[23m");
        
    if (removed.HasFlag(Modifier.Underlined))
        output.Append("\x1B[24m");
        
    if (removed.HasFlag(Modifier.CrossedOut))
        output.Append("\x1B[29m");
        
    if (removed.HasFlag(Modifier.SlowBlink) || removed.HasFlag(Modifier.RapidBlink))
        output.Append("\x1B[25m");
    
    // Handle added modifiers
    if (added.HasFlag(Modifier.Reversed))
        output.Append("\x1B[7m");
        
    if (added.HasFlag(Modifier.Bold))
        output.Append("\x1B[1m");
        
    if (added.HasFlag(Modifier.Italic))
        output.Append("\x1B[3m");
        
    if (added.HasFlag(Modifier.Underlined))
        output.Append("\x1B[4m");
        
    if (added.HasFlag(Modifier.Dim))
        output.Append("\x1B[2m");
        
    if (added.HasFlag(Modifier.CrossedOut))
        output.Append("\x1B[9m");
        
    if (added.HasFlag(Modifier.SlowBlink) || added.HasFlag(Modifier.RapidBlink))
        output.Append("\x1B[5m");
}
```

### Terminal Size and Operations

Implement terminal size detection and basic operations:

```csharp
public Size GetSize()
{
    // Use P/Invoke to ioctl with TIOCGWINSZ
    return GetTerminalSize();
}

public WindowSize GetWindowSize()
{
    var charSize = GetTerminalSize();
    var pixelSize = GetTerminalPixelSize(); // May return 0,0 if not supported
    
    return new WindowSize
    {
        ColumnsRows = charSize,
        Pixels = pixelSize
    };
}

public void Clear() => ClearRegion(ClearType.All);

public void ClearRegion(ClearType clearType)
{
    var sequence = clearType switch
    {
        ClearType.All => "\x1B[2J",
        ClearType.AfterCursor => "\x1B[0J",
        ClearType.BeforeCursor => "\x1B[1J",
        ClearType.CurrentLine => "\x1B[2K",
        ClearType.UntilNewLine => "\x1B[0K",
        _ => throw new ArgumentException($"Unsupported clear type: {clearType}")
    };
    
    writer.Write(sequence);
    writer.Flush();
}
```

### Cursor Management

```csharp
public void HideCursor()
{
    writer.Write("\x1B[?25l");
    writer.Flush();
}

public void ShowCursor()
{
    writer.Write("\x1B[?25h");
    writer.Flush();
}

public Position GetCursorPosition()
{
    // Send cursor position request and parse response
    writer.Write("\x1B[6n");
    writer.Flush();
    
    // Read response in format "\x1B[row;colR"
    var response = ReadCursorPositionResponse();
    return ParseCursorPosition(response);
}

public void SetCursorPosition(Position position)
{
    writer.Write($"\x1B[{position.Y + 1};{position.X + 1}H");
    writer.Flush();
}
```

## Key Challenges

### P/Invoke Requirements

Implement P/Invoke wrappers for Unix terminal operations:

```csharp
[StructLayout(LayoutKind.Sequential)]
public struct WinSize
{
    public ushort Row;
    public ushort Col;
    public ushort XPixel;
    public ushort YPixel;
}

[DllImport("libc", SetLastError = true)]
private static extern int ioctl(int fd, uint request, ref WinSize winSize);

private const uint TIOCGWINSZ = 0x5413; // Terminal size ioctl

private static Size GetTerminalSize()
{
    var winSize = new WinSize();
    if (ioctl(0, TIOCGWINSZ, ref winSize) == 0)
    {
        return new Size(winSize.Col, winSize.Row);
    }
    
    // Fallback to environment variables or default
    return GetSizeFromEnvironment() ?? new Size(80, 24);
}
```

### Input Response Handling

For operations like `GetCursorPosition()`, handle terminal input responses:

```csharp
private string ReadCursorPositionResponse()
{
    // Implementation depends on how we handle raw terminal input
    // May need to coordinate with input handling system
    throw new NotImplementedException("Requires coordination with input system");
}
```

### Scrolling Regions (Optional Feature)

Implement scrolling region support similar to Termion's feature-gated implementation:

```csharp
public void ScrollRegionUp(Range region, int amount)
{
    writer.Write($"\x1B[{region.Start.Value + 1};{region.End.Value}r");
    writer.Write($"\x1B[{amount}S");
    writer.Write("\x1B[r"); // Reset region
    writer.Flush();
}

public void ScrollRegionDown(Range region, int amount)
{
    writer.Write($"\x1B[{region.Start.Value + 1};{region.End.Value}r");
    writer.Write($"\x1B[{amount}T");
    writer.Write("\x1B[r"); // Reset region
    writer.Flush();
}
```

## Related Components

- `ITerminalBackend` interface definition
- `Color`, `Modifier`, and `Style` types from style system
- `Cell` type from buffer system
- `Position` and `Size` types from layout system
- Platform detection utilities

## Integration Points

- Backend factory for Unix platform detection
- Compatibility with Terminal wrapper class
- Integration with buffer rendering system
- Error handling and exception hierarchy

## Performance Considerations

- **StringBuilder Capacity**: Pre-calculate expected output size to avoid reallocations
- **State Tracking**: Maintain current colors/modifiers to minimize ANSI sequences
- **Cursor Optimization**: Track position to avoid unnecessary cursor movements
- **Batch Flushing**: Only flush when necessary to balance responsiveness with efficiency

## Testing Approach

- Unit tests for color conversion functions
- Tests for modifier difference calculations
- Integration tests with test terminal environments
- Performance benchmarks comparing to direct terminal output
- Platform-specific tests on Linux and macOS

## Acceptance Criteria

- [ ] Implements complete `ITerminalBackend` interface
- [ ] Optimized drawing minimizes cursor movements and style changes
- [ ] Color conversion supports all CycoTui color models
- [ ] Modifier handling correctly manages complex style interactions
- [ ] Terminal size detection works via P/Invoke to ioctl
- [ ] Cursor operations use standard ANSI sequences
- [ ] Clear operations support all required clear types
- [ ] Performance meets benchmarks for efficient terminal I/O
- [ ] Error handling provides meaningful error messages
- [ ] Integration tests pass on Linux and macOS platforms

## See Also

- [SPEC-BACKEND-001.md](../../specs/SPEC-BACKEND-001.md): Backend interface specification
- [004-BACKEND-ABSTRACTION-001.md](../../features/004-BACKEND-ABSTRACTION-001.md): Backend abstraction feature
- [CORE-BACKEND-INTERFACE-001](../CORE-BACKEND-INTERFACE-001/README.md): Backend interface implementation
- [Analysis: ratatui-termion/src/lib.rs](../../file-analyses/ratatui-termion-src-lib.md): Source analysis that informed this task