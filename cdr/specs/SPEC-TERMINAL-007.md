---
id: SPEC-TERMINAL-007
title: Terminal Implementation Specification
status: draft
date: 2023-11-28
---

# Terminal Implementation Specification

## Overview

The Terminal class is the main entry point for CycoTui applications. It manages the terminal state, handles double-buffered rendering, and provides the primary API for drawing frames to the terminal.

Based on analysis of `ratatui-core/src/terminal/terminal.rs`, this specification defines the requirements for implementing the Terminal class in C#.

## Scope

This specification covers:

1. Terminal class structure and state management
2. Double-buffered rendering system
3. Viewport management (Fullscreen, Inline, Fixed)
4. Frame rendering pipeline
5. Cursor management
6. Terminal resizing and content insertion

### Viewport Configuration

The Terminal must support three viewport modes:

```csharp
public enum Viewport
{
    /// Uses the entire terminal area for drawing
    Fullscreen,
    
    /// Fixed height area drawn below cursor position
    /// Height specified in lines, width matches terminal
    Inline,
    
    /// Fixed rectangular area within terminal
    Fixed
}

public class InlineViewport
{
    public ushort Height { get; set; }
}

public class FixedViewport
{
    public Rect Area { get; set; }
}
```

#### Viewport Behavior Requirements

- **Fullscreen Mode**: 
  - Uses entire terminal area for drawing
  - Default viewport mode
  - Area updates automatically on terminal resize

- **Inline Mode**:
  - Fixed height specified in number of lines
  - Width matches terminal width
  - Drawn below current cursor position
  - Must validate height against terminal capabilities

- **Fixed Mode**:
  - Uses specified rectangular area within terminal
  - Must validate area bounds against terminal size
  - Area remains fixed regardless of terminal resize

## Requirements

### Terminal Class Structure

```csharp
public class Terminal<TBackend> : IDisposable 
    where TBackend : ITerminalBackend
{
    // Core state
    private readonly TBackend backend;
    private Buffer[] buffers;  // Double buffer system
    private int currentBufferIndex;
    private bool hiddenCursor;
    
    // Viewport management
    private Viewport viewport;
    private Rect viewportArea;
    private Rect lastKnownArea;
    private Position lastKnownCursorPos;
    
    // Frame tracking
    private ulong frameCount;
}

public class TerminalOptions
{
    public Viewport Viewport { get; set; } = Viewport.Fullscreen;
}
```

### Construction and Initialization

```csharp
// Primary constructors
public static Terminal<TBackend> New<TBackend>(TBackend backend) 
    where TBackend : ITerminalBackend;

public static Terminal<TBackend> WithOptions<TBackend>(
    TBackend backend, 
    TerminalOptions options) 
    where TBackend : ITerminalBackend;
```

### Core Rendering Methods

```csharp
// Main rendering methods
public CompletedFrame Draw(Action<Frame> renderCallback);
public CompletedFrame TryDraw<TError>(Func<Frame, Result<Unit, TError>> renderCallback)
    where TError : Exception;

// Frame management
public Frame GetFrame();
public void Flush();

// Double buffering
private void SwapBuffers();
```

### Buffer Management

The Terminal must maintain a double-buffer system:

1. **Current Buffer**: Buffer being rendered to
2. **Previous Buffer**: Buffer from the last completed frame
3. **Diffing**: Compare buffers to minimize terminal writes
4. **Swapping**: Exchange buffers after successful render

### Viewport Management

Support three viewport modes:

1. **Fullscreen**: Terminal uses entire screen
2. **Inline**: Terminal appears inline with existing content  
3. **Fixed**: Terminal uses a fixed rectangular area

```csharp
public enum ViewportType
{
    Fullscreen,
    Inline,
    Fixed
}

public abstract class Viewport
{
    public static Viewport Fullscreen { get; }
    public static Viewport Inline(ushort height);
    public static Viewport Fixed(Rect area);
}
```

### Frame Rendering Pipeline

The complete rendering process:

1. **Autoresize**: Check for terminal size changes and resize buffers if needed
2. **Get Frame**: Create frame object with reference to current buffer
3. **Render Callback**: Execute user's rendering function
4. **Extract State**: Get cursor position from frame before it's dropped
5. **Flush**: Send buffer differences to backend
6. **Cursor Management**: Show/hide and position cursor as needed
7. **Swap Buffers**: Prepare for next frame
8. **Backend Flush**: Ensure all output is written
9. **Update Counters**: Increment frame count

### Cursor Management

```csharp
// Cursor visibility
public void HideCursor();
public void ShowCursor();

// Cursor positioning  
public Position GetCursorPosition();
public void SetCursorPosition(Position position);
public void SetCursorPosition(ushort x, ushort y);  // Convenience overload
```

### Terminal Operations

```csharp
// Terminal state
public Size GetSize();
public void Clear();
public void Resize(Rect area);
public void Autoresize();

// Content insertion (for inline viewports)
public void InsertBefore(ushort height, Action<Buffer> drawFunction);
```

### Error Handling and Cleanup

```csharp
// IDisposable implementation
public void Dispose()
{
    // Restore cursor if hidden
    if (hiddenCursor)
    {
        try { ShowCursor(); }
        catch { /* Log error but don't throw */ }
    }
    
    backend?.Dispose();
}
```

## Technical Approach

### Double Buffer Implementation

```csharp
private void Flush()
{
    var previousBuffer = buffers[1 - currentBufferIndex];
    var currentBuffer = buffers[currentBufferIndex];
    
    var updates = previousBuffer.Diff(currentBuffer);
    
    if (updates.Any())
    {
        var lastUpdate = updates.Last();
        lastKnownCursorPos = new Position(lastUpdate.x, lastUpdate.y);
    }
    
    backend.Draw(updates);
}
```

### Viewport Sizing for Inline Mode

For inline viewports, calculate position based on:
- Current cursor position
- Available screen space
- Requested viewport height
- Offset from previous viewport position

### Content Insertion Algorithms

Two implementation strategies:

1. **Without Scrolling Regions**: Use line-by-line drawing with manual scrolling
2. **With Scrolling Regions**: Use terminal's built-in scrolling capabilities

### Autoresize Logic

```csharp
public void Autoresize()
{
    if (viewport.Type == ViewportType.Fullscreen || 
        viewport.Type == ViewportType.Inline)
    {
        var currentSize = backend.GetSize();
        var currentArea = new Rect(0, 0, currentSize.Width, currentSize.Height);
        
        if (currentArea != lastKnownArea)
        {
            Resize(currentArea);
        }
    }
}
```

## Performance Targets

- **Buffer Diffing**: O(n) where n is the number of cells
- **Frame Rate**: Support 60+ FPS for smooth animations
- **Memory Usage**: Minimize allocations during steady-state rendering
- **Terminal I/O**: Batch writes to minimize system calls

## Examples

### Basic Usage

```csharp
using var backend = new CrosstermBackend();
using var terminal = Terminal.New(backend);

terminal.Draw(frame =>
{
    var area = frame.Area;
    frame.RenderWidget(new Paragraph("Hello World!"), area);
});
```

### Error Handling

```csharp
var result = terminal.TryDraw(frame =>
{
    // Potentially failing operations
    var data = LoadData(); // might throw
    frame.RenderWidget(new DataWidget(data), frame.Area);
    return Result.Ok();
});

if (result.IsError)
{
    // Handle rendering error
}
```

## See Also

- `SPEC-BACKEND-001.md` - Backend interface specification
- `SPEC-BUFFER-002.md` - Buffer and cell specification  
- `SPEC-VIEWPORT-008.md` - Viewport management specification
- `004-BACKEND-ABSTRACTION-001.md` - Backend abstraction feature
- `001-BUFFER-MODEL-001.md` - Buffer model feature