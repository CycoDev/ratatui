# Source File Analysis: ratatui-core/src/text/text.rs

## Basic Information

- **File Path**: ratatui-core/src/text/text.rs
- **Component**: Text and Style
- **Analysis Date**: 2023-11-28

## Key Types and Interfaces

### `Text<'a>`
- **Purpose**: Represents a string split over one or more lines, used for displaying text in the terminal
- **Key Properties**:
  - `alignment: Option<Alignment>` - Text alignment (Left, Center, Right)
  - `style: Style` - Overall styling for the text
  - `lines: Vec<Line<'a>>` - Collection of lines that make up the text
- **Key Methods**:
  - `raw(content)` - Creates text with no style
  - `styled(content, style)` - Creates text with a specific style
  - `width()` - Returns max width of all lines
  - `height()` - Returns number of lines
  - `style(style)` - Sets the style (fluent)
  - `patch_style(style)` - Adds modifiers without overwriting
  - `reset_style()` - Resets style to default
  - `alignment(alignment)` - Sets alignment (fluent)
  - `left_aligned()`, `centered()`, `right_aligned()` - Alignment helpers
  - `push_line(line)` - Adds a line to the text
  - `push_span(span)` - Adds a span to the last line
- **Usage Pattern**: Primary text container, supports multiple lines with individual styling and overall text styling

### `ToText` trait
- **Purpose**: Trait for converting values to Text
- **Key Methods**: `to_text()` - Converts value to Text
- **Usage Pattern**: Automatically implemented for any type implementing Display

## Core Behaviors

### **Multi-line Text Handling**
- **Description**: Automatically splits strings on newlines to create multiple Line objects
- **Implementation Approach**: Uses `str.lines()` iterator to split content
- **Performance Considerations**: Handles both borrowed and owned strings efficiently via Cow
- **Edge Cases**: Empty strings create a single empty line; preserves trailing newlines

### **Style Composition**
- **Description**: Supports both overall text styling and per-line styling
- **Implementation Approach**: Text style is applied first, then individual line styles
- **Performance Considerations**: Style patching combines styles without full replacement
- **Edge Cases**: Style conflicts resolved by line-level styles taking precedence

### **Alignment System**
- **Description**: Supports text-level and line-level alignment with override capabilities
- **Implementation Approach**: Text alignment provides default, line alignment overrides
- **Performance Considerations**: Alignment calculated during rendering
- **Edge Cases**: None alignment inherits from parent widget

### **Widget Rendering**
- **Description**: Text implements Widget trait for direct rendering to buffer
- **Implementation Approach**: Renders each line sequentially, applying styles and alignment
- **Performance Considerations**: Uses area intersection to avoid out-of-bounds rendering
- **Edge Cases**: Truncates content that exceeds render area

## Platform-Specific Code

No platform-specific code identified in this file.

## Dependencies

### Internal Dependencies
- `crate::buffer::Buffer` - For rendering text to screen buffer
- `crate::layout::{Alignment, Rect}` - For positioning and alignment
- `crate::style::{Style, Styled}` - For text styling
- `crate::text::{Line, Span}` - For text hierarchy components
- `crate::widgets::Widget` - For rendering interface

### External Dependencies
- `unicode_width::UnicodeWidthStr` - For calculating display width of Unicode text
- `alloc` collections - For Vec, String, Cow (no-std compatibility)

## Key Algorithms and Techniques

### **Unicode Width Calculation**
- **Purpose**: Accurately calculates display width considering Unicode and CJK characters
- **Approach**: Delegates to unicode_width crate, finds maximum width across all lines
- **Complexity**: O(n) where n is total character count
- **Optimizations**: Uses iterator.max() for efficient line width comparison

### **Text Splitting Algorithm**
- **Purpose**: Converts string content into Line objects
- **Approach**: Uses string.lines() iterator with Cow for zero-copy when possible
- **Complexity**: O(n) where n is string length
- **Optimizations**: Avoids allocation for borrowed strings when possible

### **Style Patching**
- **Purpose**: Combines styles without overwriting existing properties
- **Approach**: Uses Style.patch() method for additive style composition
- **Complexity**: O(1) style combination
- **Optimizations**: Fluent interface enables method chaining

## C# Port Considerations

### **Idiomatic Translations**
- `Vec<Line<'a>>` → `List<Line>` or `IReadOnlyList<Line>`
- `Option<Alignment>` → `Alignment?` (nullable enum)
- `Cow<'a, str>` → `string` or `ReadOnlySpan<char>` (consider memory efficiency)
- `Into<Style>` → implicit conversion operators or method overloads
- Fluent setters → C# fluent pattern with return this
- `IntoIterator` → `IEnumerable<T>` implementation

### **Potential Challenges**
- Lifetime management - C# doesn't have explicit lifetimes, need to manage string ownership
- Zero-copy string handling - May need to use Span<char> or ReadOnlySpan<char> for efficiency
- Trait system - Need to design interface hierarchy for conversions and styling
- Unicode width calculation - Need to find or implement Unicode width library for .NET

### **.NET API Equivalents**
- `unicode_width` → Need to find .NET Unicode width library or port algorithm
- `alloc::vec::Vec` → `System.Collections.Generic.List<T>`
- `core::fmt::Display` → `override ToString()` and `IFormattable`
- `core::iter::Iterator` → `IEnumerable<T>` and LINQ
- Pattern matching → C# pattern matching or switch expressions

## Documentation Updates Needed

### **Features**
- `006-TEXT-SYSTEM-001.md` - Text hierarchy, multi-line handling, styling composition
- `005-STYLE-SYSTEM-001.md` - Style patching and composition behavior

### **Specifications**
- `SPEC-TEXT-001.md` - Text structure, alignment system, Unicode width handling
- `SPEC-STYLE-005.md` - Style composition and patching algorithms

### **Tasks**
- `TEXT-HIERARCHY-001` - Implement Text/Line/Span hierarchy
- `TEXT-UNICODE-WIDTH-001` - Unicode width calculation for .NET
- `CORE-STYLE-SYSTEM-001` - Style composition and patching
- `TEXT-LINE-001` - Multi-line text handling and line management

## Questions and Issues

### **Unicode Width Calculation**
- **Context**: Need reliable Unicode width calculation for .NET
- **Potential Solutions**: Research existing .NET Unicode libraries, consider porting wcwidth algorithm, evaluate performance implications

### **Memory Efficiency**
- **Context**: Rust uses Cow for zero-copy strings, need C# equivalent
- **Potential Solutions**: Use ReadOnlySpan<char>, string interning, or accept copying overhead

### **Style System Integration**
- **Context**: Need to ensure consistent style behavior across text hierarchy
- **Potential Solutions**: Design clear inheritance and override rules, consider immutable style objects