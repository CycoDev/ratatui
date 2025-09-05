---
id: SPEC-BUFFER-002
title: Buffer and Rendering Model
status: draft
date: 2023-11-28
---

# Buffer and Rendering Model

## Overview

The Buffer and Rendering Model specification defines the core data structures and algorithms for terminal rendering in CycoTui. This includes the Buffer structure, Cell model, diffing algorithm, and rendering pipeline.

## Scope

This specification covers:
- Buffer data structures and operations
- Cell definition and properties
- Content rendering approaches
- Diffing algorithms for efficient updates
- Unicode and grapheme handling
- API design for buffer manipulation

## Requirements

## Requirements

### Buffer Structure

**Core Data Model:**
- **Area Management**: Buffer must represent a rectangular area with x, y offset and width, height dimensions
- **Cell Storage**: Flat array/vector of cells with length = width * height for efficient access
- **Coordinate Mapping**: Convert between 2D coordinates and 1D array indices using `(y - offset_y) * width + (x - offset_x)`
- **Bounds Checking**: Provide both panicking and safe variants of cell access methods

**Cell Definition:**
- Each cell contains: character/grapheme, foreground color, background color, text modifiers, skip flag
- Support for multi-width characters (e.g., emoji, CJK characters)
- Unicode grapheme cluster support for proper text segmentation
- Style composition and inheritance
- **Reset Operation**: Cells must support reset functionality to clear content and return to default state
  - Resets character content to empty/default
  - Resets foreground and background colors to default
  - Clears all text modifiers
  - Used by Clear widget and buffer initialization

### Content Rendering

**Text Rendering:**
- **Unicode Support**: Handle grapheme clusters using proper Unicode segmentation
- **Width Calculation**: Accurately compute display width for all Unicode characters including emoji
- **Multi-Width Handling**: Properly render characters that span multiple terminal columns
- **Truncation**: Gracefully truncate text at grapheme boundaries when space is limited
- **Control Character Filtering**: Remove or escape control characters during rendering

**String Rendering Methods:**
- `SetString(x, y, text, style)` - Render plain text with styling
- `SetStringN(x, y, text, maxWidth, style)` - Render with width limit
- `SetLine(x, y, line, maxWidth)` - Render Line with multiple spans
- `SetSpan(x, y, span, maxWidth)` - Render single styled text span

### Diffing Algorithm

**Efficient Update Generation:**
- Compare two buffers and generate minimal set of changed cells
- Handle multi-width character invalidation (changes affect adjacent cells)
- Support cell skipping for optimization (mark cells as unchanged)
- Track invalidated regions from multi-width character changes
- Return list of (x, y, cell) tuples for terminal updates

**Performance Requirements:**
- O(n) complexity where n is buffer size
- Early termination for unchanged regions
- Minimal memory allocation during diffing

### Unicode Handling

**Grapheme Segmentation:**
- Use Unicode-compliant grapheme cluster boundaries
- Handle combining characters, variation selectors, zero-width joiners
- Support complex emoji sequences (family, profession, skin tone modifiers)

**Display Width Calculation:**
- Implement wcwidth-compatible width calculation
- Handle East Asian wide characters (width 2)
- Support zero-width characters
- Handle emoji width correctly (typically width 2)

**C# Implementation Requirements:**
- Use System.Globalization.StringInfo for grapheme enumeration
- Implement or integrate Unicode width calculation library
- Handle surrogate pairs in UTF-16 strings correctly

### Memory Management

**Allocation Strategy:**
- Use array-based storage for cells (prefer arrays over List<T> for performance)
- Consider ArrayPool<T> for large buffers to reduce GC pressure
- Implement efficient resize operations
- Support buffer reuse and pooling

**Performance Optimization:**
- Minimize allocations during rendering operations
- Use Span<T> and Memory<T> for efficient array operations
- Consider unsafe code for performance-critical paths
- Profile and optimize coordinate calculation methods

## Technical Approach

### Buffer Implementation Strategy

**Data Structure Design:**
```csharp
public struct BufferCell
{
    public string Content { get; set; }     // Grapheme cluster
    public Color Foreground { get; set; }
    public Color Background { get; set; }
    public TextModifiers Modifiers { get; set; }
    public bool Skip { get; set; }          // For optimization
}

public class Buffer
{
    public Rect Area { get; private set; }
    private BufferCell[] _content;
    
    // Indexer for convenient access
    public ref BufferCell this[int x, int y] { get; }
    public ref BufferCell this[Position pos] { get; }
    
    // Safe access methods
    public BufferCell? GetCell(int x, int y);
    public bool TryGetCell(int x, int y, out BufferCell cell);
}
```

**Coordinate Mapping:**
```csharp
private int IndexOf(int x, int y)
{
    if (!Area.Contains(x, y))
        throw new IndexOutOfRangeException();
    return (y - Area.Y) * Area.Width + (x - Area.X);
}

private (int x, int y) PositionOf(int index)
{
    var x = index % Area.Width + Area.X;
    var y = index / Area.Width + Area.Y;
    return (x, y);
}
```

### Unicode Text Rendering

**Grapheme Segmentation:**
```csharp
using System.Globalization;

public static IEnumerable<string> GetGraphemes(string text)
{
    var enumerator = StringInfo.GetTextElementEnumerator(text);
    while (enumerator.MoveNext())
    {
        yield return enumerator.GetTextElement();
    }
}
```

**Width Calculation:**
```csharp
// Will need to implement or integrate wcwidth functionality
public static int GetDisplayWidth(string grapheme)
{
    // Implementation details depend on chosen Unicode width library
    // Must handle: ASCII (width 1), wide chars (width 2), zero-width
    return UnicodeWidth.GetWidth(grapheme);
}
```

### Buffer Diffing Algorithm

**Diff Generation:**
```csharp
public IEnumerable<(int x, int y, BufferCell cell)> Diff(Buffer other)
{
    var updates = new List<(int, int, BufferCell)>();
    int invalidated = 0;
    int toSkip = 0;
    
    for (int i = 0; i < _content.Length; i++)
    {
        var current = other._content[i];
        var previous = _content[i];
        
        if (!current.Skip && 
            (!current.Equals(previous) || invalidated > 0) && 
            toSkip == 0)
        {
            var (x, y) = PositionOf(i);
            updates.Add((x, y, current));
        }
        
        toSkip = GetDisplayWidth(current.Content) - 1;
        var affectedWidth = Math.Max(
            GetDisplayWidth(current.Content),
            GetDisplayWidth(previous.Content)
        );
        invalidated = Math.Max(affectedWidth, invalidated) - 1;
    }
    
    return updates;
}
```

### Performance Optimizations

**Memory Management:**
- Use ArrayPool<BufferCell> for large buffers
- Implement buffer reuse to avoid allocations
- Consider struct-based cells to reduce GC pressure

**Hot Path Optimization:**
- Use unsafe code for coordinate calculations if needed
- Implement Span<T>-based operations for bulk operations
- Cache grapheme width calculations where possible

## Legacy Buffer Diff
public class BufferDiff
{
    // Compare two buffers and return differences
    public static IEnumerable<CellUpdate> Diff(Buffer previous, Buffer current);
    
    // Apply differences to terminal backend
    public static void Apply(ITerminalBackend backend, IEnumerable<CellUpdate> updates);
}

public struct CellUpdate
{
    public Position Position { get; set; }
    public Cell Cell { get; set; }
}
```

### Buffer Structure

**Core Type Definition:**
```csharp
public class Buffer : ICloneable, IEquatable<Buffer>
{
    /// The rectangular area this buffer represents
    public Rect Area { get; }
    
    /// The content cells arranged in row-major order
    /// Length must always equal Area.Width * Area.Height
    private readonly Cell[] content;
    
    // Constructors
    public static Buffer Empty(Rect area);
    public static Buffer Filled(Rect area, Cell cell);
    public static Buffer WithLines<T>(IEnumerable<T> lines) where T : IIntoLine;
    
    // Indexing - throws on invalid coordinates
    public Cell this[int x, int y] { get; set; }
    public Cell this[Position position] { get; set; }
    
    // Safe access methods
    public Cell? GetCell(Position position);
    public bool TryGetCell(Position position, out Cell cell);
    public bool TrySetCell(Position position, Cell cell);
    
    // String rendering
    public void SetString(int x, int y, string text, Style style);
    public void SetLine(int x, int y, Line line);
    public void SetSpan(int x, int y, Span span);
}
```

**Implementation Requirements:**
- Use `Cell[]` array for cache-friendly linear memory layout  
- Calculate index as `y * width + x` for 2D to 1D mapping
- Implement bounds checking in indexers (throw ArgumentOutOfRangeException)
- Provide Try* pattern methods for exception-free access
- Support efficient cloning for diffing operations
- Maintain area immutability after buffer creation
- Handle resize by creating new buffer instance

### Cell Definition

**Core Type Definition:**
```csharp
public struct Cell : IEquatable<Cell>
{
    // The Unicode grapheme cluster content (using string? for nullable)
    private string? symbol;
    
    // Visual styling properties
    public Color Foreground { get; set; }
    public Color Background { get; set; }
    public Color UnderlineColor { get; set; } // Conditional feature
    public TextModifier Modifier { get; set; }
    
    // Rendering optimization flag
    public bool Skip { get; set; }
    
    // Properties and methods
    public string Symbol => symbol ?? " "; // Default to space like Ratatui
    public static readonly Cell Empty;
    
    // Fluent API methods returning modified cell
    public Cell SetSymbol(string? newSymbol);
    public Cell SetChar(char character);
    public Cell SetStyle(Style style);
    public Cell SetForeground(Color color);
    public Cell SetBackground(Color color);
    public Cell SetSkip(bool skip);
    public Cell Reset();
    
    // Symbol merging for box drawing characters
    public Cell MergeSymbol(string symbol, MergeStrategy strategy);
    
    // Internal methods
    internal Cell AppendSymbol(string symbol); // For zero-width characters
}
```

**Implementation Requirements:**
- **String Storage**: Use nullable string (string?) for symbol storage, treating null as empty/space
- **Symbol Property**: Return " " (space) when symbol is null, matching Ratatui behavior
- **Memory Optimization**: Consider implementing CompactString equivalent for memory efficiency
- **Const Construction**: Support static readonly fields for common cells (Empty, common symbols)
- **Unicode Support**: Handle multi-byte characters, grapheme clusters, and emoji sequences
- **Box Drawing Merging**: Implement symbol merging algorithm for border collapsing
- **Equality Semantics**: Treat null symbol and " " (space) as equal in comparisons and hashing

**Unicode Requirements:**
- Support Unicode grapheme clusters using `System.Globalization.StringInfo`
- Handle multi-width characters (CJK characters, emoji) correctly
- Calculate display width using wcwidth-equivalent algorithm
- Preserve grapheme cluster boundaries during text operations
- Support zero-width characters through AppendSymbol method

**Specific Features from Ratatui Analysis:**
- **CompactString Alternative**: Investigate memory-efficient string storage for short symbols
- **Symbol Merging**: Port box drawing character merging algorithm with MergeStrategy enum
- **Feature Gates**: Support conditional underline color through build configuration
- **Serde Support**: Optionally support serialization/deserialization
- **Fluent API**: All mutation methods return modified cell for method chaining

### Diffing Algorithm

**Core Implementation:**
```csharp
public static class BufferDiff
{
    public static IEnumerable<CellUpdate> ComputeDiff(Buffer previous, Buffer current)
    {
        // Skip cells that are identical or marked with Skip flag
        // Return only cells that need terminal updates
    }
    
    public static void ApplyDiff(ITerminalBackend backend, IEnumerable<CellUpdate> updates)
    {
        // Group updates by location for efficient terminal writes
        // Use cursor positioning optimization
    }
}

public struct CellUpdate
{
    public Position Position { get; set; }
    public Cell PreviousCell { get; set; }
    public Cell NewCell { get; set; }
}
```

**Algorithm Requirements:**
- Compare buffers cell-by-cell for differences
- Use Skip flag to bypass unchanged cells
- Optimize for common patterns (single-line updates, small regions)
- Group adjacent updates to minimize cursor movement
- Support partial buffer diffing for performance

### Content Rendering
- Support text, symbols, and styled content
- Handle alignment and wrapping
- Respect Unicode width and combining characters
- Process newlines and whitespace consistently

### Unicode Handling
- Properly handle grapheme clusters
- Support East Asian wide characters
- Handle zero-width and combining characters
- Account for character width differences across terminals

### Performance Constraints
- Minimize allocations during rendering
- Optimize memory usage for large terminals
- Ensure efficient diffing for real-time updates
- Support high-frequency rendering (60+ FPS)

### Testing and Validation

**Buffer Comparison Utilities:**

Based on analysis of `ratatui-core/src/buffer/assert.rs`, the buffer system must provide comprehensive testing and validation capabilities:

```csharp
public static class BufferAssert
{
    /// <summary>
    /// Asserts that two buffers are equal, providing detailed diff information on failure
    /// </summary>
    public static void AreEqual(Buffer expected, Buffer actual);
    
    /// <summary>
    /// Compares two buffers and returns detailed difference information
    /// </summary>
    public static BufferDifference Compare(Buffer expected, Buffer actual);
    
    /// <summary>
    /// Creates a human-readable diff report for testing purposes
    /// </summary>
    public static string CreateDiffReport(Buffer expected, Buffer actual);
}

public class BufferDifference
{
    public bool AreasEqual { get; set; }
    public bool ContentsEqual { get; set; }
    public IReadOnlyList<CellDifference> CellDifferences { get; set; }
}

public struct CellDifference
{
    public Position Position { get; set; }
    public Cell Expected { get; set; }
    public Cell Actual { get; set; }
}
```

**Testing Requirements:**
- Provide detailed diff reporting similar to Ratatui's `assert_buffer_eq!` macro
- Support integration with popular C# testing frameworks (xUnit, NUnit, MSTest)
- Include position-specific error messages for failed assertions
- Support both area and content comparison modes
- Optimize for test performance while maintaining detailed error reporting

**Error Handling for Testing:**
- Throw descriptive exceptions with formatted diff information
- Support configurable verbosity levels for test output
- Provide separate "quick fail" vs "detailed analysis" modes
- Include visual representations of buffer differences where helpful

## Technical Approach

### Implementation Strategy
- Implement Buffer as a class with a backing array/list of Cells
- Use efficient data structures for cell storage and access
- Implement diffing as a separate algorithm
- Provide a clean API for buffer manipulation

### API Design
- Follow C# conventions (PascalCase methods, properties)
- Expose buffer operations through intuitive methods
- Support both mutable and immutable operations
- Provide extension methods for common operations
- Include factory methods for common buffer creation scenarios
- Consider providing a fluent API for buffer operations

### Memory Management
- Use appropriate data structures to minimize GC pressure
- Consider buffer pooling for frequent renders
- Use Span<T> and Memory<T> for efficient memory operations
- Implement IDisposable if resource cleanup is needed

### Optimization Techniques
- Implement specialized fast paths for common operations
- Use buffer regions to limit scope of operations
- Consider pre-allocating common cell styles
- Optimize diffing for terminal-specific update patterns

## Performance Targets
- Buffer creation: <5ms for 80x24 terminal
- Diff computation: <2ms for typical updates on 80x24 terminal
- Memory usage: <10MB for typical terminal sizes
- Render cycle (buffer creation, diff, output): <16ms (60 FPS)

## See Also
- SPEC-BACKEND-001: Terminal Backend Specification
- SPEC-STYLE-005: Style System Specification
- VISION-API-003: API Design Vision