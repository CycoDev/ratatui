---
id: SPEC-TEXT-001
title: Text System Specification
status: draft
date: 2023-11-28
---

# Text System Specification

## Overview

The text system provides a hierarchical approach to representing styled text in terminal applications. It consists of three primary types that form a composition hierarchy: Span (uniform styling), Line (per-span styling), and Text (multi-line with alignment and styling). The Text type serves as the primary container for terminal text display, supporting multi-line content with rich styling and alignment capabilities.

## Scope

This specification covers:
- Text hierarchy design and implementation  
- String conversion and promotion patterns
- Memory management and performance considerations
- Unicode and grapheme cluster handling
- Text reflow algorithms (wrapping and truncation)
- Line composition and layout strategies
- Integration with the style system

## Requirements

### Text Reflow Requirements
- **LineComposer Interface**: Abstraction for text reflow algorithms
  - `NextLine()` method returns wrapped lines with width and alignment information
  - Support for different reflow strategies (wrapping vs truncation)
  - Iterator-like interface but yields slices of internal buffer for efficiency
- **Word Wrapping**: Intelligent text wrapping that preserves word boundaries
  - Break lines on word boundaries when possible
  - Handle overflow by breaking within words when necessary
  - Support for configurable trimming of leading/trailing whitespace
  - Efficient state machine implementation with minimal allocations
  - Handle complex Unicode scenarios (CJK characters, non-breaking spaces, zero-width characters)
- **Line Truncation**: Simple overflow handling by cutting off excess text
  - Truncate lines that exceed maximum width
  - Support horizontal scrolling offset for text navigation
  - Preserve text alignment during truncation
  - Efficient single-pass processing
- **Unicode-Aware Processing**: 
  - Accurate character width calculation for proper line measurement
  - Proper grapheme cluster boundary detection to prevent character corruption
  - Support for double-width characters (CJK), zero-width characters, and combining marks
  - Handle edge cases like non-breaking spaces and zero-width spaces correctly

### Unicode Character Handling
- Support for Unicode half-block characters (▀, ▄, █) in terminal graphics
- **Unicode Width Calculation**: Essential for accurate text layout and widget sizing
  - Character width detection for various Unicode categories (narrow, wide, zero-width)
  - East Asian Width property support (Fullwidth, Halfwidth, Wide, Narrow, Ambiguous, Neutral)
  - Zero-width character handling (combining marks, control characters, format characters)
  - Grapheme cluster width calculation for complex characters (emoji, ligatures)
  - Support for both standard terminal width and CJK-aware width calculations
  - Integration with tabs widget for proper tab spacing and overflow detection
  - Required for table widget column sizing and cell alignment
  - Performance-optimized width lookup tables for common character ranges
- Proper handling of box-drawing and block element characters
- Character width calculation for Unicode symbols
- Terminal capability detection for Unicode support
- Fallback strategies for limited Unicode environments

### Text Hierarchy Structure
- **Span**: Represents a single string segment with uniform styling
- **Line**: Collection of Spans representing a single line with varied styling  
- **Text**: Collection of Lines representing multi-line styled text
- **Masked**: Secure text wrapper that displays as masked characters
- **StyledGrapheme**: Individual grapheme with associated style information for rendering

### Span Requirements (Core Unit)
- **Structure**: Contains content as Clone-on-Write string and uniform Style
- **Construction**: Support raw (default style) and styled creation patterns
- **Fluent API**: All setter methods consume self and return modified instance with `#[must_use]` equivalent
- **Content Management**: Set, replace, or modify content through fluent interface
- **Style Management**: 
  - `Style()` method replaces existing style completely
  - `PatchStyle()` method merges new style with existing (preserves unspecified properties)
  - `ResetStyle()` method clears all style to default
- **Unicode Support**: 
  - Accurate width calculation using Unicode width standards
  - Proper grapheme cluster iteration with style inheritance
  - Filter out control characters during rendering
  - Handle zero-width and multi-width characters correctly
- **Widget Integration**: Implement Widget interface for direct rendering to buffer
- **Conversion Support**: Implicit conversion from any `Display` implementer to Span
- **Line Conversion**: Convert to aligned Line (left, center, right) with fluent methods
- **Rendering Logic**:
  - Complex grapheme positioning for zero-width characters
  - Multi-width character handling with cell clearing
  - Area overflow handling and truncation
  - Style inheritance from base style during rendering

### StyledGrapheme Requirements
- Represents the atomic unit of styled text for rendering purposes
- Contains a grapheme cluster string and associated Style
- Provides accurate whitespace detection including Unicode edge cases:
  - Zero Width Space (U+200B) considered whitespace
  - Non-Breaking Space (U+00A0) NOT considered whitespace
  - Standard Unicode whitespace characters considered whitespace
- Implements style mutation operations (immutable pattern)
- Supports generic style conversion from any type convertible to Style

### Line Structure and Behavior
- **Composition**: Lines contain a vector of Spans with overall style and optional alignment
- **Construction**: Support multiple construction patterns (raw, styled, from spans)
- **Newline Handling**: Automatically split content with newlines into multiple spans
- **Fluent Interface**: Support method chaining for configuration (style, alignment, spans)
- **Unicode Width**: Accurate display width calculation including CJK characters and emoji
- **Alignment**: Support for Left, Center, and Right alignment with proper truncation
- **Safe Truncation**: Prevent corruption of multi-byte characters during rendering
- **Rendering**: Efficient span skipping and truncation for optimal performance
- Support seamless conversion from string types to styled text types

### Masked Text Requirements
- **Structure**: Contains original string content and a masking character
- **Display Behavior**: Shows masked representation while preserving original content
- **Debug Support**: Debug output shows original string for development purposes
- **Integration**: Seamless conversion to Text and string types using masked content
- **Performance**: Efficient masking operation with character-level replacement
- **Unicode Support**: Works with any Unicode character as mask character
- **Security**: Protects sensitive content from visual display while maintaining functionality

### Text Requirements (Multi-line Container)
- **Structure**: Contains a vector of Lines with overall Style and optional Alignment
- **Construction**: Support multiple creation patterns:
  - `Raw(content)` - Creates text with no style from string content
  - `Styled(content, style)` - Creates text with specific style
  - Implicit conversion from string, Line, Vec<Line>, and other types
- **Multi-line Handling**: 
  - Automatically splits string content on newlines to create multiple Lines
  - Handles empty strings by creating single empty line
  - Preserves trailing newlines appropriately
- **Style System Integration**:
  - `Style()` method replaces existing style completely (fluent)
  - `PatchStyle()` method merges new style with existing (additive)
  - `ResetStyle()` method clears all style to default
  - Text style applied first, then individual line styles
- **Alignment Support**:
  - Text-level alignment provides default for all lines
  - Individual line alignment can override text alignment
  - Support for Left, Center, Right alignment
  - Fluent methods: `LeftAligned()`, `Centered()`, `RightAligned()`
- **Content Management**:
  - `PushLine(line)` - Adds a line to the text
  - `PushSpan(span)` - Adds a span to the last line (creates line if empty)
  - Support for addition operations (Text + Line, Text + Text)
  - Extension/concatenation support for building complex text
- **Measurement Operations**:
  - `Width()` - Returns maximum width of all lines (Unicode-aware)
  - `Height()` - Returns number of lines
- **Widget Integration**: 
  - Implements Widget interface for direct rendering to buffer
  - Renders each line sequentially with proper styling and alignment
  - Handles area intersection to avoid out-of-bounds rendering
- **Iteration Support**:
  - `Iter()` - Returns iterator over lines
  - `IterMut()` - Returns mutable iterator over lines  
  - `IntoIter()` - Consuming iterator over lines
  - Support for both reference and consuming iteration patterns
- **Collection Operations**:
  - Implement `Extend<T>` for adding multiple lines
  - Support for `FromIterator<T>` construction
  - Addition and addition-assignment operators for composition
- **Conversions**:
  - From any Display-implementing type via `ToText` trait
  - Seamless conversion from strings, Lines, Spans
  - Format trait implementation for string conversion

### Memory Management
- Efficient string storage using shared references where possible
- Minimize allocations during text composition and styling
- Support both owned and borrowed string content
- Optimize for common scenarios (single style, simple text)

### Unicode Support
- Proper handling of grapheme clusters
- Support for combining characters and diacritics
- Correct width calculation for display purposes
- Platform-appropriate text encoding handling
- **Unicode Width Calculation**: 
  - Implement equivalent to Rust's `unicode-width` crate functionality
  - Support East Asian width standards (full-width, half-width characters)
  - Handle zero-width characters (combining marks, format characters)
  - Provide accurate column width calculation for terminal rendering
  - Support for emoji width detection
  - Character boundary detection for safe text splitting

## Technical Approach

### Implementation Strategy

#### Core Types
```csharp
// Text reflow abstraction
public interface ILineComposer<T>
{
    WrappedLine<T>? NextLine();
}

// Result of line composition
public struct WrappedLine<T>
{
    public ReadOnlySpan<StyledGrapheme<T>> Graphemes { get; }
    public ushort Width { get; }
    public Alignment Alignment { get; }
    
    public WrappedLine(ReadOnlySpan<StyledGrapheme<T>> graphemes, ushort width, Alignment alignment)
}

// Word-boundary wrapping implementation
public class WordWrapper<TLine, TGrapheme> : ILineComposer<TGrapheme>
    where TLine : IEnumerable<(IEnumerable<StyledGrapheme<TGrapheme>>, Alignment)>
{
    private readonly TLine _inputLines;
    private readonly ushort _maxLineWidth;
    private readonly bool _trim;
    private readonly Queue<List<StyledGrapheme<TGrapheme>>> _wrappedLines;
    private Alignment _currentAlignment;
    
    // Object pooling for performance
    private readonly List<StyledGrapheme<TGrapheme>> _pendingWord;
    private readonly Queue<StyledGrapheme<TGrapheme>> _pendingWhitespace;
    private readonly Stack<List<StyledGrapheme<TGrapheme>>> _linePool;
    
    public WordWrapper(TLine lines, ushort maxLineWidth, bool trim = true)
    
    public WrappedLine<TGrapheme>? NextLine()
    {
        // Complex state machine implementation
        // Handle word boundaries, overflow detection, whitespace management
        // Implement efficient buffering and object reuse
    }
    
    private void ProcessInput(IEnumerable<StyledGrapheme<TGrapheme>> lineSymbols)
    {
        // Word boundary detection algorithm
        // Overflow handling with trimming support
        // Efficient whitespace and word buffer management
    }
}

// Line truncation implementation  
public class LineTruncator<TLine, TGrapheme> : ILineComposer<TGrapheme>
    where TLine : IEnumerable<(IEnumerable<StyledGrapheme<TGrapheme>>, Alignment)>
{
    private readonly TLine _inputLines;
    private readonly ushort _maxLineWidth;
    private ushort _horizontalOffset;
    private readonly List<StyledGrapheme<TGrapheme>> _currentLine;
    
    public LineTruncator(TLine lines, ushort maxLineWidth)
    
    public void SetHorizontalOffset(ushort offset)
    
    public WrappedLine<TGrapheme>? NextLine()
    {
        // Simple truncation with horizontal offset support
        // Single-pass processing for efficiency
        // Unicode-aware character boundary detection
    }
}

// Unicode helper functions
public static class UnicodeHelpers
{
    public static string TrimOffset(string source, int offset)
    {
        // Character-boundary-aware string trimming
        // Handles grapheme clusters properly
        // Returns substring starting at visual offset
    }
    
    public static int GetDisplayWidth(string text)
    {
        // Unicode width calculation
        // East Asian width support
        // Zero-width character handling
    }
    
    public static bool IsWhitespace(string grapheme)
    {
        // Unicode whitespace detection
        // Special handling for non-breaking space, zero-width space
        // Standard Unicode categories
    }
}

// Text - multi-line text container with styling and alignment (primary type)
public class Text : IReadOnlyList<Line>, IEnumerable<Line>
{
    public IReadOnlyList<Line> Lines { get; }
    public Style Style { get; }
    public Alignment? Alignment { get; }
    
    // Construction
    public Text(IEnumerable<Line> lines)
    public static Text Raw(string content)
    public static Text Styled<TStyle>(string content, TStyle style) where TStyle : IConvertible<Style>
    
    // Fluent API (methods consume and return modified instance)
    [MustUseReturnValue]
    public Text SetStyle<TStyle>(TStyle style) where TStyle : IConvertible<Style>
    
    [MustUseReturnValue]
    public Text PatchStyle<TStyle>(TStyle style) where TStyle : IConvertible<Style>
    
    [MustUseReturnValue]
    public Text ResetStyle()
    
    [MustUseReturnValue]
    public Text SetAlignment(Alignment alignment)
    
    [MustUseReturnValue]
    public Text LeftAligned()
    
    [MustUseReturnValue]
    public Text Centered()
    
    [MustUseReturnValue]
    public Text RightAligned()
    
    // Content management
    public void PushLine<T>(T line) where T : IConvertible<Line>
    public void PushSpan<T>(T span) where T : IConvertible<Span>
    
    // Measurement
    public int Width()  // Maximum width of all lines (Unicode-aware)
    public int Height() // Number of lines
    
    // Iteration
    public IEnumerator<Line> GetEnumerator()
    public Line[] ToArray()
    
    // Widget integration - key for components like ListItem
    void IWidget.Render(Rect area, Buffer buffer) // Enable direct rendering
}
```

#### Widget Component Integration
Based on analysis of ListItem and similar components:
- **Content Wrapper Pattern**: Text serves as content field in widget components
  - Example: `ListItem` contains `Text content` field for flexible content representation
  - Enables widget components to accept any type convertible to Text
- **Dimension Delegation**: Widget components delegate size calculations to Text
  - `ListItem.Height` delegates to `Text.Height()` for line count
  - `ListItem.Width` delegates to `Text.Width()` for maximum line width
- **Style Composition**: Text styling integrates with component-level styling
  - Component style acts as base layer
  - Text content style adds specific formatting
  - Clear precedence rules for style inheritance
    
    // Widget integration
    public void Render(Rect area, Buffer buffer)
    
    // Conversions
    public static implicit operator Text(string content)
    public static implicit operator Text(Line line)
    public static implicit operator Text(Span span)
    public static implicit operator Text(List<Line> lines)
    
    // Collection operations
    public static Text operator +(Text left, Text right)
    public static Text operator +(Text left, Line right)
    
    // Display integration
    public override string ToString() // Multi-line string representation
}

// Span - uniform styling for a string segment (core building block)
public class Span
{
    public string Content { get; }
    public Style Style { get; }
    
    // Construction
    public Span(string content, Style style = default)
    public static Span Raw(string content)
    public static Span Styled<TStyle>(string content, TStyle style) where TStyle : IConvertible<Style>
    
    // Fluent API (methods consume and return modified instance)
    [MustUseReturnValue]
    public Span SetContent(string content)
    
    [MustUseReturnValue] 
    public Span SetStyle<TStyle>(TStyle style) where TStyle : IConvertible<Style>
    
    [MustUseReturnValue]
    public Span PatchStyle<TStyle>(TStyle style) where TStyle : IConvertible<Style>
    
    [MustUseReturnValue]
    public Span ResetStyle()
    
    // Text operations
    public int Width() // Unicode width calculation
    public IEnumerable<StyledGrapheme> StyledGraphemes(Style baseStyle)
    
    // Line conversion with alignment
    public Line IntoLeftAlignedLine()
    public Line IntoCenteredLine()  
    public Line IntoRightAlignedLine()
    
    // Widget integration
    public void Render(Rect area, Buffer buffer)
    
    // Conversions
    public static implicit operator Span(string content)
    public static implicit operator Line(Span span)
    public static implicit operator Text(Span span)
}

// Line - collection of spans for single line with alignment and styling
public class Line : IReadOnlyList<Span>
{
    public IReadOnlyList<Span> Spans { get; }
    public Style Style { get; }
    public Alignment? Alignment { get; }
    
    public Line(IEnumerable<Span> spans)
    public Line(string content)
    
    // Construction methods
    public static Line Raw(string content)
    public static Line Styled(string content, Style style)
    public static Line FromSpans(IEnumerable<Span> spans)
    
    // Fluent configuration
    public Line SetStyle(Style style)
    public Line SetAlignment(Alignment alignment)
    public Line SetSpans(IEnumerable<Span> spans)
    public Line AddSpan(Span span)
    
    // Alignment shortcuts
    public Line LeftAligned()
    public Line Centered()  
    public Line RightAligned()
    
    // Text operations
    public int Width()  // Unicode display width
    public IEnumerable<StyledGrapheme> StyledGraphemes(Style baseStyle)
    
    // Conversions
    public static implicit operator Line(string content)
    public static implicit operator Line(Span span)
    public static implicit operator Text(Line line)
}

// ToText trait equivalent - extension method approach
public static class ToTextExtensions
{
    public static Text ToText<T>(this T value) where T : IFormattable
        => Text.Raw(value.ToString());
}

// Extend operations for Text
public static class TextExtensions
{
    public static void Extend<T>(this Text text, IEnumerable<T> items) where T : IConvertible<Line>
    {
        foreach (var item in items)
            text.PushLine(item);
    }
}

// StyledGrapheme - atomic rendering unit
public struct StyledGrapheme
{
    public string Symbol { get; }
    public Style Style { get; }
    
    public StyledGrapheme(string symbol, Style style)
    
    // Style mutation (immutable pattern)
    public StyledGrapheme SetStyle<TStyle>(TStyle style) where TStyle : IConvertible<Style>
    
    // Unicode whitespace detection with special cases
    public bool IsWhitespace()
    {
        // Zero Width Space (U+200B) considered whitespace
        // Non-Breaking Space (U+00A0) NOT considered whitespace  
        // Standard Unicode whitespace characters considered whitespace
    }
}
```

#### Conversion Extensions
```csharp
public static class TextConversions
{
    public static Span ToSpan(this string text, Style style = default)
    public static Line ToLine(this string text)
    public static Text ToText(this string text)
    
    public static Line ToLine(this Span span)
    public static Text ToText(this Line line)
}
```

### Memory Optimization
- Use `ReadOnlyMemory<char>` for string content to enable slicing without allocation
- Implement copy-on-write semantics for style modifications
- Pool common Style instances to reduce allocation overhead
- Consider string interning for frequently used text content

### Unicode Handling
- Use `StringInfo` class for grapheme cluster enumeration
- Leverage .NET Rune support for proper Unicode scalar value handling
- Implement East Asian width calculation for proper display width
- Handle normalization requirements for text comparison

## Platform-Specific Details

### Windows
- Handle UTF-16 encoding appropriately
- Consider console output encoding requirements
- Support for Windows-specific text rendering optimizations

### Unix
- Ensure proper UTF-8 handling for terminal output
- Support for various terminal text capabilities
- Handle differences in font rendering and character support

## Terms and Definitions

- **Grapheme**: A user-perceived character that may consist of multiple Unicode code points
- **Span**: A text segment with uniform styling
- **Line**: A single line composed of multiple styled segments
- **Text**: Multi-line text composed of multiple lines
- **Copy-on-Write**: Memory optimization technique that delays copying until modification occurs

## Examples

### Basic Usage
```csharp
// Simple string conversion
Line title = "My Application";
Text content = "Line 1\nLine 2\nLine 3";

// Styled content
var styledTitle = new Line(new[]
{
    Span.Styled("My", Style.Default.Foreground(Color.Yellow)),
    Span.Raw(" Application")
});

// Complex multi-line with varied styling
var complexText = new Text(new[]
{
    new Line(new[] { Span.Styled("Header", Style.Default.Bold()) }),
    new Line("Regular content"),
    new Line(new[] 
    { 
        Span.Raw("Status: "),
        Span.Styled("OK", Style.Default.Foreground(Color.Green))
    })
});
```

### Widget Integration
```csharp
// Widgets should accept flexible text input
public class Block
{
    public Line Title { get; set; }
    
    public Block SetTitle(string title) => SetTitle((Line)title);
    public Block SetTitle(Span title) => SetTitle((Line)title);
    public Block SetTitle(Line title) { Title = title; return this; }
}
```

## Performance Targets

- String-to-Span conversion: O(1) for simple cases
- Line composition: O(n) where n is number of spans
- Text rendering: O(m*n) where m is lines and n is average spans per line
- Memory overhead: < 50% for typical styled text scenarios

## See Also

- SPEC-STYLE-005: Style System Specification
- SPEC-BUFFER-002: Buffer and Rendering Model  
- SPEC-WIDGET-003: Widget Implementation Specification
- 005-STYLE-SYSTEM-001: Style System Feature